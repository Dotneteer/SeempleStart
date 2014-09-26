using System;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// Use this attribute to specify an id for a <see cref="LogEventBase"/> 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventIdAttribute : EventAttributeBase
    {
        /// <summary>
        /// Gets the event identifier value.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified event identifier.
        /// </summary>
        /// <param name="id">Event identifier</param>
        public EventIdAttribute(int id)
        {
            Value = id;
        }
    }
}