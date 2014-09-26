using System;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// Attribute describing the name of a performance counter category
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcCategoryNameAttribute : PmcStringAttributeBase
    {
        /// <summary>
        /// Creates an instance with the specified value
        /// </summary>
        /// <param name="value">Attribute value</param>
        public PmcCategoryNameAttribute(string value)
            : base(value)
        {
        }
    }
}