namespace Seemplest.Core.DataAccess
{
    /// <summary>
    /// This enum declares the mode the database operates in
    /// </summary>
    public enum SqlOperationMode
    {
        /// <summary>The database can be used with read and write operations.</summary>
        ReadWrite = 0,

        /// <summary>The database can be used only with read operations.</summary>
        ReadOnly,

        /// <summary>
        /// The database can be used with read and write operations, and all 
        /// operations must be tracked.
        /// </summary>
        Tracked
    }
}