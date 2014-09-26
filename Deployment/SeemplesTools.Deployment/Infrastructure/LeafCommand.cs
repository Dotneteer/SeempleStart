using System;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az osztály egy önmagában elemi egységként végrehajtható parancsot ad meg.
    /// </summary>
    public abstract class LeafCommand : Command
    {
        /// <summary>
        /// Végrehajtja a parancsot
        /// </summary>
        public override void Run()
        {
            if (Container.Cancelled)
            {
                throw new ApplicationException("Deployment transaction cancelled by user.");
            }

            if (Status != CommandStatus.Runnable)
            {
                throw new InvalidOperationException(String.Format(
                    "Internal error: Command.Run called in invalid status {0}.", Status));
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
        protected abstract void DoRun();

        /// <summary>
        /// Véglegesíti a parancs végrehajtását
        /// </summary>
        public override void Commit()
        {
            if (Status != CommandStatus.Succeeded)
            {
                throw new InvalidOperationException(String.Format(
                    "Internal error: Command.Commit called in invalid status {0}.", Status));
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
        /// A parancs véglegesítése
        /// </summary>
        protected abstract void DoCommit();

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
                    "Internal error: Command.Rollback called in invalid status {0}.", Status));
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
        /// A parancs visszagörgetése
        /// </summary>
        protected abstract void DoRollback();
    }
}
