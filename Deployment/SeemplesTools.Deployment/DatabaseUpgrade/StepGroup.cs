using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.DatabaseUpgrade.Steps;

namespace SeemplesTools.Deployment.DatabaseUpgrade
{
    /// <summary>
    /// A group of steps responsible for upgrading a logical database from one version to another.
    /// </summary>
    public class StepGroup
    {
        #region Properties

        /// <summary>
        /// Name of the logical database this step group is performed on.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Version of the logical database after this step group is completed.
        /// </summary>
        private readonly DatabaseVersion _targetVersion;

        /// <summary>
        /// Version of the logical database after this step group is completed.
        /// </summary>
        public DatabaseVersion TargetVersion
        {
            get { return _targetVersion; }
        }

        /// <summary>
        /// Name of schemas associated with the logical database after this step group is completed.
        /// </summary>
        private HashSet<string> _schemaNames;

        /// <summary>
        /// Name of schemas associated with the logical database after this step group is completed.
        /// </summary>
        public HashSet<string> SchemaNames
        {
            get { return _schemaNames; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _schemaNames = value;
            }
        }

        /// <summary>
        /// Set if the step group is cross-branch (marked by the upgrade script).
        /// </summary>
        public bool CrossBranch { get; set; }

        /// <summary>
        /// List of dependencies specified in the upgrade scripts.
        /// </summary>
        public List<UnresolvedDependency> UnresolvedDependencies { get; private set; }

        /// <summary>
        /// The list of steps contained in this step group.
        /// </summary>
        private readonly LinkedList<Step> _steps;

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                var stepWithDescription = _steps.FirstOrDefault(s => s.GetType() == typeof(RunScriptStep)) ??
                                          _steps.FirstOrDefault();
                return stepWithDescription == null 
                    ? null 
                    : stepWithDescription.LongDescription;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes an instance of the <see cref="StepGroup"/> class.
        /// </summary>
        /// <param name="databaseName">Name of the logical database this step group is performed on.</param>
        /// <param name="targetVersion">Version of the logical database after this step group is performed.</param>
        public StepGroup(string databaseName, DatabaseVersion targetVersion)
        {
            if (databaseName == null)
            {
                throw new ArgumentNullException("databaseName");
            }

            DatabaseName = databaseName;
            _targetVersion = targetVersion;
            _schemaNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            UnresolvedDependencies = new List<UnresolvedDependency>();
            _steps = new LinkedList<Step>();
        }

        /// <summary>
        /// Add a step to the end of the step group.
        /// </summary>
        /// <param name="step">The step to add to the step group.</param>
        public void AddLast(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException("step");
            }
            _steps.AddLast(step);
        }

        /// <summary>
        /// Add a step to the start of the step group.
        /// </summary>
        /// <param name="step">The step to add to the step group.</param>
        public void AddFirst(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException("step");
            }
            _steps.AddFirst(step);
        }

        #endregion

        #region Execution

        /// <summary>
        /// Returns the short descriptions of the steps which have one.
        /// </summary>
        /// <returns>Non-null short descriptions of steps in the step group.</returns>
        public IEnumerable<string> GetShortDescriptions()
        {
            return _steps.Where(s => s.ShortDescription != null).Select(s => s.ShortDescription);
        }

        /// <summary>
        /// Returns the long descriptions of the steps.
        /// </summary>
        /// <returns>Long descriptions of steps in the step group.</returns>
        public IEnumerable<string> GetLongDescriptions()
        {
            return _steps.Select(s => s.LongDescription);
        }

        /// <summary>
        /// Execute the steps contained in the step group.
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step group should
        /// be executed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var step in _steps)
            {
                step.Execute(connection, transaction);
            }
        }

        #endregion

        #region Debugging

        /// <summary>
        /// Returns a human-readable description of the step.
        /// </summary>
        /// <returns>Human-readable description of the step.</returns>
        public override string ToString()
        {
            return String.Join("; ", _steps);
        }

        #endregion
    }
}
