using System;
using System.Diagnostics;
using Seemplest.Core.Common;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// This class is intended to be the base of all classes defining an event to be written
    /// intot the Windows event log.
    /// </summary>
    /// <remarks>
    /// The derived classes of this class can be used as the generic parameter 
    /// of <see cref="WindowsEventLogger"/>
    /// </remarks>
    public abstract class LogEventBase
    {
        /// <summary>
        /// Gets the source of the event.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        public EventLogEntryType Type { get; private set; }

        /// <summary>
        /// Gets the Id of the event.
        /// </summary>
        public int EventId { get; private set; }

        /// <summary>
        /// Gets the category Id of the event.
        /// </summary>
        public short CategoryId { get; private set; }

        /// <summary>
        /// Gets the name of the log this event is intended to be written.
        /// </summary>
        public string LogName { get; private set; }

        /// <summary>
        /// Gets the flag indicating whether log name is valid
        /// </summary>
        public bool LogNameValid { get; private set; }

        /// <summary>
        /// Gest the default message of this event
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Creates a new instance of the class using the attributes decorating it.
        /// </summary>
        protected LogEventBase()
        {
            var attrSet = new AttributeSet(GetType(), typeof(EventAttributeBase));
            CategoryId = attrSet.Optional(new EventCategoryIdAttribute(0)).Value;
            EventId = attrSet.Optional(new EventIdAttribute(0)).Value;
            Source = attrSet.Single<EventSourceAttribute>().Value;
            Type = attrSet.Optional(new EventTypeAttribute(EventLogEntryType.Information)).Value;
            Message = attrSet.Optional(new EventMessageAttribute(String.Empty)).Value;
            var logNameAttr = attrSet.Single<EventLogNameAttribute>();
            LogName = logNameAttr.Value;
            LogNameValid = logNameAttr.IsValid;
        }
    }
}