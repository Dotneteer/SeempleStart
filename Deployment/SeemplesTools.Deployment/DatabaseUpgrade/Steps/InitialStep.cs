using System;
using System.Data.SqlClient;

namespace SeemplesTools.Deployment.DatabaseUpgrade.Steps
{
    /// <summary>
    /// Dummy initial step for a logical database.
    /// </summary>
    public class InitialStep : Step
    {
        #region Properties

        /// <summary>
        /// Short description (not including database name).
        /// </summary>
        public override string ShortDescription
        {
            get
            {
                return StepGroup.TargetVersion == null
                    ? "new logical database"
                    : String.Format("{0} (current)", StepGroup.TargetVersion);
            }
        }

        /// <summary>
        /// Long description (including database name).
        /// </summary>
        public override string LongDescription
        {
            get
            {
                return StepGroup.TargetVersion == null
                    ? String.Format("{0} (new)", StepGroup.DatabaseName)
                    : String.Format("{0} {1} (current)", StepGroup.DatabaseName, StepGroup.TargetVersion);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes an instance of the <see cref="InitialStep"/> class.
        /// </summary>
        /// <param name="stepGroup">The step group containing the newly initialized step.</param>
        public InitialStep(StepGroup stepGroup)
            : base(stepGroup)
        {
        }

        #endregion

        #region Execution

        /// <summary>
        /// Execute the step (do nothing).
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step should be
        /// performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public override void Execute(SqlConnection connection, SqlTransaction transaction)
        {
        }

        #endregion
    }
}
