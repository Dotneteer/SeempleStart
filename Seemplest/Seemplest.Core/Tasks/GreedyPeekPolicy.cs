namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class implements a greedy peek policy that does not wait between peeks
    /// </summary>
    public class GreedyPeekPolicy : IQueuePeekPolicy
    {
        /// <summary>
        /// Sets the execution context this peek policy is used within.
        /// </summary>
        /// <param name="context">Task execution context</param>
        /// <remarks>This policy does not use the context.</remarks>
        public void SetTaskExecutionContext(ITaskExecutionContext context)
        {
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
            return 0;
        }
    }
}