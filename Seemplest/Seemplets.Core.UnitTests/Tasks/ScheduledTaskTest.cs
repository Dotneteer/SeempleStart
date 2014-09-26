using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Tasks;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Tasks
{
    [TestClass]
    public class ScheduledTaskTest
    {
        [TestMethod]
        public void ParamHigherThanRunToWork()
        {
            // --- Arrange
            var tester = new TestSchedule { RunTo = new DateTime(2004, 01, 02) };
            var param = new DateTime(2008, 02, 04);
            var expected = DateTime.MaxValue;

            // --- Act
            var real = tester.NextTimeToRun(param);

            // --- Assert
            real.ShouldEqual(expected);
        }

        [TestMethod]
        public void MonthFrequencyWorks()
        {
            // --- Arrange
            var tester1 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Month,
                Frequency = 2,
                Offset = TimeSpan.Zero
            };
            var tester2 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Month,
                Frequency = 2,
                Offset = TimeSpan.FromDays(5).Add(TimeSpan.FromHours(-5))
            };
            var tester3 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Month,
                Frequency = 4,
                Offset = TimeSpan.FromDays(5)
            };
            var param = new DateTime(2011, 05, 01);
            var expected1 = new DateTime(2011, 06, 01);
            var expected2 = new DateTime(2011, 06, 05, 19, 0, 0);
            var expected3 = new DateTime(2011, 08, 06);

            // --- Act
            var real1 = tester1.NextTimeToRun(param);
            var real2 = tester2.NextTimeToRun(param);
            var real3 = tester3.NextTimeToRun(param);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
            real3.ShouldEqual(expected3);
        }

        [TestMethod]
        public void WeekFrequencyWorks()
        {
            // --- Arrange
            var tester1 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Week,
                Frequency = 1,
                Offset = TimeSpan.Zero
            };
            var tester2 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Week,
                Frequency = 1,
                Offset = TimeSpan.FromDays(-1).Add(TimeSpan.FromHours(-5))
            };
            var tester3 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Week,
                Frequency = 1,
                Offset = TimeSpan.FromDays(-7)
            };
            var tester4 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Week,
                Frequency = 3,
                Offset = TimeSpan.Zero
            };
            var tester5 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Week,
                Frequency = 3,
                Offset = TimeSpan.FromDays(-1)
            };
            var tester6 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Week,
                Frequency = 3,
                Offset = TimeSpan.FromDays(-7)
            };

            var param1 = new DateTime(2011, 1, 5);
            var param2 = new DateTime(2011, 1, 9);
            var expected1 = new DateTime(2011, 1, 10);
            var expected2 = new DateTime(2011, 1, 15, 19, 0, 0);
            var expected3 = new DateTime(2011, 1, 17);
            var expected4 = new DateTime(2011, 1, 16);

            // --- Act
            var real1 = tester1.NextTimeToRun(param1);
            var real2 = tester2.NextTimeToRun(param2);
            var real3 = tester3.NextTimeToRun(param1);
            var real4 = tester4.NextTimeToRun(param1);
            var real5 = tester5.NextTimeToRun(param1);
            var real6 = tester6.NextTimeToRun(param1);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
            real3.ShouldEqual(expected1);
            real4.ShouldEqual(expected3);
            real5.ShouldEqual(expected4);
            real6.ShouldEqual(expected1);
        }

        [TestMethod]
        public void DayFrequencyWithoutSpecificDaysWork()
        {
            // --- Arrange
            var tester1 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Day,
                Frequency = 1,
                Offset = TimeSpan.Zero
            };
            var tester2 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Day,
                Frequency = 1,
                Offset = TimeSpan.FromHours(-2)
            };
            var tester3 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Day,
                Frequency = 1,
                Offset = TimeSpan.FromHours(-3)
            };
            var tester4 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Day,
                Frequency = 3,
                Offset = TimeSpan.Zero
            };
            var tester5 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Day,
                Frequency = 3,
                Offset = TimeSpan.FromHours(-2)
            };
            var tester6 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Day,
                Frequency = 3,
                Offset = TimeSpan.FromHours(-3)
            };
            var param = new DateTime(2011, 01, 08, 22, 0, 0);
            var expected1 = new DateTime(2011, 1, 9, 0, 0, 0);
            var expected2 = new DateTime(2011, 1, 8, 22, 0, 0);
            var expected3 = new DateTime(2011, 1, 9, 21, 0, 0);
            var expected4 = new DateTime(2011, 1, 9, 0, 0, 0);
            var expected5 = new DateTime(2011, 1, 8, 22, 0, 0);
            var expected6 = new DateTime(2011, 1, 11, 21, 0, 0);

            // --- Act
            var real1 = tester1.NextTimeToRun(param);
            var real2 = tester2.NextTimeToRun(param);
            var real3 = tester3.NextTimeToRun(param);
            var real4 = tester4.NextTimeToRun(param);
            var real5 = tester5.NextTimeToRun(param);
            var real6 = tester6.NextTimeToRun(param);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
            real3.ShouldEqual(expected3);
            real4.ShouldEqual(expected4);
            real5.ShouldEqual(expected5);
            real6.ShouldEqual(expected6);
        }

        [TestMethod]
        public void DayFrequencyWithSpecificDaysWork()
        {
            // --- Arrange
            var tester = new TestSchedule
            {
                RunOnDayOfWeek = new List<DayOfWeek>
                                     {
                                         DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday
                                     },
                FrequencyType = TaskFrequencyType.Day,
                Offset = TimeSpan.Zero
            };
            var param1 = new DateTime(2011, 6, 8, 0, 0, 0);
            var param2 = new DateTime(2011, 6, 10, 1, 0, 0);
            var expected1 = new DateTime(2011, 6, 10, 0, 0, 0);
            var expected2 = new DateTime(2011, 6, 11, 0, 0, 0);

            // --- Act
            var real1 = tester.NextTimeToRun(param1);
            var real2 = tester.NextTimeToRun(param2);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
        }

        [TestMethod]
        public void HourFrequencyWorks()
        {
            // --- Arrange
            var tester1 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Hour,
                Frequency = 1,
                Offset = TimeSpan.Zero
            };
            var tester2 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Hour,
                Frequency = 1,
                Offset = TimeSpan.FromMinutes(-5)
            };
            var tester3 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Hour,
                Frequency = 3,
                Offset = TimeSpan.Zero
            };
            var tester4 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Hour,
                Frequency = 3,
                Offset = TimeSpan.FromMinutes(-5)
            };
            var param1 = new DateTime(2011, 06, 08, 0, 0, 0);
            var param2 = new DateTime(2011, 06, 08, 11, 5, 0);
            var expected1 = new DateTime(2011, 06, 08, 1, 0, 0);
            var expected2 = new DateTime(2011, 06, 08, 12, 0, 0);
            var expected3 = new DateTime(2011, 06, 08, 0, 55, 0);
            var expected4 = new DateTime(2011, 06, 08, 11, 55, 0);
            var expected5 = new DateTime(2011, 06, 08, 3, 0, 0);
            var expected6 = new DateTime(2011, 06, 08, 12, 0, 0);
            var expected7 = new DateTime(2011, 06, 08, 2, 55, 0);
            var expected8 = new DateTime(2011, 06, 08, 11, 55, 0);

            // --- Act
            var real1 = tester1.NextTimeToRun(param1);
            var real2 = tester1.NextTimeToRun(param2);
            var real3 = tester2.NextTimeToRun(param1);
            var real4 = tester2.NextTimeToRun(param2);
            var real5 = tester3.NextTimeToRun(param1);
            var real6 = tester3.NextTimeToRun(param2);
            var real7 = tester4.NextTimeToRun(param1);
            var real8 = tester4.NextTimeToRun(param2);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
            real3.ShouldEqual(expected3);
            real4.ShouldEqual(expected4);
            real5.ShouldEqual(expected5);
            real6.ShouldEqual(expected6);
            real7.ShouldEqual(expected7);
            real8.ShouldEqual(expected8);
        }

        [TestMethod]

        public void MinuteFrequencyWorks()
        {
            // --- Arrange
            var tester1 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Minute,
                Frequency = 1,
                Offset = TimeSpan.Zero
            };
            var tester2 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Minute,
                Frequency = 1,
                Offset = TimeSpan.FromSeconds(-5)
            };
            var tester3 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Minute,
                Frequency = 3,
                Offset = TimeSpan.Zero
            };
            var tester4 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Minute,
                Frequency = 3,
                Offset = TimeSpan.FromSeconds(-5)
            };
            var param1 = new DateTime(2011, 06, 08, 0, 5, 0);
            var param2 = new DateTime(2011, 06, 08, 11, 5, 5);
            var param3 = new DateTime(2011, 06, 08, 0, 6, 0);
            var expected1 = new DateTime(2011, 06, 08, 0, 6, 0);
            var expected2 = new DateTime(2011, 06, 08, 11, 6, 0);
            var expected3 = new DateTime(2011, 06, 08, 0, 5, 55);
            var expected4 = new DateTime(2011, 06, 08, 11, 5, 55);
            var expected5 = new DateTime(2011, 06, 08, 0, 6, 0);
            var expected6 = new DateTime(2011, 06, 08, 0, 9, 0);
            var expected7 = new DateTime(2011, 06, 08, 11, 6, 0);
            var expected8 = new DateTime(2011, 06, 08, 0, 5, 55);
            var expected9 = new DateTime(2011, 06, 08, 11, 5, 55);

            // --- Act
            var real1 = tester1.NextTimeToRun(param1);
            var real2 = tester1.NextTimeToRun(param2);
            var real3 = tester2.NextTimeToRun(param1);
            var real4 = tester2.NextTimeToRun(param2);
            var real5 = tester3.NextTimeToRun(param1);
            var real6 = tester3.NextTimeToRun(param3);
            var real7 = tester3.NextTimeToRun(param2);
            var real8 = tester4.NextTimeToRun(param1);
            var real9 = tester4.NextTimeToRun(param2);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
            real3.ShouldEqual(expected3);
            real4.ShouldEqual(expected4);
            real5.ShouldEqual(expected5);
            real6.ShouldEqual(expected6);
            real7.ShouldEqual(expected7);
            real8.ShouldEqual(expected8);
            real9.ShouldEqual(expected9);
        }

        [TestMethod]
        public void SecondFrequencyWorks()
        {
            // --- Arrange
            var tester1 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Second,
                Offset = TimeSpan.Zero,
                Frequency = 1
            };
            var tester2 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Second,
                Offset = TimeSpan.Zero,
                Frequency = 3
            };
            var tester3 = new TestSchedule
            {
                FrequencyType = TaskFrequencyType.Second,
                Offset = TimeSpan.FromSeconds(-1),
                Frequency = 3
            };
            var param1 = new DateTime(2011, 06, 08, 0, 5, 5);
            var param2 = new DateTime(2011, 06, 08, 0, 5, 6);
            var expected1 = new DateTime(2011, 06, 08, 0, 5, 6);
            var expected2 = new DateTime(2011, 06, 08, 0, 5, 6);
            var expected3 = new DateTime(2011, 06, 08, 0, 5, 9);
            var expected4 = new DateTime(2011, 06, 08, 0, 5, 8);

            // --- Act
            var real1 = tester1.NextTimeToRun(param1);
            var real2 = tester2.NextTimeToRun(param1);
            var real3 = tester2.NextTimeToRun(param2);
            var real4 = tester3.NextTimeToRun(param2);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
            real3.ShouldEqual(expected3);
            real4.ShouldEqual(expected4);
        }

        [TestMethod]
        public void ComplexSheduleWorks()
        {
            // --- Arrange
            var tester = new TestSchedule
            {
                RunFrom = new DateTime(2010, 4, 1),
                RunTo = new DateTime(2011, 04, 1),
                FrequencyType = TaskFrequencyType.Month,
                Frequency = 4,
                Offset = TimeSpan.FromDays(9)
            };
            var param1 = new DateTime(2010, 04, 10);
            var param2 = new DateTime(2009, 03, 04);
            var param3 = new DateTime(2011, 05, 05);
            var param4 = new DateTime(2011, 03, 06);
            var expected1 = new DateTime(2010, 08, 10);
            var expected2 = new DateTime(2010, 04, 10);
            var expected3 = DateTime.MaxValue;

            // --- Act
            var real1 = tester.NextTimeToRun(param1);
            var real2 = tester.NextTimeToRun(param2);
            var real3 = tester.NextTimeToRun(param3);
            var real4 = tester.NextTimeToRun(param4);

            // --- Assert
            real1.ShouldEqual(expected1);
            real2.ShouldEqual(expected2);
            real3.ShouldEqual(expected3);
            real4.ShouldEqual(expected3);
        }

        private class TestSchedule : ScheduleInformation
        {
        }
    }
}
