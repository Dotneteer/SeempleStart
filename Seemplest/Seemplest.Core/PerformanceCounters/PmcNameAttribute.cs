using System;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// Attribute describing the name of a performance counter 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcNameAttribute : PmcStringAttributeBase
    {
        /// <summary>
        /// Creates an instance with the specified value
        /// </summary>
        /// <param name="value">Attribute value</param>
        public PmcNameAttribute(string value)
            : base(value)
        {
        }
    }
}