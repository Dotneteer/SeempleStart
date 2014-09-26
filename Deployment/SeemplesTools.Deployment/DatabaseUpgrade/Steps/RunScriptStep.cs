using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.DatabaseUpgrade.Steps
{
    /// <summary>
    /// <see cref="Step"/> that runs a database upgrade or create script and optionally its corresponding test data
    /// script.
    /// </summary>
    public class RunScriptStep : Step
    {
        #region Properties

        /// <summary>
        /// Short description (not including database name).
        /// </summary>
        public override string ShortDescription
        {
            get { return StepGroup.TargetVersion.ToString(); }
        }

        /// <summary>
        /// Long description (including database name).
        /// </summary>
        public override string LongDescription
        {
            get { return String.Format("{0} to {1}", StepGroup.DatabaseName, StepGroup.TargetVersion); }
        }

        /// <summary>
        /// File name of the upgrade or create script.
        /// </summary>
        private readonly string _scriptPath;

        /// <summary>
        /// File name of the localization script.
        /// </summary>
        private readonly string _testDataScriptPath;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of the <see cref="RunScriptStep"/> class.
        /// </summary>
        /// <param name="stepGroup">The step group containing the newly initialized step.</param>
        /// <param name="sourceVersion">Version of the logical database before the operation or null if a create script is
        /// to be run.</param>
        /// <param name="scriptPath">Path of the upgrade or create script.</param>
        /// <param name="testDataScriptPath">Path of the test data script or null if test data is not to be inserted.
        /// </param>
        public RunScriptStep(
            StepGroup stepGroup,
            DatabaseVersion sourceVersion,
            string scriptPath,
            string testDataScriptPath)
            : base(stepGroup)
        {
            if (scriptPath == null)
            {
                throw new ArgumentNullException("scriptPath");
            }

            _scriptPath = scriptPath;
            _testDataScriptPath = testDataScriptPath;

            ReadMetadata(sourceVersion);

            if (stepGroup.SchemaNames.Count == 0)
            {
                throw new ApplicationException(String.Format("No schemas specified in {0}", _scriptPath));
            }
        }

        /// <summary>
        /// Read metadata from the script; set the <see cref="StepGroup.CrossBranch"/>,
        /// <see cref="StepGroup.SchemaNames"/> and <see cref="StepGroup.UnresolvedDependencies"/> properties accordingly.
        /// </summary>
        /// <param name="sourceVersion">Version of the logical database before the operation or null if a create script is
        /// to be run.</param>
        private void ReadMetadata(DatabaseVersion sourceVersion)
        {
            var lines = File.ReadAllLines(_scriptPath);

            foreach (var line in lines)
            {
                string name;
                string value;

                if (!ReadCustomParameter(line, out name, out value)) continue;

                switch (name)
                {
                    case "schemas":
                        var schemas = value.Split(',').Select(s => s.Trim());

                        // ReSharper disable once PossibleMultipleEnumeration
                        if (schemas.Any(String.IsNullOrEmpty))
                        {
                            throw new ApplicationException(String.Format(
                                "Invalid value for @Yw-Schemas flag in {0} (empty schema name)", _scriptPath));
                        }

                        // ReSharper disable once PossibleMultipleEnumeration
                        StepGroup.SchemaNames = new HashSet<string>(schemas, StringComparer.InvariantCultureIgnoreCase);
                        break;

                    case "crossbranch":
                        bool crossBranch;
                        if (!Boolean.TryParse(value, out crossBranch))
                        {
                            throw new ApplicationException(String.Format(
                                "Invalid value for @Yw-CrossBranch flag in {0}: {1}.", _scriptPath, value));
                        }

                        StepGroup.CrossBranch = crossBranch;
                        break;
                    
                    case "before":
                        if (value.IndexOf('-') != -1)
                        {
                            throw new ApplicationException(String.Format(
                                "Invalid 'Before' dependency descriptor in {0}: {1}.", _scriptPath, value));
                        }

                        if (sourceVersion == null)
                        {
                            StepGroup.UnresolvedDependencies.Add(new UnresolvedDependency(
                                DependencyType.ReverseSoft,
                                value,
                                DatabaseVersion.MinValue,
                                DatabaseVersion.MaxValue));
                        }
                        else
                        {
                            StepGroup.UnresolvedDependencies.Add(new UnresolvedDependency(
                                DependencyType.Hard,
                                value,
                                sourceVersion,
                                sourceVersion));
                            StepGroup.UnresolvedDependencies.Add(new UnresolvedDependency(
                                DependencyType.ReverseSoft,
                                value,
                                sourceVersion.Next,
                                DatabaseVersion.MaxValue));
                        }
                        break;
                    
                    case "after":
                        if (value.IndexOf('-') != -1)
                        {
                            throw new ApplicationException(String.Format(
                                "Invalid 'After' dependency descriptor in {0}: {1}.", _scriptPath, value));
                        }

                        StepGroup.UnresolvedDependencies.Add(new UnresolvedDependency(
                            DependencyType.Hard,
                            value,
                            StepGroup.TargetVersion,
                            StepGroup.TargetVersion));
                        StepGroup.UnresolvedDependencies.Add(new UnresolvedDependency(
                            DependencyType.ReverseSoft,
                            value,
                            StepGroup.TargetVersion.Next,
                            DatabaseVersion.MaxValue));
                        break;
                    
                    case "dependency":
                        StepGroup.UnresolvedDependencies.Add(UnresolvedDependency.FromDescriptor(
                            DependencyType.Hard,
                            value,
                            sourceVersion,
                            _scriptPath));
                        break;
                    
                    case "optional-dependency":
                        StepGroup.UnresolvedDependencies.Add(UnresolvedDependency.FromDescriptor(
                            DependencyType.Optional,
                            value,
                            sourceVersion,
                            _scriptPath));
                        break;
                    
                    case "soft-dependency":
                        StepGroup.UnresolvedDependencies.Add(UnresolvedDependency.FromDescriptor(
                            DependencyType.Soft,
                            value,
                            sourceVersion,
                            _scriptPath));
                        break;
                    
                    case "reverse-dependency":
                        StepGroup.UnresolvedDependencies.Add(UnresolvedDependency.FromDescriptor(
                            DependencyType.ReverseHard,
                            value,
                            sourceVersion,
                            _scriptPath));
                        break;
                    
                    case "reverse-optional-dependency":
                        StepGroup.UnresolvedDependencies.Add(UnresolvedDependency.FromDescriptor(
                            DependencyType.ReverseOptional,
                            value,
                            sourceVersion,
                            _scriptPath));
                        break;
                    
                    case "reverse-soft-dependency":
                        StepGroup.UnresolvedDependencies.Add(UnresolvedDependency.FromDescriptor(
                            DependencyType.ReverseSoft,
                            value,
                            sourceVersion,
                            _scriptPath));
                        break;
                }
            }
        }

        /// <summary>
        /// Read the name and value of a custom parameter from a line of the script.
        /// </summary>
        /// <param name="line">The line of the script.</param>
        /// <param name="name">The name of the custom parameter.</param>
        /// <param name="value">The value of the custom parameter.</param>
        /// <returns>True if the line contained an custom parameter; false otherwise.</returns>
        private static bool ReadCustomParameter(string line, out string name, out string value)
        {
            const string CUSTOM_PARAMETER_PREFIX = "---";
            line = line.TrimStart();

            if (!line.StartsWith(CUSTOM_PARAMETER_PREFIX))
            {
                name = null;
                value = null;
                return false;
            }

            line = line.Substring(CUSTOM_PARAMETER_PREFIX.Length).TrimStart();
            var index = line.IndexOf(':');
            if (index == -1)
            {
                name = null;
                value = null;
                return false;
            }

            name = line.Substring(0, index).TrimEnd().ToLowerInvariant();
            value = line.Substring(index + 1).Trim();
            return true;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Run the upgrade or create script and the test data script.
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step should be
        /// performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public override void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            RunSqlScript(connection, transaction, _scriptPath);

            if (_testDataScriptPath != null)
            {
                RunSqlScript(connection, transaction, _testDataScriptPath);
            }
        }

        /// <summary>
        /// Run an SQL script.
        /// </summary>
        /// <param name="connection">SQL connection the operation is to be performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        /// <param name="scriptPath">Path of the script to run.</param>
        private static void RunSqlScript(SqlConnection connection, SqlTransaction transaction, string scriptPath)
        {
            var lines = File.ReadAllLines(scriptPath);

            var start = 0;
            for (var current = 0; current < lines.Length; current++)
            {
                if (lines[current].Trim().ToLower() != "go") continue;
                if (start < current)
                {
                    var commandText = String.Join("\r\n", lines, start, current - start);
                    ExecuteCommand(connection, transaction, commandText);
                }

                start = current + 1;
            }

            if (start < lines.Length)
            {
                var commandText = String.Join("\r\n", lines, start, lines.Length - start);

                ExecuteCommand(connection, transaction, commandText);
            }
        }

        private static void ExecuteCommand(SqlConnection connection, SqlTransaction transaction, string commandText)
        {
            if (String.IsNullOrWhiteSpace(commandText)) return;

            using (var command = new SqlCommand(commandText, connection, transaction))
            {
                DeploymentTransaction.Current.LogSqlCommand(command);
                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
