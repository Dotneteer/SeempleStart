using System;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// Attribute naming the default instance of a performance counter 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcDefaultInstanceAttribute : PmcStringAttributeBase
    {
        /// <summary>
        /// Creates an instance with the specified value
        /// </summary>
        /// <param name="value">Attribute value</param>
        public PmcDefaultInstanceAttribute(string value)
            : base(value)
        {
        }
    }
}