using System.Threading;
using Seemplest.Core.Queue;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the execution context of a task.
    /// </summary>
    public interface ITaskExecutionContext
    {
        /// <summary>
        /// Gets the cancellation token of the execution context.
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Gets the queue specified with the name.
        /// </summary>
        /// <param name="queueName">Name of the queue to access</param>
        /// <returns>Queue instance, if exists; otherwise, defaultValue</returns>
        INamedQueue GetNamedQueue(string queueName);

        /// <summary>
        /// Gest the specified property of the execution context
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>PropertySettings value, if found; otherwise, null</returns>
        T GetProperty<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Sets the specified property of the execution context.
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        void SetProperty(string key, object value);
    }
}