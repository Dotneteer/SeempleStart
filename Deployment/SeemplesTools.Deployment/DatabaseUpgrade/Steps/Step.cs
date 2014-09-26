using System;
using System.Data.SqlClient;
using SeemplesTools.Deployment.Commands;

namespace SeemplesTools.Deployment.DatabaseUpgrade.Steps
{
    /// <summary>
    /// Abstract class describing a step to be executed by
    /// <see cref="CreateLogicalDatabasesCommand"/>.
    /// </summary>
    public abstract class Step
    {
        #region Properties

        /// <summary>
        /// The step group this step is contained in.
        /// </summary>
        private readonly StepGroup _stepGroup;

        /// <summary>
        /// The step group this step is contained in.
        /// </summary>
        public StepGroup StepGroup
        {
            get { return _stepGroup; }
        }

        /// <summary>
        /// Short description (not including database name).
        /// </summary>
        public abstract string ShortDescription { get; }

        /// <summary>
        /// Long description (including database name).
        /// </summary>
        public abstract string LongDescription { get; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class.
        /// </summary>
        /// <param name="stepGroup">The step group the newly initialized step is contained in.</param>
        /// <param name="addToStart">True to add the step to the start of the step group instead of the end.</param>
        protected Step(StepGroup stepGroup, bool addToStart = false)
        {
            if (stepGroup == null)
            {
                throw new ArgumentNullException("stepGroup");
            }

            _stepGroup = stepGroup;

            if (addToStart)
            {
                stepGroup.AddFirst(this);
            }
            else
            {
                stepGroup.AddLast(this);
            }
        }

        #endregion

        #region Execution

        /// <summary>
        /// Execute the step.
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step should be
        /// performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public abstract void Execute(SqlConnection connection, SqlTransaction transaction);

        #endregion

        #region Debugging

        /// <summary>
        /// Returns a human-readable description of the step.
        /// </summary>
        /// <returns>Human-readable description of the step.</returns>
        public override string ToString()
        {
            return LongDescription;
        }

        #endregion
    }
}
