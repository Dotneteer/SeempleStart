using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Seemplest.Core.Configuration.ResourceConnections;
using Seemplest.Core.Queue;
using Seemplest.Core.Tasks.Configuration;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// Thisd class implements a default task execution context.
    /// </summary>
    public class DefaultTaskExecutionContext : ITaskExecutionContext
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly INamedQueueProvider _queueProvider;
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        /// <summary>
        /// Gets the cancellation token of the execution context.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource
        {
            get { return _tokenSource; }
        }

        /// <summary>
        /// Creates a new execution context using the specified arguments.
        /// </summary>
        /// <param name="queueProvider">Named queue provider object</param>
        /// <param name="properties">Dictionary of properties</param>
        public DefaultTaskExecutionContext(INamedQueueProvider queueProvider, IDictionary<string, object> properties = null)
        {
            _queueProvider = queueProvider;
            if (properties != null)
            {
                _properties = new Dictionary<string, object>(properties);
            }
        }

        /// <summary>
        /// Creates a new execution context using the specified settings.
        /// </summary>
        /// <param name="settings">Context settings</param>
        public DefaultTaskExecutionContext(TaskExecutionContextSettings settings)
        {
            _queueProvider = ResourceConnectionFactory.CreateResourceConnection<INamedQueueProvider>(settings.ProviderKey);
            _properties = new Dictionary<string, object>();
            foreach (var property in settings.Properties.Values)
            {
                var value = Convert.ChangeType(property.Value, property.Type, CultureInfo.InvariantCulture);
                _properties.Add(property.Name, value);
            }
        }

        /// <summary>
        /// Gets the queue specified with the name.
        /// </summary>
        /// <param name="queueName">Name of the queue to access</param>
        /// <returns>Queue instance, if exists; otherwise, null</returns>
        public INamedQueue GetNamedQueue(string queueName)
        {
            return _queueProvider.GetQueue(queueName);
        }

        /// <summary>
        /// Gest the specified property of the execution context
        /// </summary>
        /// <param name="key">PropertySettings key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>PropertySettings value, if found; otherwise, defaultValue</returns>
        public T GetProperty<T>(string key, T defaultValue = default(T))
        {
            object value;
            return _properties.TryGetValue(key, out value) ? (T)value : defaultValue;
        }

        /// <summary>
        /// Sets the specified property of the execution context.
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        public void SetProperty(string key, object value)
        {
            _properties[key] = value;
        }
    }
}