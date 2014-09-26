using System;
using System.Collections.Generic;

namespace SeemplesTools.Deployment.DatabaseUpgrade
{
    /// <summary>
    /// One node of a <see cref="DependencyGraph"/>.
    /// </summary>
    public class DependencyNode
    {
        #region Properties

        /// <summary>
        /// The upgrade step group represented by this upgrade path node.
        /// </summary>
        public StepGroup StepGroup { get; private set; }

        /// <summary>
        /// List of other <see cref="DependencyNode"/>s this node depends on.
        /// </summary>
        public List<DependencyNode> Dependencies { get; private set; }

        /// <summary>
        /// List of other <see cref="DependencyNode"/>s depending on this node.
        /// </summary>
        /// <remarks>
        /// This property is only set after <see cref="DependencyGraph.BuildReverseDependencies"/> has been called on the
        /// <see cref="DependencyGraph"/> containing this node.
        /// </remarks>
        public List<DependencyNode> ReverseDependencies { get; private set; }

        /// <summary>
        /// The next <see cref="DependencyNode"/> in a cycle.
        /// </summary>
        /// <remarks>
        /// This property is set by <see cref="DependencyGraph.CycleDetected"/> to find a cycle when one is known to
        /// exist.
        /// </remarks>
        public DependencyNode NextNode { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates an instance of the <see cref="DependencyNode"/> class.
        /// </summary>
        /// <param name="stepGroup">The upgrade step group represented by this dependency node.</param>
        public DependencyNode(StepGroup stepGroup)
        {
            if (stepGroup == null)
            {
                throw new ArgumentNullException("stepGroup");
            }

            StepGroup = stepGroup;
            Dependencies = new List<DependencyNode>();
            ReverseDependencies = new List<DependencyNode>();
        }

        #endregion

        #region Debugging

        /// <summary>
        /// Returns a human-readable description of the step group represented by this upgrade node.
        /// </summary>
        /// <returns>A human-readable description of the step group represented by this upgrade node.</returns>
        public override string ToString()
        {
            return StepGroup.ToString();
        }

        #endregion
    }
}
