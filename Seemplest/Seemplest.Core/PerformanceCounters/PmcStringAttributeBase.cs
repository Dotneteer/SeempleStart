namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// Abstract attribute type for a String value
    /// </summary>
    public abstract class PmcStringAttributeBase: PmcAttributeBase
    {
        /// <summary>
        /// Gets the value of the attribute
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Creates an instance with the specified value
        /// </summary>
        /// <param name="value">Attribute value</param>
        protected PmcStringAttributeBase(string value)
        {
            Value = value;
        }
    }
}