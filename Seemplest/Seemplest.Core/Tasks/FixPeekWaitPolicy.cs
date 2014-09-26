namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This peek policy uses a fix wait time defined in the task execution context's
    /// PeekWaitTime property.
    /// </summary>
    public class FixPeekWaitPolicy : IQueuePeekPolicy
    {
        private const string PEEK_WAIT_TIME = "PeekWaitTime";
        private int _waitTime;

        /// <summary>
        /// Sets the execution context this peek policy is used within.
        /// </summary>
        /// <param name="context">Task execution context</param>
        /// <remarks>The policy can use the context to access its properties.</remarks>
        public void SetTaskExecutionContext(ITaskExecutionContext context)
        {
            _waitTime = context == null ? 0 : context.GetProperty(PEEK_WAIT_TIME, 0);
        }

        /// <summary>
        /// Gets the timespan from the last peek of the request queue to make the next
        /// peek, in milliseconds.
        /// </summary>
        /// <returns>
        /// Number of milliseconds from the last peek of the request queue to make the next
        /// peek.
        /// </returns>
        public int NextPeekTimeInMilliseconds()
        {
            return _waitTime;
        }
    }
}