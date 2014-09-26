namespace Seemplest.Core.Tracing
{
    /// <summary>
    /// This enumeration defines the types of diagnostics log items
    /// </summary>
    public enum TraceLogItemType
    {
        /// <summary>Empty item type</summary>
        Undefined = 0,

        /// <summary>Informational message</summary>
        Informational = 1,

        /// <summary>Success message</summary>
        Success = 2,

        /// <summary>Warning message</summary>
        Warning = 3,

        /// <summary>Error message</summary>
        Error = 4,

        /// <summary>Fatal error message</summary>
        Fatal = 5
    }
}