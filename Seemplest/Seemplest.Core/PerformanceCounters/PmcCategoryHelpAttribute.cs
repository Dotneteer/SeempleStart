using System;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// Attribute describing the help text of a performance counter category
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcCategoryHelpAttribute: PmcStringAttributeBase
    {
        /// <summary>
        /// Creates an instance with the specified value
        /// </summary>
        /// <param name="value">Attribute value</param>
        public PmcCategoryHelpAttribute(string value)
            : base(value)
        {
        }
    }
}