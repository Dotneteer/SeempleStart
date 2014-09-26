using System;
using System.Diagnostics;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class represents the type of a perfromance counter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcTypeAttribute : PmcAttributeBase
    {
        /// <summary>
        /// Gets the flag indicating multi-instancing
        /// </summary>
        public PerformanceCounterType Type { get; private set; }

        /// <summary>
        /// Creates a performance counter type 
        /// </summary>
        /// <param name="type">Type fo the counter/param>
        public PmcTypeAttribute(PerformanceCounterType type)
        {
            Type = type;
        }
    }
}