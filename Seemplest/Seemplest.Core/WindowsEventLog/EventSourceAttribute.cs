using System;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// Use this attribute to specify a source for a <see cref="LogEventBase"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventSourceAttribute : EventAttributeBase
    {
        /// <summary>
        /// Gets the event source value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified source name.
        /// </summary>
        /// <param name="sourceName">Source name value</param>
        public EventSourceAttribute(string sourceName)
        {
            Value = sourceName;
        }
    }
}