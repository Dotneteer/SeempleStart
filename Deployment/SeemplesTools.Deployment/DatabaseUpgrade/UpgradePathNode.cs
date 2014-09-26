using System;
using System.Collections.Generic;
using SeemplesTools.Deployment.Common;

namespace SeemplesTools.Deployment.DatabaseUpgrade
{
    /// <summary>
    /// One node of an <see cref="UpgradePathGraph"/>.
    /// </summary>
    public class UpgradePathNode : IComparable<UpgradePathNode>
    {
        #region Properties

        /// <summary>
        /// The upgrade step group represented by this upgrade path node.
        /// </summary>
        public StepGroup StepGroup { get; private set; }

        /// <summary>
        /// List of upgrade path nodes which can be executed right after this one.
        /// </summary>
        public List<UpgradePathNode> NextNodes { get; private set; }

        /// <summary>
        /// The previous node in an upgrade path.
        /// </summary>
        /// <remarks>
        /// This property is only set if the <see cref="UpgradePathGraph.FindUpgradePath"/> method has been called on the
        /// <see cref="UpgradePathGraph"/> containing this node.
        /// 
        /// This property is only meaningful on nodes in the optimal upgrade path; that is, on the node returned by
        /// <see cref="UpgradePathGraph.FindUpgradePath"/> and on nodes that can be reached from that node through this
        /// property recursively.
        /// </remarks>
        public UpgradePathNode PreviousNode { get; set; }

        /// <summary>
        /// Set to the number of the iteration in which <see cref="UpgradePathGraph.FindUpgradePath"/> has
        /// visited this node. This property is used to handle directed cycles in the
        /// upgrade path graph.
        /// </summary>
        public int IterationNumber { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates an instance of the <see cref="UpgradePathNode"/> class.
        /// </summary>
        /// <param name="stepGroup">The upgrade step group represented by this upgrade path node.</param>
        public UpgradePathNode(StepGroup stepGroup)
        {
            if (stepGroup == null)
            {
                throw new ArgumentNullException("stepGroup");
            }

            StepGroup = stepGroup;
            NextNodes = new List<UpgradePathNode>();
            IterationNumber = Int32.MaxValue;
        }

        #endregion

        #region IComparable<UpgradePathNode>

        /// <summary>
        /// Compare this object to another <see cref="UpgradePathNode"/> based on the target version of
        /// the each one's step.
        /// </summary>
        /// <param name="other">The <see cref="UpgradePathNode"/> to compare this object to.</param>
        /// <returns>1 if this step has a higher target version than other; -1 if this step has a lower
        /// target version; 0 if the target versions are equal.</returns>
        public int CompareTo(UpgradePathNode other)
        {
            return other == null 
                ? 1 
                : DatabaseVersion.Compare(StepGroup.TargetVersion, other.StepGroup.TargetVersion);
        }

        #endregion

        #region Debugging

        /// <summary>
        /// Returns a human-readable description of this node.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StepGroup.ToString();
        }

        #endregion
    }
}
