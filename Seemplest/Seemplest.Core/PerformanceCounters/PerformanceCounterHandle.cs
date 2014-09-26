using System;
using System.Diagnostics;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class represents a handle to a physical performance counter
    /// </summary>
    public class PerformanceCounterHandle
    {
        private readonly PerformanceCounter _counterInstance;
        private readonly string _instanceName;

        public PerformanceCounterHandle(Type counterType, string instanceName = null)
        {
            if (!counterType.IsSubclassOf(typeof (PmcDefinitionBase)))
            {
                throw new InvalidOperationException(
                    String.Format("{0} type is not derived from PmcDefinitionBase.", counterType));
            }
            try
            {
                var counterDefinition = (PmcDefinitionBase)Activator.CreateInstance(counterType);
                if (counterDefinition.IsMultiInstance && string.IsNullOrWhiteSpace(instanceName))
                {
                    instanceName = counterDefinition.DefaultInstance;
                }
                _counterInstance = string.IsNullOrWhiteSpace(instanceName)
                    ? new PerformanceCounter(counterDefinition.CategoryName, counterDefinition.Name, counterDefinition.IsPredefined)
                    : new PerformanceCounter(counterDefinition.CategoryName, counterDefinition.Name, instanceName, counterDefinition.IsPredefined);
            }
            catch
            {
                // --- This exception is intentionally caught
                _counterInstance = null;
            }
            _instanceName = instanceName;
        }

        /// <summary>
        /// Gets the flag indicating whether this instance has a phycial performance counter or not.
        /// </summary>
        public bool HasInstance
        {
            get { return _counterInstance != null; }
        }

        /// <summary>
        /// Gets the instance name of the performance counter.
        /// </summary>
        public string InstanceName
        {
            get { return _instanceName; }
        }

        /// <summary>
        /// Gets the raw value of the counter.
        /// </summary>
        public long RawValue
        {
            get { return HasInstance ? _counterInstance.RawValue : 0; }
            set { if (HasInstance) _counterInstance.RawValue = value; }
        }

        /// <summary>
        /// Increments the performance counter value
        /// </summary>
        public void Increment()
        {
            if (HasInstance) _counterInstance.Increment();
        }

        /// <summary>
        /// Increments the performance counter with the specified value.
        /// </summary>
        /// <param name="value">Value to increment the counter with</param>
        public void IncrementBy(long value)
        {
            if (HasInstance) _counterInstance.IncrementBy(value);
        }

        /// <summary>
        /// Decrements the performance counter value
        /// </summary>
        public void Decrement()
        {
            if (HasInstance) _counterInstance.Decrement();
        }

        /// <summary>
        /// Obtains a counter sample, and returns the raw, or uncalculated value for it.
        /// </summary>
        /// <returns>Counter sample value</returns>
        public CounterSample NextSample()
        {
            return HasInstance ? _counterInstance.NextSample() : CounterSample.Empty;
        }

        /// <summary>
        /// Obtains a counter sample and returns the calculated value for it.
        /// </summary>
        /// <returns>Counter value</returns>
        public float NextValue()
        {
            return HasInstance ? _counterInstance.NextValue() : 0.0F;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in read-only mode.
        /// </summary>
        public bool ReadOnly
        {
            get { return HasInstance && _counterInstance.ReadOnly; }
            set { if (HasInstance) _counterInstance.ReadOnly = value; }
        }
    }
}