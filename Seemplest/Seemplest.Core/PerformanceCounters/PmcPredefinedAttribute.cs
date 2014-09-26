using System;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class signs if a performance counter is a predefined category
    /// </summary>
    /// <remarks>Predefined counters are not installed.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcPredefinedAttribute : PmcAttributeBase
    {
        /// <summary>
        /// Gets the flag indicating multi-instancing
        /// </summary>
        public bool IsPredefined { get; private set; }

        /// <summary>
        /// Creates a performance counter category with the specified multi-instancing
        /// </summary>
        /// <param name="isPredefined">Multi-instancing flag</param>
        public PmcPredefinedAttribute(bool isPredefined = true)
        {
            IsPredefined = isPredefined;
        }
    }
}