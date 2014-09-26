using System;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// Attribute describing the help text of a performance counter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PmcHelpAttribute: PmcStringAttributeBase
    {
        /// <summary>
        /// Creates an instance with the specified value
        /// </summary>
        /// <param name="value">Attribute value</param>
        public PmcHelpAttribute(string value)
            : base(value)
        {
        }
    }
}