using System;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// Use this attribute to specify a message for a <see cref="LogEventBase"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventMessageAttribute : EventAttributeBase
    {
        /// <summary>
        /// Gets the event message value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified message.
        /// </summary>
        /// <param name="message">Source name value</param>
        public EventMessageAttribute(string message)
        {
            Value = message;
        }
    }
}