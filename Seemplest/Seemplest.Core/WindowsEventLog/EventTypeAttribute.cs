using System;
using System.Diagnostics;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// Use this attribute to specify a type for a <see cref="LogEventBase"/> 
    /// </summary>
    /// <remarks>
    /// The usage of this attribute is required for the <see cref="LogEventBase"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventTypeAttribute : EventAttributeBase
    {
        /// <summary>
        /// Gets the type of the event log entry.
        /// </summary>
        public EventLogEntryType Value { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified entry type.
        /// </summary>
        /// <param name="type">Event log entry type</param>
        public EventTypeAttribute(EventLogEntryType type)
        {
            Value = type;
        }
    }
}