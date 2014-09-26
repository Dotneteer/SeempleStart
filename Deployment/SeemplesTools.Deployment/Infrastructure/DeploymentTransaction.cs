using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Text;
using SeemplesTools.Deployment.Common;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az osztály egy deploymenthez tartozó parancskonténert ír le.
    /// </summary>
    public class DeploymentTransaction : ICommandContainer
    {
        #region Properties

        // --- A deploymenthez tartozó parancsok listája
        private readonly List<Command> _commands = new List<Command>();
        
        // --- A naplóállomány
        private StreamWriter _log;

        /// <summary>
        /// A naplófájl neve
        /// </summary>
        public string LogFile { get; set; }
        
        /// <summary>
        /// A deployment tulajdonságainak gyűjteménye
        /// </summary>
        public Dictionary<string, object> Properties { get; private set; }
        
        /// <summary>
        /// A visszagörgetés során előálló kivételek listája
        /// </summary>
        public IList<Exception> RollbackExceptions { get; private set; }

        /// <summary>
        /// Üres a konténer?
        /// </summary>
        public bool IsEmpty
        {
            get { return _commands.Count == 0; }
        }

        /// <summary>
        /// Meg lett szakítva a parancsok végrehajtása?
        /// </summary>
        public bool Cancelled { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Egy üres konténer inicializálása
        /// </summary>
        public DeploymentTransaction()
        {
            Properties = new Dictionary<string, object>();
            RollbackExceptions = new List<Exception>();
        }

        /// <summary>
        /// Új parancs hozzáadása a konténerhez
        /// </summary>
        /// <param name="command">A hozzáadandó parancs</param>
        public void AddCommand(Command command)
        {
            if (command.Status == CommandStatus.Initializing)
                command.FinishInitialization();

            _commands.Add(command);
            command.Container = this;
        }

        #endregion

        #region Progress

        /// <summary>
        /// A parancsok előrehaladottságának jelzése
        /// </summary>
        /// <param name="command">A parancsot leíró objektum</param>
        /// <param name="progressText">Az aktuális előrehaladottsági fázis</param>
        /// <param name="commandProgress">Az előrehaladás mértéke</param>
        public void ReportProgress(Command command, string progressText, int commandProgress = 0)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            var index = _commands.IndexOf(command);
            if (index == -1)
            {
                throw new ArgumentException(
                    "Internal error: DeploymentTransaction.ReportProgress called with an unknown command.");
            }

            DeploymentUserInterface.Current.SetProgress(
                ProgressHelper.GetProgress(index, _commands.Count, commandProgress),
                progressText);
        }

        #endregion

        #region Execution

        /// <summary>
        /// A parancs futtatása
        /// </summary>
        public void Run()
        {
            // --- A parancsobjektum elhelyezése a veremben
            s_RunningTransactions.Push(this);

            try
            {
                InitializeLog();
                try
                {
                    DeploymentUserInterface.Current.StartProgress();
                    try
                    {
                        Log("Setup transaction started.");
                        RunCommands();

                        try
                        {
                            Log("Committing transaction.");
                        }
                        catch
                        {
                            RollbackFromIndex(_commands.Count - 1);
                            throw;
                        }

                        CommitCommands();
                        Log("Setup transaction finished.");
                    }
                    finally
                    {
                        DeploymentUserInterface.Current.EndProgress();
                    }
                }
                finally
                {
                    if (_log != null)
                    {
                        _log.Dispose();
                        _log = null;
                    }
                }
            }
            finally
            {
                s_RunningTransactions.Pop();
            }
        }

        /// <summary>
        /// A parancsok futtatása
        /// </summary>
        private void RunCommands()
        {
            var i = -1;
            string currentCommandType = null;
            try
            {
                for (i = 0; i < _commands.Count; i++)
                {
                    var command = _commands[i];
                    currentCommandType = command.GetType().Name;

                    Log("Starting command {0} ({1}): {2}", currentCommandType, i, command.ProgressText);
                    ReportProgress(command, command.ProgressText);

                    command.Run();
                    Log("Finished command {0} ({1}).", currentCommandType, i);

                    currentCommandType = null;
                }
            }
            catch (Exception e)
            {
                try
                {
                    Log("An error has occurred during execution of command {0} ({1}): {2}", currentCommandType, i, e);
                    Log("Rolling back transaction.");
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // --- Ez a kivétel szándékosan lett elnyelve
                }

                RollbackFromIndex(i);
                throw;
            }
        }

        /// <summary>
        /// A parancsok véglegesítése
        /// </summary>
        private void CommitCommands()
        {
            var i = -1;
            string currentCommandType = null;

            try
            {
                for (i = 0; i < _commands.Count; i++)
                {
                    var command = _commands[i];
                    currentCommandType = command.GetType().Name;

                    Log("Committing command {0} ({1}): {2}", currentCommandType, i, command.CommitProgressText);
                    ReportProgress(command, command.CommitProgressText);

                    command.Commit();
                    Log("Finished committing command {0} ({1}).", currentCommandType, i);

                    currentCommandType = null;
                }
            }
            catch (Exception e)
            {
                try
                {
                    Log("An error has occurred during committing of command {0} ({1}): {2}", currentCommandType, i, e);
                    Log("Rolling back transaction.");
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // --- Ez a kivétel szándékosan lett elnyelve
                }

                RollbackFromIndex(_commands.Count - 1);
                throw;
            }
        }

        /// <summary>
        /// A parancsok visszagörgetése a megadott indextől
        /// </summary>
        /// <param name="index">Ettől az indextől kell a végrehajtott parancsokat visszagörgetni</param>
        private void RollbackFromIndex(int index)
        {
            try
            {
                string currentCommandType = null;
                for (var i = index; i >= 0; i--)
                {
                    try
                    {
                        var command = _commands[i];
                        currentCommandType = command.GetType().Name;

                        Log("Rolling back command {0} ({1}): {2}", currentCommandType, i, command.CommitProgressText);
                        ReportProgress(command, command.RollbackProgressText);

                        command.Rollback();
                        Log("Finished rolling back command {0} ({1}).", currentCommandType, i);

                        currentCommandType = null;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            RollbackExceptions.Add(e);
                        }
                        // ReSharper disable once EmptyGeneralCatchClause
                        catch
                        {
                            // --- Ez a kivétel szándékosan lett elnyelve
                        }

                        try
                        {
                            Log("An error has occurred while rolling back command (0) ({1}): {2}", currentCommandType, i, e);
                            Log("Continuing rollback.");
                        }
                        // ReSharper disable once EmptyGeneralCatchClause
                        catch
                        {
                            // --- Ez a kivétel szándékosan lett elnyelve
                        }
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    RollbackExceptions.Add(e);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // --- Ez a kivétel szándékosan lett elnyelve
                }

                try
                {
                    Log("An error has occurred while rolling back the setup transaction: {0}", e);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // --- Ez a kivétel szándékosan lett elnyelve
                }
            }
        }

        /// <summary>
        /// A parancs végrehajtásának megszakítása
        /// </summary>
        public void Cancel()
        {
            Cancelled = true;
        }

        #endregion

        #region Logging

        /// <summary>
        /// A naplózás iniciazliálása
        /// </summary>
        private void InitializeLog()
        {
            if (String.IsNullOrEmpty(LogFile)) return;
            try
            {
                _log = new StreamWriter(LogFile, true)
                {
                    AutoFlush = true
                };
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error opening log file.", e);
            }
        }

        /// <summary>
        /// Naplózza a megadott üzenetet
        /// </summary>
        /// <param name="message">Üzenet</param>
        public void Log(string message)
        {
            if (_log == null) return;
            _log.Write("{0:yyyy-MM-dd HH:mm:ss}: ", DateTime.Now);
            _log.WriteLine(message);
        }

        /// <summary>
        /// Naplózza a megadott paraméterezett üzenetet
        /// </summary>
        /// <param name="message">Üzenet</param>
        /// <param name="args">Az üzenet paraméterei</param>
        public void Log(string message, params object[] args)
        {
            if (_log == null) return;
            _log.Write("{0:yyyy-MM-dd HH:mm:ss}: ", DateTime.Now);
            _log.WriteLine(message, args);
        }

        /// <summary>
        /// Naplózza a megadott SQL parancsot
        /// </summary>
        /// <param name="command">SQL parancs</param>
        public void LogSqlCommand(SqlCommand command)
        {
            var message = new StringBuilder();
            message.AppendLine("Executing SQL command:");
            message.AppendLine(command.CommandText);
            if (command.Parameters.Count > 0)
            {
                message.AppendLine("with parameters:");
                foreach (SqlParameter parameter in command.Parameters)
                {
                    message.AppendFormat("  {0} = {1}", parameter.ParameterName, parameter.Value);
                    message.AppendLine();
                }
            }

            Log(message.ToString());
        }

        #endregion

        #region Current

        // --- A deployment tranzakciók végrehajtási verme
        private static readonly Stack<DeploymentTransaction> s_RunningTransactions = new Stack<DeploymentTransaction>();
        
        /// <summary>
        /// Visszaadja az aktuálisan futó deployment tranzakciót
        /// </summary>
        public static DeploymentTransaction Current
        {
            get
            {
                return s_RunningTransactions.Count == 0
                    ? null
                    : s_RunningTransactions.Peek();
            }
        }

        #endregion
    }
}
