using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SeemplesTools.Deployment.Commands;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.DatabaseUpgrade.Steps;

namespace SeemplesTools.Deployment.DatabaseUpgrade
{
    /// <summary>
    /// Graph containing possible steps groups to be executed while upgrading a single logical database by
    /// <see cref="CreateLogicalDatabasesCommand"/> with edges marking the pairs
    /// which may be executed immediately after each other.
    /// </summary>
    public class UpgradePathGraph
    {
        #region Properties

        /// <summary>
        /// Name of the database to be upgraded.
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// The nodes of the upgrade path graph.
        /// </summary>
        public List<UpgradePathNode> Nodes { get; private set; }

        /// <summary>
        /// The start node of the upgrade path graph.
        /// </summary>
        public UpgradePathNode StartNode { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Create an instance of the <see cref="UpgradePathGraph"/> class based on scripts in directories.
        /// </summary>
        /// <param name="databaseName">Name of the database to be upgraded.</param>
        /// <param name="sourceVersion">Installed version of the database or null if the database is not installed.
        /// </param>
        /// <param name="existingSchemaNames">The name of the schemas associated with the installed version of the logical
        /// database.</param>
        /// <param name="paths">Paths of the directories containing the upgrade scripts.</param>
        /// <param name="insertTestData">True if test data is to be inserted along with running the upgrade scripts.
        /// </param>
        public UpgradePathGraph(
            string databaseName,
            DatabaseVersion sourceVersion,
            IEnumerable<string> existingSchemaNames,
            IEnumerable<string> paths,
            bool insertTestData)
        {
            if (databaseName == null)
            {
                throw new ArgumentNullException("databaseName");
            }

            // ReSharper disable PossibleMultipleEnumeration
            if (paths == null || !paths.Any() || paths.Any(String.IsNullOrEmpty))
            {
                throw new ArgumentNullException("paths");
            }

            _databaseName = databaseName;

            var startStepGroup = new StepGroup(databaseName, sourceVersion)
            {
                SchemaNames = new HashSet<string>(existingSchemaNames, StringComparer.InvariantCultureIgnoreCase)
            };
            // ReSharper disable once ObjectCreationAsStatement
            new InitialStep(startStepGroup);
            StartNode = new UpgradePathNode(startStepGroup);

            Nodes = new List<UpgradePathNode> { StartNode };

            var nodesToProcess = new Stack<UpgradePathNode>();
            nodesToProcess.Push(StartNode);

            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Pop();

                IEnumerable<UpgradePathNode> nextNodes;

                if (currentNode.StepGroup.TargetVersion == null)
                {
                    nextNodes = GetCreateNodes(databaseName, paths, insertTestData);
                }
                else
                {
                    nextNodes = GetUpgradeNodes(
                        databaseName,
                        currentNode.StepGroup.TargetVersion,
                        currentNode.StepGroup.SchemaNames,
                        paths,
                        insertTestData);
                }

                foreach (var nextNode in nextNodes)
                {
                    Nodes.Add(nextNode);
                    currentNode.NextNodes.Add(nextNode);
                    nodesToProcess.Push(nextNode);
                }
            }
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Enumerate the nodes to be added to the upgrade path graph which create a new version of the database without a
        /// pre-existing version being present.
        /// </summary>
        /// <param name="databaseName">Name of the database to be upgraded.</param>
        /// <param name="paths">Paths of the directories containing the upgrade scripts.</param>
        /// <param name="insertTestData">True if test data is to be inserted along with running the upgrade scripts.</param>
        /// <returns>List of the nodes which create a new version of the database.</returns>
        private static IEnumerable<UpgradePathNode> GetCreateNodes(
            string databaseName,
            IEnumerable<string> paths,
            bool insertTestData)
        {
            var pattern = String.Format("{0}_*.sql", databaseName);

            var scriptPaths = paths.SelectMany(p => Directory.GetFiles(p, pattern));

            foreach (var scriptPath in scriptPaths)
            {
                var scriptName = Path.GetFileName(scriptPath);

                // ReSharper disable once PossibleNullReferenceException
                var scriptTargetVersionString = scriptName.Substring(
                    databaseName.Length + 1,
                    scriptName.Length - databaseName.Length - 5);

                if (scriptTargetVersionString.IndexOfAny(new[] { '-', '_' }) >= 0)
                    continue;

                var testDataScriptPath = scriptPath.Substring(0, scriptPath.Length - 4) + "_testdata.sql";

                if (insertTestData && !File.Exists(testDataScriptPath))
                {
                    throw new ApplicationException(String.Format(
                        "Missing test data insert script {0}.", Path.GetFileName(testDataScriptPath)));
                }

                DatabaseVersion scriptTargetVersion;
                if (!DatabaseVersion.TryParse(scriptTargetVersionString, out scriptTargetVersion))
                {
                    throw new ApplicationException(String.Format(
                        "Database script {0} has invalid target version.", scriptPath));
                }

                var stepGroup = new StepGroup(databaseName, scriptTargetVersion);
                // ReSharper disable once ObjectCreationAsStatement
                new RunScriptStep(
                    stepGroup,
                    null,
                    scriptPath,
                    insertTestData ? testDataScriptPath : null);

                var createdSchemas = new HashSet<string>(
                    stepGroup.SchemaNames,
                    StringComparer.InvariantCultureIgnoreCase);

                if (createdSchemas.Count > 0)
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new CreateSchemasStep(stepGroup, createdSchemas);
                }

                yield return new UpgradePathNode(stepGroup);
            }
        }

        /// <summary>
        /// Enumerate the nodes to be added to the upgrade path graph which upgrade a specific version of the database.
        /// </summary>
        /// <param name="databaseName">Name of the database to be upgraded.</param>
        /// <param name="sourceVersion">Installed version of the database or null if the database is not installed.
        /// </param>
        /// <param name="existingSchemaNames">The name of the schemas associated with the previous version of the logical
        /// database.</param>
        /// <param name="paths">Paths of the directories containing the upgrade scripts.</param>
        /// <param name="insertTestData">True if test data is to be inserted along with running the upgrade scripts.</param>
        /// <returns>List of the nodes which upgrade the specified version of the database to a new version.</returns>
        private static IEnumerable<UpgradePathNode> GetUpgradeNodes(
            string databaseName,
            DatabaseVersion sourceVersion,
            IEnumerable<string> existingSchemaNames,
            IEnumerable<string> paths,
            bool insertTestData)
        {
            var pattern = String.Format("{0}_*.sql", databaseName);

            var scriptPaths = paths.SelectMany(p => Directory.GetFiles(p, pattern));

            foreach (var scriptPath in scriptPaths)
            {
                var scriptName = Path.GetFileName(scriptPath);

                // ReSharper disable once PossibleNullReferenceException
                var versionsString = scriptName.Substring(
                    databaseName.Length + 1,
                    scriptName.Length - databaseName.Length - 5);

                if (versionsString.Contains("_"))
                    continue;

                var index = versionsString.IndexOf('-');
                if (index == -1) continue;

                var scriptSourceVersionString = versionsString.Substring(0, index);
                var scriptTargetVersionString = versionsString.Substring(index + 1);

                if (scriptTargetVersionString.Contains("-")) continue;

                if (scriptPath.ToLowerInvariant().EndsWith("_localization.sql")) continue;

                DatabaseVersion scriptSourceVersion;
                if (!DatabaseVersion.TryParse(scriptSourceVersionString, out scriptSourceVersion))
                {
                    throw new ApplicationException(String.Format(
                        "Database script {0} has invalid source version.", scriptPath));
                }

                if (scriptSourceVersion != sourceVersion) continue;

                var testDataScriptPath = scriptPath.Substring(0, scriptPath.Length - 4) + "_testdata.sql";

                if (insertTestData && !File.Exists(testDataScriptPath))
                {
                    throw new ApplicationException(String.Format(
                        "Missing test data insert script {0}.", Path.GetFileName(testDataScriptPath)));
                }

                DatabaseVersion scriptTargetVersion;
                if (!DatabaseVersion.TryParse(scriptTargetVersionString, out scriptTargetVersion))
                {
                    throw new ApplicationException(String.Format(
                        "Database script {0} has invalid target version.", scriptPath));
                }

                var stepGroup = new StepGroup(databaseName, scriptTargetVersion);
                // ReSharper disable once ObjectCreationAsStatement
                new RunScriptStep(
                    stepGroup,
                    sourceVersion,
                    scriptPath,
                    insertTestData ? testDataScriptPath : null);

                // ReSharper disable PossibleMultipleEnumeration
                var createdSchemas = new HashSet<string>(
                    stepGroup.SchemaNames.Except(existingSchemaNames),
                    StringComparer.InvariantCultureIgnoreCase);
                var deletedSchemas = new HashSet<string>(
                    existingSchemaNames
                        .Except(stepGroup.SchemaNames)
                        .Except(new[] { "dbo" }),
                    StringComparer.InvariantCultureIgnoreCase);
                // ReSharper restore PossibleMultipleEnumeration

                if (createdSchemas.Count > 0)
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new CreateSchemasStep(stepGroup, createdSchemas);
                }

                if (deletedSchemas.Count > 0)
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new DropSchemasStep(stepGroup, deletedSchemas);
                }

                yield return new UpgradePathNode(stepGroup);
            }
        }

        #endregion

        #region Finding the optimal upgrade path

        /// <summary>
        /// Find the optimal upgrade path from the start node to the specified version.
        /// </summary>
        /// <param name="targetVersion">The target version to upgrade to.</param>
        /// <returns>The <see cref="UpgradePathNode"/> of the last step of the optimal upgrade path.</returns>
        /// <remarks>
        /// This method may only be run once.
        /// 
        /// After running this method, the entire path can be determined by stepping backwards from the last node through
        /// the <see cref="UpgradePathNode.PreviousNode"/> property.
        /// </remarks>
        public UpgradePathNode FindUpgradePath(DatabaseVersion targetVersion)
        {
            if (targetVersion == null)
            {
                throw new ArgumentNullException("targetVersion");
            }

            var nodesToProcess = new Queue<UpgradePathNode>();
            var nodesToProcessInNextIteration = new Queue<UpgradePathNode>();

            nodesToProcess.Enqueue(StartNode);

            for (var iteration = 1; ; iteration++)
            {
                UpgradePathNode currentNode = null;

                while (true)
                {
                    if (currentNode == null)
                    {
                        if (nodesToProcess.Count == 0)
                            break;

                        currentNode = nodesToProcess.Dequeue();
                    }

                    if (currentNode.IterationNumber < iteration)
                    {
                        currentNode = null;
                        continue;
                    }

                    if (currentNode.StepGroup.TargetVersion == targetVersion)
                        return currentNode;

                    var crossBranchNodes = new List<UpgradePathNode>();
                    var nonCrossBranchNodes = new List<UpgradePathNode>();

                    foreach (var nextNode in currentNode.NextNodes)
                    {
                        var stepGroup = nextNode.StepGroup;

                        if (stepGroup.CrossBranch)
                        {
                            if (nextNode.IterationNumber <= iteration + 1) continue;
                            nextNode.IterationNumber = iteration + 1;
                            nextNode.PreviousNode = currentNode;
                            crossBranchNodes.Add(nextNode);
                        }
                        else
                        {
                            if (nextNode.IterationNumber <= iteration) continue;
                            nextNode.IterationNumber = iteration;
                            nextNode.PreviousNode = currentNode;
                            nonCrossBranchNodes.Add(nextNode);
                        }
                    }

                    crossBranchNodes.Sort();
                    for (var i = crossBranchNodes.Count - 1; i >= 0; i--)
                        nodesToProcessInNextIteration.Enqueue(crossBranchNodes[i]);

                    nonCrossBranchNodes.Sort();
                    for (var i = nonCrossBranchNodes.Count - 2; i >= 0; i--)
                        nodesToProcess.Enqueue(nonCrossBranchNodes[i]);

                    currentNode = nonCrossBranchNodes.Count > 0 
                        ? nonCrossBranchNodes[nonCrossBranchNodes.Count - 1] 
                        : null;
                }

                if (nodesToProcessInNextIteration.Count == 0) break;

                nodesToProcess = nodesToProcessInNextIteration;
                nodesToProcessInNextIteration = new Queue<UpgradePathNode>();
            }

            throw new ApplicationException(String.Format(
                "Cannot upgrade logical database {0} to version {1} because the necessary upgrade scripts don't exist.",
                _databaseName, targetVersion));
        }

        #endregion
    }
}
