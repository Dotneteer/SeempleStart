using System;
using System.Collections.Generic;
using System.Linq;
using SeemplesTools.Deployment.Commands;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.DatabaseUpgrade.Steps;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.DatabaseUpgrade
{
    /// <summary>
    /// Graph containing steps to be executed during
    /// <see cref="CreateLogicalDatabasesCommand"/> with edges marking the dependencies
    /// between them.
    /// </summary>
    public class DependencyGraph
    {
        #region Private types

        /// <summary>
        /// A pair of a database version and a <see cref="DependencyNode"/>.
        /// </summary>
        private class VersionNodePair : IComparable<VersionNodePair>
        {
            /// <summary>
            /// The database version.
            /// </summary>
            private readonly DatabaseVersion _version;

            /// <summary>
            /// The database version.
            /// </summary>
            public DatabaseVersion Version
            {
                get { return _version; }
            }

            /// <summary>
            /// The <see cref="DependencyNode"/> corresponding to the version.
            /// </summary>
            private readonly DependencyNode _node;

            /// <summary>
            /// The <see cref="DependencyNode"/> corresponding to the version.
            /// </summary>
            public DependencyNode Node
            {
                get { return _node; }
            }

            /// <summary>
            /// Creates an instance of the <see cref="VersionNodePair"/> class.
            /// </summary>
            /// <param name="version">The database version.</param>
            /// <param name="node">The <see cref="DependencyNode"/> corresponding to the database version.</param>
            public VersionNodePair(DatabaseVersion version, DependencyNode node)
            {
                if (version == null)
                {
                    throw new ArgumentNullException("version");
                }

                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }

                _version = version;
                _node = node;
            }

            /// <summary>
            /// Compare this object to another <see cref="VersionNodePair"/> based on version.
            /// </summary>
            /// <param name="other">The <see cref="VersionNodePair"/> to compare this object to.</param>
            /// <returns>1 if this object has a higher target version than other; -1 if this object has a lower target
            /// version; 0 if the versions are equal.</returns>
            public int CompareTo(VersionNodePair other)
            {
                return other == null 
                    ? 1 
                    : DatabaseVersion.Compare(Version, other.Version);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The nodes of the dependency graph.
        /// </summary>
        public List<DependencyNode> Nodes { get; private set; }

        /// <summary>
        /// Lookup table for fast retrieval of the latest version of a database in a specified version range.
        /// </summary>
        /// <remarks>
        /// The keys of the dictionary are database names.
        /// 
        /// The values of the dictionary are ascending lists of <see cref="VersionNodePair"/> objects for the
        /// specified database.
        /// </remarks>
        private readonly Dictionary<string, List<VersionNodePair>> _databaseVersionLookupTable;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize an empty instance of the <see cref="DependencyGraph"/> class.
        /// </summary>
        public DependencyGraph()
        {
            Nodes = new List<DependencyNode>();
            _databaseVersionLookupTable =
                new Dictionary<string, List<VersionNodePair>>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Build upgrade path for one database

        /// <summary>
        /// Find the optimal upgrade path between two versions of a logical database based on scripts in a directory and
        /// add its upgrade steps to the dependency graph.
        /// </summary>
        /// <param name="databaseName">Name of the logical database to be upgraded.</param>
        /// <param name="sourceVersion">Installed version of the logical database or null if the database is not
        /// installed.</param>
        /// <param name="targetVersion">Target version of the logical database.</param>
        /// <param name="existingSchemaNames">The name of the schemas associated with the installed version of the logical
        /// database.</param>
        /// <param name="paths">Paths of the directories containing the upgrade scripts.</param>
        /// <param name="serviceUsers">Name of the users to grant rights to on the logical database.</param>
        /// <param name="insertTestData">True if test data is to be inserted along with the database upgrade.</param>
        /// <remarks>
        /// This method may not be called after a call to <see cref="GetTopologicalOrder"/>.
        /// </remarks>
        public void BuildUpgradePath(
            string databaseName,
            DatabaseVersion sourceVersion,
            DatabaseVersion targetVersion,
            IEnumerable<string> existingSchemaNames,
            IEnumerable<string> paths,
            IEnumerable<string> serviceUsers,
            bool insertTestData)
        {
            if (databaseName == null)
            {
                throw new ArgumentNullException("databaseName");
            }

            if (targetVersion == null)
            {
                throw new ArgumentNullException("targetVersion");
            }

            // ReSharper disable PossibleMultipleEnumeration
            if (paths == null || !paths.Any() || paths.Any(String.IsNullOrEmpty))
            {
                throw new ArgumentNullException("paths");
            }

            var upgradePathGraph = new UpgradePathGraph(
                databaseName,
                sourceVersion,
                existingSchemaNames,
                paths,
                insertTestData);
            var lastUpgradePathNode = upgradePathGraph.FindUpgradePath(targetVersion);
            var lastUpgradeNode = AddUpgradePathToDependencyGraph(lastUpgradePathNode, databaseName);

            var finalStepGroup = new StepGroup(databaseName, targetVersion)
            {
                SchemaNames = new HashSet<string>(
                    lastUpgradeNode.StepGroup.SchemaNames,
                    StringComparer.InvariantCultureIgnoreCase)
            };

            foreach (var serviceUser in serviceUsers)
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GrantRightsStep(finalStepGroup, serviceUser);
            }

            // ReSharper disable once ObjectCreationAsStatement
            new SetMetadataStep(finalStepGroup);

            var finalNode = new DependencyNode(finalStepGroup);
            Nodes.Add(finalNode);
            finalNode.Dependencies.Add(lastUpgradeNode);
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Add the steps of an upgrade path to the dependency graph.
        /// </summary>
        /// <param name="endUpgradePathNode">The <see cref="UpgradePathNode"/> of the last step of the upgrade path.
        /// </param>
        /// <param name="databaseName">The name of the database to be upgraded.</param>
        /// <returns>The <see cref="DependencyNode"/> of the last step of the upgrade path.</returns>
        private DependencyNode AddUpgradePathToDependencyGraph(UpgradePathNode endUpgradePathNode, string databaseName)
        {
            var versions = new Stack<string>();

            DependencyNode lastDependencyNode = null;
            DependencyNode nextDependencyNode = null;

            var versionNodePairs = new List<VersionNodePair>();

            var currentUpgradePathNode = endUpgradePathNode;
            while (currentUpgradePathNode != null)
            {
                var currentNodeVersions = currentUpgradePathNode.StepGroup.GetShortDescriptions();

                foreach (var version in currentNodeVersions)
                {
                    versions.Push(version);
                }

                var currentDependencyNode = new DependencyNode(currentUpgradePathNode.StepGroup);

                if (lastDependencyNode == null)
                {
                    lastDependencyNode = currentDependencyNode;
                }

                Nodes.Add(currentDependencyNode);

                if (currentDependencyNode.StepGroup.TargetVersion != null)
                {
                    versionNodePairs.Add(
                        new VersionNodePair(currentDependencyNode.StepGroup.TargetVersion, currentDependencyNode));
                }

                if (nextDependencyNode != null)
                {
                    nextDependencyNode.Dependencies.Add(currentDependencyNode);
                }

                nextDependencyNode = currentDependencyNode;
                currentUpgradePathNode = currentUpgradePathNode.PreviousNode;
            }

            DeploymentTransaction.Current.Log("Upgrade path for {0}: {1}", databaseName, String.Join(" -> ", versions));

            versionNodePairs.Sort();
            _databaseVersionLookupTable[databaseName] = versionNodePairs;

            return lastDependencyNode;
        }

        #endregion

        #region Topological sort

        /// <summary>
        /// Marks the inter-database dependencies in the graph and returns a topological order.
        /// </summary>
        /// <returns>Topological ordering of the step groups required to perform all queued upgrade operations.</returns>
        /// <remarks>
        /// This method may only be called once.
        /// </remarks>
        public List<StepGroup> GetTopologicalOrder()
        {
            ResolveDependencies();
            BuildReverseDependencies();
            return BuildTopologicalOrder();
        }

        /// <summary>
        /// Create edges between the nodes according to the dependencies marked in the upgrade scripts.
        /// </summary>
        private void ResolveDependencies()
        {
            foreach (var node in Nodes)
            {
                var stepGroup = node.StepGroup;

                foreach (var dependency in stepGroup.UnresolvedDependencies)
                {
                    if (!_databaseVersionLookupTable.ContainsKey(dependency.TargetDatabaseName))
                    {
                        if (dependency.Type == DependencyType.Hard || dependency.Type == DependencyType.ReverseHard)
                        {
                            throw new ApplicationException(String.Format(
                                "Cannot satisfy dependencies in upgrade scripts: the step [{0}] requires database {1}, " +
                                "version {2}-{3} which is not present in the upgrade path.",
                                stepGroup.Description,
                                dependency.TargetDatabaseName,
                                dependency.MinVersion == DatabaseVersion.MinValue
                                    ? "*"
                                    : dependency.MinVersion.ToString(),
                                dependency.MaxVersion == DatabaseVersion.MaxValue
                                    ? "*"
                                    : dependency.MaxVersion.ToString()));
                        }
                        continue;
                    }

                    var dependencyNode = GetNode(
                        _databaseVersionLookupTable[dependency.TargetDatabaseName],
                        dependency.MinVersion,
                        dependency.MaxVersion);

                    if (dependencyNode == null)
                    {
                        if (dependency.Type != DependencyType.Soft && dependency.Type != DependencyType.ReverseSoft)
                        {
                            throw new ApplicationException(String.Format(
                                "Cannot satisfy dependencies in upgrade scripts: the step [{0}] requires database {1}, " +
                                "version {2}-{3} which is not present in the upgrade path.",
                                stepGroup.Description,
                                dependency.TargetDatabaseName,
                                dependency.MinVersion == DatabaseVersion.MinValue
                                    ? "*"
                                    : dependency.MinVersion.ToString(),
                                dependency.MaxVersion == DatabaseVersion.MaxValue
                                    ? "*"
                                    : dependency.MaxVersion.ToString()));
                        }

                        continue;
                    }

                    if (dependency.Type == DependencyType.Hard ||
                        dependency.Type == DependencyType.Optional ||
                        dependency.Type == DependencyType.Soft)
                    {
                        node.Dependencies.Add(dependencyNode);
                    }
                    else
                    {
                        dependencyNode.Dependencies.Add(node);
                    }
                }
            }
        }

        /// <summary>
        /// Find the range of nodes corresponding to the latest version within a specified range in an ordered list.
        /// </summary>
        /// <param name="list">Ordered list of versions.</param>
        /// <param name="minVersion">Lowest allowed version.</param>
        /// <param name="maxVersion">Highest allowed version.</param>
        /// <returns>The range of nodes corresponding to the highest version in the list within the specified range.
        /// </returns>
        private static DependencyNode GetNode(
            IReadOnlyList<VersionNodePair> list,
            DatabaseVersion minVersion,
            DatabaseVersion maxVersion)
        {
            var minBound = 0;
            var maxBound = list.Count - 1;

            while (minBound <= maxBound)
            {
                var middle = minBound + ((maxBound - minBound) >> 1);

                var difference = list[middle].Version.CompareTo(maxVersion);

                if (difference == 0) return list[middle].Node;
                if (difference > 0)
                {
                    maxBound = middle - 1;
                }
                else
                {
                    minBound = middle + 1;
                }
            }

            if (maxBound < 0)
                return null;

            return list[maxBound].Version >= minVersion 
                ? list[maxBound].Node 
                : null;
        }

        /// <summary>
        /// Populate the <see cref="DependencyNode.ReverseDependencies"/> properties of the nodes.
        /// </summary>
        private void BuildReverseDependencies()
        {
            foreach (var currentNode in Nodes)
            {
                foreach (var dependencyNode in currentNode.Dependencies)
                {
                    dependencyNode.ReverseDependencies.Add(currentNode);
                }
            }
        }

        /// <summary>
        /// Sort the nodes in topological order and return their step groups in a list. May only be called after
        /// <see cref="BuildReverseDependencies"/>.
        /// </summary>
        /// <returns>The step groups of the nodes of the dependency graph in topological order.</returns>
        private List<StepGroup> BuildTopologicalOrder()
        {
            var startingNodes = Nodes.Where(node => node.Dependencies.Count == 0).ToList();
            startingNodes.Reverse();

            var nodesToProcess = new Stack<DependencyNode>(startingNodes);
            var result = new List<StepGroup>();
            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Pop();

                result.Add(currentNode.StepGroup);

                foreach (var nextNode in currentNode.ReverseDependencies)
                {
                    nextNode.Dependencies.Remove(currentNode);
                    if (nextNode.Dependencies.Count == 0)
                        nodesToProcess.Push(nextNode);
                }
            }

            CheckRemainingEdges();

            DeploymentTransaction.Current.Log("Topological order of upgrade steps:");

            foreach (var description in result.SelectMany(stepGroup => stepGroup.GetLongDescriptions()))
            {
                DeploymentTransaction.Current.Log("  " + description);
            }

            DeploymentTransaction.Current.Log("End of topological order listing.");

            return result;
        }

        /// <summary>
        /// Check to see if there are nodes with dependencies left after building the topological order. This means that
        /// the dependency graph has a cycle. In this case, <see cref="CycleDetected"/> is called to log a cycle and throw
        /// an exception.
        /// </summary>
        private void CheckRemainingEdges()
        {
            foreach (var node in Nodes.Where(node => node.Dependencies.Count != 0))
            {
                CycleDetected(node);
            }
        }

        /// <summary>
        /// Called when the dependency graph contains a cycle. Log the nodes forming the cycle and throw an exception.
        /// </summary>
        /// <param name="startingNode">One node which is the member of a cycle.</param>
        private static void CycleDetected(DependencyNode startingNode)
        {
            var currentNode = startingNode;

            while (currentNode.NextNode == null)
            {
                if (currentNode.Dependencies.Count == 0)
                {
                    throw new InvalidOperationException(String.Format(
                        "Internal error: topological ordering didn't process node: {0}.",
                        currentNode.StepGroup.Description));
                }

                currentNode.NextNode = currentNode.Dependencies[0];
                currentNode = currentNode.NextNode;
            }

            DeploymentTransaction.Current.Log("Cycle detected in dependency graph:");

            var firstNodeOfCycle = currentNode;

            do
            {
                DeploymentTransaction.Current.Log("  " + currentNode.StepGroup.Description);
                currentNode = currentNode.NextNode;
            } while (currentNode != firstNodeOfCycle);

            DeploymentTransaction.Current.Log("End of dependency cycle listing.");

            throw new ApplicationException("Dependency cycle detected in upgrade scripts.");
        }

        #endregion
    }
}
