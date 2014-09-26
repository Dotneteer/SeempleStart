namespace SeemplesTools.Deployment.DatabaseUpgrade
{
    /// <summary>
    /// Type of dependency between an upgrade script and another database.
    /// </summary>
    public enum DependencyType
    {
        /// <summary>
        /// The target database must be present and upgraded to the specified version range before this script is run.
        /// </summary>
        /// <remarks>
        /// This script is guaranteed to run after the target database is upgraded to the specified version range. There's
        /// no guarantee the target database is not upgraded to a later version before this script is run. To do that, a
        /// reverse dependency needs to be added as well.
        /// </remarks>
        Hard,
        /// <summary>
        /// If the target database is present then it must be upgraded to the specified version range before this script
        /// is run.
        /// </summary>
        /// <remarks>
        /// This script is guaranteed to run after the target database is upgraded to the specified version range. There's
        /// no guarantee the target database is not upgraded to a later version before this script is run. To do that, a
        /// reverse dependency needs to be added as well.
        /// </remarks>
        Optional,
        /// <summary>
        /// If the target database is present and its upgrade path contains a version within the specified version range
        /// then this script must be run after the target database is upgraded to the specified version range.
        /// </summary>
        /// <remarks>
        /// This script is guaranteed to run after the target database is upgraded to the specified version range. There's
        /// no guarantee the target database is not upgraded to a later version before this script is run. To do that, a
        /// reverse dependency needs to be added as well.
        /// </remarks>
        Soft,
        /// <summary>
        /// The target database must be present and upgraded to the specified version range after this script is run.
        /// </summary>
        ReverseHard,
        /// <summary>
        /// If the target database is present then it must be upgraded to the specified version range after this script is
        // run.
        /// </summary>
        ReverseOptional,
        /// <summary>
        /// If the target database is present and its upgrade path contains a version within the specified version range
        /// then this script must be run before the target database is upgraded to the specified version range.
        /// </summary>
        ReverseSoft
    }
}
