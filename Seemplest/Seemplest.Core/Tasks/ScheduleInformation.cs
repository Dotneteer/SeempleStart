using System;
using System.Collections.Generic;
using System.Linq;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class describes a shedule that is used to run tasks.
    /// </summary>
    public class ScheduleInformation : IScheduleInformation
    {
        const int JANUARY_INDEX = 1;
        const int DECEMBER_INDEX = 12;
        const int LASTDAYOFDEC = 31;
        const int FIRSTDAYOFJAN = 1;
        const int THURSDAY = 4;

        private int _frequency;

        /// <summary>
        /// Gets or sets the first date this task must run.
        /// </summary>
        /// <remarks>Use null, if there is no constraint for this date.</remarks>
        public DateTime? RunFrom { get; set; }

        /// <summary>
        /// Gets or sets the last date this task must run.
        /// </summary>
        /// <remarks>Use null, if there is no constraint for this date.</remarks>
        public DateTime? RunTo { get; set; }

        /// <summary>
        /// Gets or sets the task frequency type for this task.
        /// </summary>
        public TaskFrequencyType FrequencyType { get; set; }

        /// <summary>
        /// Gets or sets the frequency when the task should run.
        /// </summary>
        /// <remarks>
        /// Cannot be zero or less.
        /// </remarks>
        public int Frequency
        {
            get { return _frequency; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Frequency cannot be less than or equal to zero", "value");
                }
                _frequency = value;
            }
        }

        /// <summary>
        /// Offset from the beginning of the "zero" time point.
        /// </summary>
        /// <remarks>
        /// "Zero" time points:
        ///     Monthly Frequency: the first day of a month
        ///     Weekly Frequency: mondays
        ///     Daily Frequency: 00:00:00 in a day (or 12:00:00 AM)
        ///     Hourly Frequency: The zero minute in a hour
        ///     Minutely Frequency: The zero second in a minute
        ///     Secondly Frequency: Not defined (doesn't work)
        /// </remarks>

        public TimeSpan Offset { get; set; }
        /// <summary>
        /// Gets or sets the day of week when the task should run.
        /// </summary>
        /// <remarks>
        /// Only with daily frequency
        /// </remarks>
        public IEnumerable<DayOfWeek> RunOnDayOfWeek { get; set; }

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
        public virtual DateTime NextTimeToRun(DateTime calculateFrom)
        {
            var date = RunFrom.HasValue && RunFrom.Value > calculateFrom
                ? RunFrom.Value : calculateFrom;

            if (RunTo.HasValue && RunTo.Value < calculateFrom)
                return DateTime.MaxValue;

            switch (FrequencyType)
            {
                case TaskFrequencyType.None:
                    break;
                case TaskFrequencyType.Month:
                    date = NextTimeToRunWithMonthlyFrequency(date);
                    break;
                case TaskFrequencyType.Week:
                    date = NextTimeToRunWithWeeklyFrequency(date);
                    break;
                case TaskFrequencyType.Day:
                    if (RunOnDayOfWeek == null || !RunOnDayOfWeek.Any() || RunOnDayOfWeek.Count() == 7)
                        date = NextTimeToRunWithDaylyFrequencyWithoutSpecificDays(date);
                    else
                        date = NextTimeToRunWithDaylyFrequencyWithSpecificDays(date);
                    break;
                case TaskFrequencyType.Hour:
                    date = NextTimeToRunWithHourlyFrequency(date);
                    break;
                case TaskFrequencyType.Minute:
                    date = NextTimeToRunWithMinuteLyFrequency(date);
                    break;
                case TaskFrequencyType.Second:
                    date = NextTimeToRunWithSecondlyFrequency(date);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("calculateFrom");
            }
            if (RunTo.HasValue && RunTo.Value < date) return DateTime.MaxValue;
            return date;
        }

        private DateTime NextTimeToRunWithMonthlyFrequency(DateTime date)
        {
            var returnDate = DateTime.MinValue
                .AddYears(date.Year - 1)
                .AddMonths(date.Month - 1);
            var month = Frequency - (returnDate.Month % Frequency);
            returnDate = returnDate.Add(Offset).AddMonths(month - Frequency);
            while (returnDate <= date)
                returnDate = returnDate.AddMonths(Frequency);
            return returnDate;
        }

        /// <summary>
        /// This method calculates the next time the task with this schedule should run.
        /// </summary>
        /// <param name="date">Date to calculate the next run time point form</param>
        /// <returns></returns>
        private DateTime NextTimeToRunWithWeeklyFrequency(DateTime date)
        {
            var returnDate = DateTime.MinValue.AddYears(date.Year - 1).AddMonths(date.Month - 1);
            while (returnDate.DayOfWeek != DayOfWeek.Monday)
                returnDate = returnDate.AddDays(1);
            var weeks = GetWeekNumber(date);
            var realOffset = Offset
                .Subtract(TimeSpan.FromDays(Offset.Days))
                .Add(TimeSpan.FromDays(Offset.Days % (7 * Frequency)));
            weeks += Frequency - weeks % Frequency;
            returnDate = returnDate.AddDays((weeks - 1) * 7).Add(realOffset);
            while (returnDate < date)
                returnDate = returnDate.AddDays(7 * Frequency);
            return returnDate;
        }

        private DateTime NextTimeToRunWithDaylyFrequencyWithoutSpecificDays(DateTime date)
        {
            var returnDate = DateTime.MinValue
                .AddYears(date.Year - 1)
                .AddMonths(date.Month - 1)
                .AddDays(date.Day - 1);
            var days = Frequency - (returnDate.DayOfYear % Frequency);
            returnDate = returnDate.Add(Offset);
            returnDate = returnDate.AddDays(days);
            while (returnDate < date)
                returnDate = returnDate.AddDays(Frequency);
            return returnDate;
        }

        private DateTime NextTimeToRunWithDaylyFrequencyWithSpecificDays(DateTime date)
        {
            var returnDate = DateTime.MinValue
                .AddYears(date.Year - 1)
                .AddMonths(date.Month - 1)
                .AddDays(date.Day - 1);
            var realOffset = Offset;
            while (returnDate <= date || !RunOnDayOfWeek.Contains(returnDate.DayOfWeek))
                returnDate = returnDate.AddDays(1);
            returnDate = returnDate.Add(realOffset);
            return returnDate;
        }

        private DateTime NextTimeToRunWithHourlyFrequency(DateTime date)
        {
            var returnDate = DateTime.MinValue
                .AddYears(date.Year - 1)
                .AddMonths(date.Month - 1)
                .AddDays(date.Day - 1);
            var hours = Frequency - (returnDate.Hour % Frequency);
            returnDate = returnDate.Add(Offset).AddHours(hours);
            while (returnDate < date) returnDate = returnDate.AddHours(Frequency);
            return returnDate;
        }

        private DateTime NextTimeToRunWithMinuteLyFrequency(DateTime date)
        {
            var returnDate = DateTime.MinValue
                .AddYears(date.Year - 1)
                .AddMonths(date.Month - 1)
                .AddDays(date.Day - 1)
                .AddHours(date.Hour)
                .AddMinutes(date.Minute);
            var minutes = Frequency - (returnDate.Minute % Frequency);
            returnDate = returnDate.Add(Offset).AddMinutes(minutes);
            while (returnDate < date)
                returnDate = returnDate.AddMinutes(Frequency);
            return returnDate;
        }

        private DateTime NextTimeToRunWithSecondlyFrequency(DateTime date)
        {
            var returnDate = DateTime.MinValue
                .AddYears(date.Year - 1)
                .AddMonths(date.Month - 1)
                .AddDays(date.Day - 1)
                .AddHours(date.Hour)
                .AddMinutes(date.Minute)
                .AddSeconds(date.Second);
            var seconds = Frequency - (returnDate.Second % Frequency);
            returnDate = returnDate.Add(Offset).AddSeconds(seconds);
            while (returnDate < date)
                returnDate = returnDate.AddSeconds(Frequency);
            return returnDate;
        }

        //Source: http://www.java2s.com/Code/CSharp/Development-Class/Gettheweeknumber.htm
        private static int GetWeekNumber(DateTime date)
        {
            var thursdayFlag = false;

            // Single the day number since the beginning of the year
            var dayOfYear = date.DayOfYear;

            // Single the numeric weekday of the first day of the 
            // year (using sunday as FirstDay)
            var startWeekDayOfYear =
                (int)(new DateTime(date.Year, JANUARY_INDEX, FIRSTDAYOFJAN)).DayOfWeek;
            var endWeekDayOfYear =
                (int)(new DateTime(date.Year, DECEMBER_INDEX, LASTDAYOFDEC)).DayOfWeek;

            // Compensate for the fact that we are using monday
            // as the first day of the week
            if (startWeekDayOfYear == 0)
                startWeekDayOfYear = 7;
            if (endWeekDayOfYear == 0)
                endWeekDayOfYear = 7;

            // Calculate the number of days in the first and last week
            var daysInFirstWeek = 8 - (startWeekDayOfYear);

            // If the year either starts or ends on a thursday it will have a 53rd week
            if (startWeekDayOfYear == THURSDAY || endWeekDayOfYear == THURSDAY)
                thursdayFlag = true;

            // We begin by calculating the number of FULL weeks between the start of the year and
            // our date. The number is rounded up, so the smallest possible value is 0.
            var fullWeeks = (int)Math.Ceiling((dayOfYear - (daysInFirstWeek)) / 7.0);

            var weekNumber = fullWeeks;

            // If the first week of the year has at least four days, then the actual week number for our date
            // can be incremented by one.
            if (daysInFirstWeek >= THURSDAY)
                weekNumber = weekNumber + 1;

            // If week number is larger than week 52 (and the year doesn't either start or end on a thursday)
            // then the correct week number is 1.
            if (weekNumber > 52 && !thursdayFlag)
                weekNumber = 1;

            // If week number is still 0, it means that we are trying to evaluate the week number for a
            // week that belongs in the previous year (since that week has 3 days or less in our date's year).
            // We therefore make a recursive call using the last day of the previous year.
            if (weekNumber == 0)
                weekNumber = GetWeekNumber(new DateTime(date.Year - 1, DECEMBER_INDEX, LASTDAYOFDEC));
            return weekNumber;
        }
    }
}