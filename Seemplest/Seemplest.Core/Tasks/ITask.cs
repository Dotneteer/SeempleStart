using System;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface represents the responsibilities a task should implement
    /// </summary>
    public interface ITask : IDisposable
    {
        /// <summary>
        /// Sets up the task that will be run in the specified context.
        /// </summary>
        /// <param name="context">Task execution context</param>
        void Setup(ITaskExecutionContext context);

        /// <summary>
        /// Runs the specific task.
        /// </summary>
        void Run();
    }
}