using System;
using System.Diagnostics;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class describes the type of the performance counter category
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcCategoryTypeAttribute : PmcAttributeBase
    {
        public PerformanceCounterCategoryType Type { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified category type.
        /// </summary>
        /// <param name="type"></param>
        public PmcCategoryTypeAttribute(PerformanceCounterCategoryType type)
        {
            Type = type;
        }
    }
}