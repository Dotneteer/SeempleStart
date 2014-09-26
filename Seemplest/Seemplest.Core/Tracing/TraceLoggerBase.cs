namespace Seemplest.Core.Tracing
{
    /// <summary>
    /// This class is intended to be the base class of all trace loggers.
    /// </summary>
    public abstract class TraceLoggerBase : ITraceLogger
    {
        /// <summary>
        /// Logs the specified trace entry
        /// </summary>
        /// <param name="item">Trace entry</param>
        public void Log(TraceLogItem item)
        {
            item.EnsureProperties();
            DoLog(item);
        }

        /// <summary>
        /// Override to specify how the trace entry should be logged.
        /// </summary>
        /// <param name="item">Trace entry</param>
        protected abstract void DoLog(TraceLogItem item);

        public void Log(TraceLogItemType type, string operationType, string message, string detailedMessage = null)
        {
            var item = new TraceLogItem
                {
                    Type = type,
                    OperationType = operationType,
                    Message = message,
                    DetailedMessage = detailedMessage
                };
            Log(item);
        }

        /// <summary>
        /// Logs an informational entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogInfo(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Informational, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a success entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogSuccess(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Success, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a warning entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogWarning(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Warning, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs an error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogError(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Error, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a fatal error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogFatalError(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Fatal, operationType, message, detailedMessage);
        }
    }
}