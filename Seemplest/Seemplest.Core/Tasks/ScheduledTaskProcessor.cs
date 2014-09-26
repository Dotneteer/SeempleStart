using System;
using System.Collections.Generic;
using System.Diagnostics;
using Seemplest.Core.Configuration;
using Seemplest.Core.WindowsEventLog;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class is a basic implementation of a task processor that processes
    /// scheduled tasks.
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    public class ScheduledTaskProcessor<TTask> : TaskProcessorBase
        where TTask : class, ITask, new()
    {
        private DateTime _nextTimeToRun;
        private readonly TimeSpan _epsilon = TimeSpan.Zero;
        private ScheduleInformation _schedule = new ScheduleInformation();

        /// <summary>
        /// Gets or sets the schedule information used to execute this task.
        /// </summary>
        public IScheduleInformation ScheduleInfo
        {
            get { return _schedule; }
            set { _schedule = (ScheduleInformation)value; }
        }

        /// <summary>
        /// Creates a new instance of the scheduled task processor
        /// </summary>
        public ScheduledTaskProcessor()
        {
            Init();
        }

        /// <summary>
        /// Creates a new scheduled task procerros and binds it to the sepcified context.
        /// </summary>
        /// <param name="context">Task execution context</param>
        public ScheduledTaskProcessor(ITaskExecutionContext context)
            : base(context)
        {
            Init();
        }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        private void Init()
        {
        }

        /// <summary>
        /// Gets or sets the first date this task must run.
        /// </summary>
        /// <remarks>Use null, if there is no constraint for this date.</remarks>
        public DateTime? RunFrom
        {
            get { return _schedule.RunFrom; }
            set { _schedule.RunFrom = value; }
        }

        /// <summary>
        /// Gets or sets the last date this task must run.
        /// </summary>
        /// <remarks>Use null, if there is no constraint for this date.</remarks>
        public DateTime? RunTo
        {
            get { return _schedule.RunTo; }
            set { _schedule.RunTo = value; }
        }

        /// <summary>
        /// Gets or sets the task frequency type for this task.
        /// </summary>
        public TaskFrequencyType FrequencyType
        {
            get { return _schedule.FrequencyType; }
            set { _schedule.FrequencyType = value; }
        }

        /// <summary>
        /// Gets or sets the frequency when the task should run.
        /// </summary>
        public int Frequency
        {
            get { return _schedule.Frequency; }
            set { _schedule.Frequency = value; }
        }

        /// <summary>
        /// Offset from the beginning of the "zero" time point.
        /// </summary>
        public TimeSpan Offset
        {
            get { return _schedule.Offset; }
            set { _schedule.Offset = value; }
        }

        /// <summary>
        /// Gets or sets the day of week when the task should run.
        /// </summary>
        public IEnumerable<DayOfWeek> RunOnDayOfWeek
        {
            get { return _schedule.RunOnDayOfWeek; }
            set { _schedule.RunOnDayOfWeek = value; }
        }

        /// <summary>
        /// Starts processing messages.
        /// </summary>
        public override void Start()
        {
            var now = EnvironmentInfo.GetCurrentDateTimeUtc();
            _nextTimeToRun = ScheduleInfo.NextTimeToRun(now);
            base.Start();
        }

        /// <summary>
        /// Checks wether there is any task to execute
        /// </summary>
        /// <returns>
        /// True, if there is any task that can be executed; otherwise, false.
        /// </returns>
        protected override bool HasAnyTaskToExecute()
        {
            var now = EnvironmentInfo.GetCurrentDateTimeUtc();
            return (_nextTimeToRun - now) < _epsilon;
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
                        "Task execution interrupted while processing a scheduled task.", ex);
                    success = false;
                }
                catch (Exception ex)
                {
                    // --- The message procession failed, log this failure.
                    WindowsEventLogger.Log<TaskExecutionFailed>(
                        "Task execution failed while processing scheduled task.", ex);
                    success = false;
                }
                finally
                {
                    // --- Set up the next time when the task should run
                    _nextTimeToRun = ScheduleInfo.NextTimeToRun(EnvironmentInfo.GetCurrentDateTimeUtc());
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