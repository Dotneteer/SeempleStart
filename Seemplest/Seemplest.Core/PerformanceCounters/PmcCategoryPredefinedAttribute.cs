using System;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class signs if a PmcCategory is a predefined one.
    /// </summary>
    /// <remarks>Predefined categories are not installed.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcCategoryPredefinedAttribute : PmcAttributeBase
    {
        /// <summary>
        /// Gets the flag indicating multi-instancing
        /// </summary>
        public bool IsPredefined { get; private set; }

        /// <summary>
        /// Creates a performance counter category with the specified multi-instancing
        /// </summary>
        /// <param name="isPredefined">Multi-instancing flag</param>
        public PmcCategoryPredefinedAttribute(bool isPredefined)
        {
            IsPredefined = isPredefined;
        }
    }
}