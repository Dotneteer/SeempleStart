using System;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This event argument describe the event when a configuration has been changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigurationChangedEventArgs<T>: EventArgs
    {
        /// <summary>
        /// Gets the value before the configuration change.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets the value after the configuration change
        /// </summary>
        public T NewValue { get; private set; }

        /// <summary>
        /// Creates a new instance using the specified event properties.
        /// </summary>
        /// <param name="oldValue">Old configuration value</param>
        /// <param name="newValue">New configuration value</param>
        public ConfigurationChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}