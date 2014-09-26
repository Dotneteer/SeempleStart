using System;
using System.Diagnostics;
using System.Text;
using Seemplest.Core.Common;
using Seemplest.Core.Tracing;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// This class can be used to write events to the windows event log.
    /// </summary>
    /// <remarks>
    /// The generic parameter must be the derived class of <see cref="LogEventBase"/>
    /// </remarks>
    public static class WindowsEventLogger
    {
        private const int MAX_MESSAGE_LENGTH = 31800;

        /// <summary>
        /// Default log name that is mapped to the Application log
        /// </summary>
        public const string DEFAULT_APP_LOG = "$default";

        /// <summary>
        /// Mapper for Event Log names
        /// </summary>
        public static INameMapper LogNameMapper { get; set; }

        /// <summary>
        /// Mapper for Event Source names
        /// </summary>
        public static INameMapper LogSourceMapper { get; set; }

        /// <summary>
        /// The alternate logger to be used instead of the Windows Event Log
        /// </summary>
        private static ITraceLogger TraceLogger { get; set; }

        /// <summary>
        /// Sets up the static members of this class
        /// </summary>
        static WindowsEventLogger()
        {
            LogNameMapper = new DefaultLogNameMapper();
            LogSourceMapper = new DefaultLogSourceNameMapper();
        }

        /// <summary>
        /// Redirects Windows Event Log entries to the specified logger.
        /// </summary>
        /// <param name="logger"></param>
        public static void RedirectLogTo(ITraceLogger logger)
        {
            TraceLogger = logger;
        }

        /// <summary>
        /// Redirects logging to the Windows Event Log.
        /// </summary>
        public static void Reset()
        {
            TraceLogger = null;
        }

        /// <summary>
        /// Logs the message into the EventLog defined by <typeparamref name="TEventDefinition"/>
        /// </summary>
        /// <typeparam name="TEventDefinition">
        /// Must be a derived class of <see cref="LogEventBase"/>
        /// </typeparam>
        /// <param name="message">The string message to log</param>
        /// <param name="messageParams">The parameters of the message</param>
        public static void Log<TEventDefinition>(string message = null, params object[] messageParams)
            where TEventDefinition : LogEventBase, new()
        {
            // --- Prepare the message to log
            var eventClass = new TEventDefinition();
            if (!eventClass.LogNameValid)
            {
                throw new InvalidOperationException("The event does not have a valid log name");
            }
            var msg = message != null ? String.Format(message, messageParams) : eventClass.Message;
            if (msg.Length > MAX_MESSAGE_LENGTH)
            {
                msg = msg.Substring(0, MAX_MESSAGE_LENGTH);
            }

            if (TraceLogger == null)
            {
                // --- Log it to the Windows Event log
                var logger = new EventLog(LogNameMapper.Map(eventClass.LogName))
                {
                    Source = LogSourceMapper.Map(eventClass.Source),
                    MachineName = ".",
                };
                logger.WriteEntry(msg, eventClass.Type, eventClass.EventId, eventClass.CategoryId);
            }
            else
            {
                // --- Log it to the secondary log
                LogToSecondary(eventClass, msg);
            }
        }

        /// <summary>
        /// Logs the message into the EventLog defined by <typeparamref name="TEventDefinition"/>
        /// </summary>
        /// <typeparam name="TEventDefinition">
        /// Must be a derived class of <see cref="LogEventBase"/>
        /// </typeparam>
        /// <param name="ex">Exception to log</param>
        public static void Log<TEventDefinition>(Exception ex)
            where TEventDefinition : LogEventBase, new()
        {
            Log<TEventDefinition>("An unexcepted exception has been raised", ex);
        }

        /// <summary>
        /// Logs the message into the EventLog defined by <typeparamref name="TEventDefinition"/>
        /// </summary>
        /// <typeparam name="TEventDefinition">
        /// Must be a derived class of <see cref="LogEventBase"/>
        /// </typeparam>
        /// <param name="message">The string message to log</param>
        /// <param name="ex">Exception to log</param>
        public static void Log<TEventDefinition>(string message, Exception ex)
            where TEventDefinition : LogEventBase, new()
        {
            // --- Prepare the message to log
            var eventClass = new TEventDefinition();
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(message);
            messageBuilder.AppendFormat("{0}: {1}", ex.GetType(), ex.Message);
            messageBuilder.AppendLine();
            messageBuilder.AppendLine(ex.StackTrace);
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                messageBuilder.AppendFormat("{0}: ", ex);
                messageBuilder.AppendLine(ex.Message);
                messageBuilder.AppendLine(ex.StackTrace);
                innerEx = innerEx.InnerException;
            }
            var finalMessage = messageBuilder.ToString();
            if (finalMessage.Length > MAX_MESSAGE_LENGTH)
            {
                finalMessage = finalMessage.Substring(0, MAX_MESSAGE_LENGTH);
            }

            if (TraceLogger == null)
            {
                var logger = new EventLog(LogNameMapper.Map(eventClass.LogName))
                {
                    Source = LogSourceMapper.Map(eventClass.Source),
                    MachineName = ".",
                };
                logger.WriteEntry(finalMessage, eventClass.Type, eventClass.EventId, eventClass.CategoryId);
            }
            else
            {
                // --- Log it to the secondary log
                LogToSecondary(eventClass, finalMessage);
            }
        }

        /// <summary>
        /// Log the specified message to the secondary event log
        /// </summary>
        /// <param name="eventClass">Event parameters</param>
        /// <param name="finalMessage">Message to log</param>
        private static void LogToSecondary(LogEventBase eventClass, string finalMessage)
        {
            var itemType = TraceLogItemType.Informational;
            switch (eventClass.Type)
            {
                case EventLogEntryType.FailureAudit:
                case EventLogEntryType.Error:
                    itemType = TraceLogItemType.Error;
                    break;
                case EventLogEntryType.Warning:
                    itemType = TraceLogItemType.Warning;
                    break;
                case EventLogEntryType.SuccessAudit:
                    itemType = TraceLogItemType.Success;
                    break;
            }
            var logItem = new TraceLogItem
            {
                Message = finalMessage,
                DetailedMessage = String.Format("EventId: {0}, CategoryId: {1}",
                    eventClass.EventId, eventClass.CategoryId),
                OperationType = "Windows Event Trace",
                Type = itemType
            };
            TraceLogger.Log(logItem);
        }
    }
}