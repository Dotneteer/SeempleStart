using System.Threading;
using Seemplest.Core.Queue;
using Seemplest.Core.Tasks;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// Executin context for email processing
    /// </summary>
    public class EmailTaskExecutionContext : ITaskExecutionContext
    {
        public EmailTaskExecutionContext()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets the queue specified with the name.
        /// </summary>
        /// <param name="queueName">Name of the queue to access</param>
        /// <returns>Queue instance, if exists; otherwise, defaultValue</returns>
        public INamedQueue GetNamedQueue(string queueName)
        {
            return null;
        }

        /// <summary>
        /// Gest the specified property of the execution context
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>PropertySettings value, if found; otherwise, null</returns>
        public T GetProperty<T>(string key, T defaultValue = default(T))
        {
            return default(T);
        }

        /// <summary>
        /// Sets the specified property of the execution context.
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        public void SetProperty(string key, object value)
        {
        }

        /// <summary>
        /// Gets the cancellation token of the execution context.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }
    }
}