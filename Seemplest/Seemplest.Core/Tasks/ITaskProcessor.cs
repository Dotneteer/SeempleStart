using System;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the responsibilities of an object that can process tasks.
    /// </summary>
    public interface ITaskProcessor : IDisposable
    {
        /// <summary>
        /// Gets or sets the name of task processor
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Sets the execution context of the task processor
        /// </summary>
        /// <param name="context"></param>
        void SetContext(ITaskExecutionContext context);

        /// <summary>
        /// Starts processing tasks.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops processing new tasks, but allow complete ones already under progress
        /// </summary>
        void Stop();
    }
}