namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface represents the policy used to poll the queue for new
    /// messages
    /// </summary>
    public interface IQueuePeekPolicy
    {
        /// <summary>
        /// Sets the execution context this peek policy is used within.
        /// </summary>
        /// <param name="context">Task execution context</param>
        /// <remarks>The policy can use the context to access its properties.</remarks>
        void SetTaskExecutionContext(ITaskExecutionContext context);

        /// <summary>
        /// Gets the timespan from the last peek of the request queue to make the next
        /// peek, in milliseconds.
        /// </summary>
        /// <returns>
        /// Number of milliseconds from the last peek of the request queue to make the next
        /// peek.
        /// </returns>
        int NextPeekTimeInMilliseconds();
    }
}