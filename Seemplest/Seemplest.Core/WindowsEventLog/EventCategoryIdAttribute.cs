using System;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// Use this attribute to specify a category for a <see cref="LogEventBase"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventCategoryIdAttribute : EventAttributeBase
    {
        /// <summary>
        /// Gets the category identifier.
        /// </summary>
        public short Value { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified event category identifier.
        /// </summary>
        /// <param name="categoryId">Event category identifier</param>
        public EventCategoryIdAttribute(short categoryId)
        {
            Value = categoryId;
        }
    }
}