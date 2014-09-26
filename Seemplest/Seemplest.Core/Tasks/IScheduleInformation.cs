using System;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the behaviour of an object that can calculate schedule.
    /// </summary>
    public interface IScheduleInformation
    {
        /// <summary>
        /// Calculates the next point in time when this task should run.
        /// </summary>
        /// <param name="calculateFrom">
        /// The point in time to calculate the subsequent scheduled run.
        /// </param>
        /// <returns>
        /// The next point in time when the task should run. DateTime.MaxValue, 
        /// if the task should never run.
        /// </returns>
        DateTime NextTimeToRun(DateTime calculateFrom);
    }
}