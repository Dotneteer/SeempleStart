namespace Seemplest.Core.Tracing
{
    /// <summary>
    /// This interface describes operation to trace the flow of operations
    /// </summary>
    public interface ITraceLogger
    {
        /// <summary>
        /// Logs the specified trace entry
        /// </summary>
        /// <param name="item">Trace entry</param>
        void Log(TraceLogItem item);

        /// <summary>
        /// Logs an entry with the specified type, operation type, message, and details.
        /// </summary>
        /// <param name="type">Long entry type</param>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        void Log(TraceLogItemType type, string operationType, string message, string detailedMessage = null);

        /// <summary>
        /// Logs an informational entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        void LogInfo(string operationType, string message, string detailedMessage = null);

        /// <summary>
        /// Logs a success entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        void LogSuccess(string operationType, string message, string detailedMessage = null);

        /// <summary>
        /// Logs a warning entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        void LogWarning(string operationType, string message, string detailedMessage = null);

        /// <summary>
        /// Logs an error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        void LogError(string operationType, string message, string detailedMessage = null);

        /// <summary>
        /// Logs a fatal error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        void LogFatalError(string operationType, string message, string detailedMessage = null);
    }
}