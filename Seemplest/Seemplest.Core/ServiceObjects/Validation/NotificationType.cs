namespace Seemplest.Core.ServiceObjects.Validation
{
    /// <summary>
    /// This enum defines the types of notifications that can be retrieved from a business operation.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Informational notification
        /// </summary>
        Info = 0,

        /// <summary>
        /// Trace notification
        /// </summary>
        Trace = 1,

        /// <summary>
        /// Warning notification
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error notification
        /// </summary>
        Error = 3,

        /// <summary>
        /// Fatal error notification
        /// </summary>
        FatalError = 4,
    }
}