using System;
using Seemplest.Core.Tracing;

namespace SeemplestBlocks.Core.Diagnostics
{
    /// <summary>
    /// Ez az osztály felelős a rendszerben található események fájlba történő naplózásáért
    /// </summary>
    public static class Tracer
    {
        private static TraceLogItemType s_LogType;

        /// <summary>
        /// Inicializálja a naplózó komponenst
        /// </summary>
        public static void Start()
        {
            var logFolder = TracerConfiguration.LogFolder;
            var logFile = TracerConfiguration.LogFile;
            var folderPattern = TracerConfiguration.FolderPattern;
            var fileSuffixPattern = TracerConfiguration.FileSuffixPattern;
            LoggerInstance = new FileTraceLogger(
                logFile,
                logFolder,
                folderPattern,
                fileNameSuffixPattern: fileSuffixPattern,
                flushAfter: 1);
            s_LogType = TracerConfiguration.LogLevel;
        }

        /// <summary>
        /// A komponenshez tartozó naplózó objektum
        /// </summary>
        public static FileTraceLogger LoggerInstance { get; private set; }

        public static void Stop()
        {
            if (LoggerInstance != null) LoggerInstance.Dispose();
            LoggerInstance = null;
        }

        /// <summary>
        /// Naplózza az adott bejegyzést
        /// </summary>
        /// <param name="item">Naplóbejegyzés</param>
        public static void Log(TraceLogItem item)
        {
            try
            {
                if (LoggerInstance == null || (int)item.Type < (int)s_LogType) return;

                // --- Az üzenetben lévő sorvégeket lecseréljük
                if (item.Message != null)
                {
                    item.Message = item.Message.Replace("\r\n", "\n");
                    item.Message = item.Message.Replace("\n", "\r\n\t\t\t\t\t");
                }
                if (item.DetailedMessage != null)
                {
                    item.DetailedMessage = item.DetailedMessage.Replace("\r\n", "\n");
                    item.DetailedMessage = item.DetailedMessage.Replace("\n", "\r\n\t\t\t\t\t\t");
                }

                // --- Naplózzuk az üzenetet
                LoggerInstance.Log(item);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // --- A naplózásban levő kivételt lenyeljük                
            }
        }

        public static void Log(TraceLogItemType type, string operationType, string message, string detailedMessage = null)
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
        public static void LogInfo(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Informational, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a success entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public static void LogSuccess(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Success, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a warning entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public static void LogWarning(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Warning, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs an error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public static void LogError(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Error, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a fatal error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public static void LogFatalError(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceLogItemType.Fatal, operationType, message, detailedMessage);
        }


    }
}