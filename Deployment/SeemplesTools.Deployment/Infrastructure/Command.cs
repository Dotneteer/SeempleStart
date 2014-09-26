using System;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Egy végrehajtható parancs absztrakt megvalósítása.
    /// </summary>
    public abstract class Command
    {
        #region Properties

        /// <summary>
        /// Az a parancskonténer, amelyhez ez a parancs kapcsolódik
        /// </summary>
        public ICommandContainer Container { get; set; }

        /// <summary>
        /// A parancs aktuális állapota
        /// </summary>
        public CommandStatus Status { get; protected set; }

        /// <summary>
        /// A parancs eredeti szövege
        /// </summary>
        public string Original { get; set; }
        
        /// <summary>
        /// A parancs neve
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// A parancs aktuális végrehajtási fázisához kapcsolódó szöveg
        /// </summary>
        public abstract string ProgressText { get; }
        
        /// <summary>
        /// A parancs véglegesítési fázisához kapcsolódó szöveg
        /// </summary>
        public abstract string CommitProgressText { get; }

        /// <summary>
        /// A parancs visszagörgetési fázisához kapcsolódó szöveg
        /// </summary>
        public abstract string RollbackProgressText { get; }

        #endregion

        #region Initialization

        /// <summary>
        /// A parancs argumentumainak értelmezése
        /// </summary>
        /// <param name="option">Az opció neve</param>
        /// <param name="argument">Az argumentum értéke</param>
        /// <param name="original">A parancs szövege</param>
        /// <returns>Sikerült az értelmezés?</returns>
        public bool ParseOption(string option, string argument, string original)
        {
            if (Status != CommandStatus.Initializing)
            {
                throw new InvalidOperationException(String.Format(
                    "Internal error: Command.ParseOption called in invalid status {0}.", Status));
            }

            if (option == null)
            {
                throw new ArgumentNullException("option");
            }

            if (original == null)
            {
                throw new ArgumentNullException("original");
            }

            return DoParseOption(option, argument, original);
        }


        /// <summary>
        /// Értelmezi a parancs argumentumait
        /// </summary>
        /// <param name="option">Az opció neve</param>
        /// <param name="argument">Az argumentum értéke</param>
        /// <param name="original">A parancs szövege</param>
        /// <returns>Sikerült az értelmezés?</returns>
        protected abstract bool DoParseOption(string option, string argument, string original);

        /// <summary>
        /// Befejezi a parancs inicializálását
        /// </summary>
        public void FinishInitialization()
        {
            if (Status != CommandStatus.Initializing)
            {
                throw new InvalidOperationException(String.Format(
                    "Internal error: Command.FinishInitialization called in invalid status {0}.", Status));
            }
            DoFinishInitialization();
            Status = CommandStatus.Runnable;
        }

        /// <summary>
        /// Befejezi a parancs inicializálását
        /// </summary>
        protected abstract void DoFinishInitialization();

        /// <summary>
        /// Leellenőrzi a parancs kötelező paraméterének kitöltöttségét
        /// </summary>
        /// <typeparam name="T">A paraméter típusa</typeparam>
        /// <param name="value">A paraméter értéke</param>
        /// <param name="name">A paraméter neve</param>
        /// <param name="original">A parancs szövege</param>
        protected void CheckMandatoryParameter<T>(T value, string name, string original)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (value == null || value.Equals(default(T)))
            {
                throw new ParameterException(String.Format(
                    "Mandatory option {0} not specified for command {1}.", name, original));
            }
        }

        #endregion

        #region Execution

        /// <summary>
        /// Végrehajtja a parancsot
        /// </summary>
        public abstract void Run();
        
        /// <summary>
        /// Véglegesíti a parancs végrehajtását
        /// </summary>
        public abstract void Commit();
        
        /// <summary>
        /// Visszagörgeti a parancsot
        /// </summary>
        public abstract void Rollback();

        #endregion
    }
}
