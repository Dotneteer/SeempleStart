using System;
using System.Collections.Generic;
using SeemplesTools.Deployment.Common;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az osztály egy több parancsot végrehajtó parancskonténert valósít meg
    /// </summary>
    public abstract class ContainerCommand : Command, ICommandContainer
    {
        #region Properties

        /// <summary>
        /// A konténerben lévő végrehajtandó parancsok
        /// </summary>
        protected List<Command> Commands = new List<Command>();
       
        // --- Az utoljára végrehajtott parancs indexe
        private int _lastExecutedCommand = -1;

        /// <summary>
        /// Meg lett szakítva?
        /// </summary>
        public bool Cancelled
        {
            get { return Container.Cancelled; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Befejezi a parancs inicializálását
        /// </summary>
        protected override void DoFinishInitialization()
        {
            foreach (var command in Commands)
            {
                command.FinishInitialization();
            }
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

            var index = Commands.IndexOf(command);
            if (index == -1)
            {
                throw new ArgumentException(
                    "Internal error: ContainerCommand.ReportProgress called with an unknown command.");
            }

            if (Container == null)
            {
                throw new InvalidOperationException(
                    "Internal error: ContainerCommand.ReportProgress called on a container that is not a member of a deployment transaction.");
            }

            Container.ReportProgress(this, progressText, 
                ProgressHelper.GetProgress(index, Commands.Count, commandProgress));
        }

        #endregion

        #region Execution

        /// <summary>
        /// Végrehajtja a parancsot
        /// </summary>
        public override void Run()
        {
            foreach (var command in Commands)
            {
                command.Container = this;
            }

            if (Container.Cancelled)
            {
                throw new ApplicationException("Deployment transaction cancelled by user.");
            }

            if (Status != CommandStatus.Runnable)
            {
                throw new InvalidOperationException(String.Format(
                    "ContainerCommand.Run called in invalid status {0}.", Status));
            }

            try
            {
                DoRun();
                Status = CommandStatus.Succeeded;
            }
            catch
            {
                Status = CommandStatus.Failed;
                throw;
            }
        }

        /// <summary>
        /// A parancs végrehajtása
        /// </summary>
        private void DoRun()
        {
            string currentCommandType = null;

            try
            {
                // --- A parancsokat a definiálás sorrendjében hajtjuk végre
                for (_lastExecutedCommand = 0; _lastExecutedCommand < Commands.Count; _lastExecutedCommand++)
                {
                    var command = Commands[_lastExecutedCommand];
                    currentCommandType = command.GetType().Name;

                    DeploymentTransaction.Current.Log("Starting nested command {0} ({1}): {2}",
                        currentCommandType, _lastExecutedCommand, command.ProgressText);
                    ReportProgress(command, command.ProgressText);

                    command.Run();
                    DeploymentTransaction.Current.Log("Finished nested command {0} ({1}).",
                        currentCommandType, _lastExecutedCommand);

                    currentCommandType = null;
                }
            }
            catch (Exception e)
            {
                try
                {
                    DeploymentTransaction.Current.Log(
                        "An error has occurred during execution of nested command {0} ({1}): {2}",
                        currentCommandType, _lastExecutedCommand, e);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // --- Ez a kivétel szándékosan lett elnyelve
                }

                throw;
            }
        }

        /// <summary>
        /// Véglegesíti a parancs végrehajtását
        /// </summary>
        public override void Commit()
        {
            if (Status != CommandStatus.Succeeded)
            {
                throw new InvalidOperationException(String.Format(
                    "ContainerCommand.Commit called in invalid status {0}.", Status));
            }

            try
            {
                DoCommit();
                Status = CommandStatus.Committed;
            }
            catch
            {
                Status = CommandStatus.CommitFailed;
                throw;
            }
        }

        /// <summary>
        /// Véglegesíti a parancsot
        /// </summary>
        private void DoCommit()
        {
            var i = -1;
            string currentCommandType = null;

            try
            {
                // --- A parancsokat a definiálás sorrendjében véglegesítjük
                for (i = 0; i < Commands.Count; i++)
                {
                    var command = Commands[i];
                    currentCommandType = command.GetType().Name;

                    DeploymentTransaction.Current.Log("Committing nested command {0} ({1}): {2}",
                        currentCommandType, i, command.CommitProgressText);
                    ReportProgress(command, command.CommitProgressText);

                    command.Commit();
                    DeploymentTransaction.Current.Log("Finished committing nested command {0} ({1}).",
                        currentCommandType, i);

                    currentCommandType = null;
                }
            }
            catch (Exception e)
            {
                try
                {
                    DeploymentTransaction.Current.Log(
                        "An error has occurred during committing of nested command {0} ({1}): {2}",
                        currentCommandType, i, e);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // --- Ez a kivétel szándékosan lett elnyelve
                }

                throw;
            }
        }

        /// <summary>
        /// Visszagörgeti a parancsot
        /// </summary>
        public override void Rollback()
        {
            if (Status != CommandStatus.Succeeded &&
                Status != CommandStatus.Failed &&
                Status != CommandStatus.Committed &&
                Status != CommandStatus.CommitFailed)
            {
                throw new InvalidOperationException(String.Format(
                    "ContainerCommand.Rollback called in invalid status {0}.", Status));
            }

            try
            {
                DoRollback();
                Status = CommandStatus.RolledBack;
            }
            catch
            {
                Status = CommandStatus.RollbackFailed;
                throw;
            }
        }

        /// <summary>
        /// Visszagörgeti a parancsot
        /// </summary>
        private void DoRollback()
        {
            var i = _lastExecutedCommand >= Commands.Count ? Commands.Count - 1 : _lastExecutedCommand;
            string currentCommandType = null;

            while (i >= 0)
            {
                try
                {
                    var command = Commands[i];
                    currentCommandType = command.GetType().Name;

                    DeploymentTransaction.Current.Log("Rolling back nested command {0} ({1}): {2}",
                        currentCommandType, i, command.RollbackProgressText);
                    ReportProgress(command, command.RollbackProgressText);

                    command.Rollback();
                    DeploymentTransaction.Current.Log("Finished rolling back nested command {0} ({1}).",
                        currentCommandType, i);

                    currentCommandType = null;
                }
                catch (Exception e)
                {
                    try
                    {
                        DeploymentTransaction.Current.RollbackExceptions.Add(e);
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                        // --- Ez a kivétel szándékosan lett elnyelve
                    }

                    try
                    {
                        DeploymentTransaction.Current.Log(
                            "An error has occurred while rolling back nested command (0) ({1}): {2}",
                            currentCommandType, i, e);
                        DeploymentTransaction.Current.Log("Continuing rollback.");
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                        // --- Ez a kivétel szándékosan lett elnyelve
                    }
                }

                i--;
            }
        }

        #endregion
    }
}
