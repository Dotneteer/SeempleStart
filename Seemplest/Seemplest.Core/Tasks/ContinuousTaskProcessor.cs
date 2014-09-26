using System;
using System.Diagnostics;
using Seemplest.Core.WindowsEventLog;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This task processor implements a task that continuously processes the specified task.
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    public class ContinuousTaskProcessor<TTask> : TaskProcessorBase
        where TTask : class, ITask, new()
    {
        /// <summary>
        /// Checks wether there is any task to execute
        /// </summary>
        /// <returns>
        /// True, if there is any task that can be executed; otherwise, false.
        /// </returns>
        protected override bool HasAnyTaskToExecute()
        {
            return true;
        }

        /// <summary>
        /// Processes tasks that are ready to run.
        /// </summary>
        protected override void ProcessTasks()
        {
            // --- Create the task, and dispose it when processed
            using (var newTask = new TTask())
            {
                var success = true;
                var stopwatch = new Stopwatch();
                try
                {
                    stopwatch.Restart();

                    // --- Setup with graceful cancellation
                    if (ShouldStopTaskProcessing) return;
                    newTask.Setup(Context);

                    // --- Run with graceful cancellation
                    CancellationToken.ThrowIfCancellationRequested();
                    newTask.Run();
                    NumTasksPmc.Increment();
                    NumTasksPerSecondPmc.Increment();
                }
                catch (OperationCanceledException ex)
                {
                    // --- The message procession canceled, log this failure.
                    WindowsEventLogger.Log<TaskExecutionInterrupted>(
                        "Task execution interrupted while processing a continuous task.", ex);
                    success = false;
                }
                catch (Exception ex)
                {
                    // --- The message procession failed, log this failure.
                    WindowsEventLogger.Log<TaskExecutionFailed>(
                        "Task execution failed while processing a continuous task.", ex);
                    success = false;
                }
                stopwatch.Stop();
                if (!success)
                {
                    NumFailuresPmc.Increment();
                    NumFailuresPerSecondPmc.Increment();
                }
                LastProcessTimePmc.RawValue = (int)stopwatch.ElapsedMilliseconds;
            }
        }
    }
}