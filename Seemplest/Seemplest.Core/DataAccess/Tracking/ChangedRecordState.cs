namespace Seemplest.Core.DataAccess.Tracking
{
    /// <summary>
    /// This enumeration describes the state of a changed record
    /// </summary>
    public enum ChangedRecordState
    {
        /// <summary>
        /// The record has been read from the database and attached to the tracking context.
        /// </summary>
        Attached,

        /// <summary>
        /// The record has been inserted to the database.
        /// </summary>
        Inserted,

        /// <summary>
        /// The record has been updated while it's been attached to the tracking context.
        /// </summary>
        Updated,

        /// <summary>
        /// The record has been deleted.
        /// </summary>
        Deleted
    }
}