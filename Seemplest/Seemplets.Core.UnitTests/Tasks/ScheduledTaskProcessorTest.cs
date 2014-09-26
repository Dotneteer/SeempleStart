using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Queue;
using Seemplest.Core.Tasks;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Tasks
{
    [TestClass]
    public class ScheduledTaskProcessorTest
    {
        private static AutoResetEvent s_SetupAutoResetEvent;
        private static AutoResetEvent s_RunAutoResetEvent;
        private static DefaultTaskExecutionContext s_Context;

        [TestMethod]
        public void ScheduledTaskWorksAsExpected()
        {
            // --- Arrange
            var schedule = new ScheduleInformation
            {
                FrequencyType = TaskFrequencyType.Second,
                Frequency = 2,
            };
            var context = new DefaultTaskExecutionContext(new MemoryNamedQueueProvider());
            var processor =
                new ScheduledTaskProcessor<SimpleTask>(context) { ScheduleInfo = schedule };

            // --- Act
            SimpleTask.Counter = 0;
            processor.Start();
            Thread.Sleep(5000);
            processor.Stop();

            // --- Assert
            SimpleTask.Counter.ShouldBeLessThanOrEqualTo(3);
        }

        [TestMethod]
        public void FaultyScheduledTaskWorksAsExpected()
        {
            // --- Arrange
            var schedule = new ScheduleInformation
            {
                FrequencyType = TaskFrequencyType.Second,
                Frequency = 2,
            };
            var context = new DefaultTaskExecutionContext(new MemoryNamedQueueProvider());
            var processor =
                new ScheduledTaskProcessor<FaultyTask>(context) { ScheduleInfo = schedule };

            // --- Act
            FaultyTask.Counter = 0;
            processor.Start();
            Thread.Sleep(5000);
            processor.Stop();

            // --- Assert
            FaultyTask.Counter.ShouldBeLessThanOrEqualTo(3);
        }

        [TestMethod]
        public void CancelInSetupWorksAsExpected()
        {
            // --- Arrange
            using (s_SetupAutoResetEvent = new AutoResetEvent(false))
            {
                var schedule = new ScheduleInformation
                {
                    FrequencyType = TaskFrequencyType.Second,
                    Frequency = 2,
                };
                s_Context = new DefaultTaskExecutionContext(new MemoryNamedQueueProvider());
                var processor =
                    new ScheduledTaskProcessor<TaskWithSetupAutoResetEvent>(s_Context) { ScheduleInfo = schedule };
                TaskWithSetupAutoResetEvent.Counter = 0;

                // --- Act
                processor.Start();
                s_SetupAutoResetEvent.WaitOne(5000).ShouldBeTrue("Task did not get to setup phase in 5 seconds");
                processor.Stop();

                // --- Assert
                TaskWithSetupAutoResetEvent.Counter.ShouldEqual(0);
            }
        }

        //[TestMethod]
        //public void CancelInRunWorksAsExpected()
        //{
        //    // --- Arrange
        //    using (s_RunAutoResetEvent = new AutoResetEvent(false))
        //    {
        //        var testfilter = new LogSourceFilterSettings("*");
        //        var testroutedescriptor =
        //            new LogRouteSettings("name", true, new DiagnosticsLoggerSettings("inname", typeof(TestDiagnosticsLogger)));
        //        var testDiagnosticsLogger = (TestDiagnosticsLogger)testroutedescriptor.DiagnosticsLogger.Instance;
        //        testroutedescriptor.AddFilter(testfilter);
        //        var settings = new DiagnosticsConfigurationSettings(true,
        //                                                            new List<LogRouteSettings> { testroutedescriptor });
        //        DiagnosticsManager.Settings = settings;
        //        testDiagnosticsLogger.Logged.Clear();
        //        var schedule = new ScheduleInformation
        //        {
        //            FrequencyType = TaskFrequencyType.Second,
        //            Frequency = 2,
        //        };
        //        s_Context = new DefaultTaskExecutionContext(new MemoryNamedQueueProvider());
        //        var processor =
        //            new ScheduledTaskProcessor<TaskWithRunAutoResetEvent>(s_Context) { ScheduleInfo = schedule };
        //        TaskWithRunAutoResetEvent.Counter = 0;

        //        // --- Act
        //        processor.Start();
        //        s_RunAutoResetEvent.WaitOne(5000).ShouldBeTrue("Task did not get to setup phase in 5 seconds");
        //        processor.Stop();

        //        // --- Assert
        //        TaskWithRunAutoResetEvent.Counter.ShouldEqual(1);
        //        testDiagnosticsLogger.Logged.Count.ShouldEqual(0);
        //    }
        //}

        [TestMethod]
        public void CancelExceptionThrownWhileRunningWorksAsExpected()
        {
            // --- Arrange
            using (s_RunAutoResetEvent = new AutoResetEvent(false))
            {
                var schedule = new ScheduleInformation
                {
                    FrequencyType = TaskFrequencyType.Second,
                    Frequency = 2,
                };
                s_Context = new DefaultTaskExecutionContext(new MemoryNamedQueueProvider());
                var processor =
                    new ScheduledTaskProcessor<TaskWithExceptionThrownWhileRunning>(s_Context) { ScheduleInfo = schedule };
                TaskWithExceptionThrownWhileRunning.Counter = 0;

                // --- Act
                processor.Start();
                s_RunAutoResetEvent.WaitOne(5000).ShouldBeTrue("Task did not get to setup phase in 5 seconds");
                processor.Stop();

                // --- Assert
                TaskWithExceptionThrownWhileRunning.Counter.ShouldEqual(1);
            }
        }

        [TestMethod]
        public void StopInRunWorksAsExpected()
        {
            // --- Arrange
            using (s_RunAutoResetEvent = new AutoResetEvent(false))
            {
                var schedule = new ScheduleInformation
                {
                    FrequencyType = TaskFrequencyType.Second,
                    Frequency = 2,
                };
                var context = new DefaultTaskExecutionContext(new MemoryNamedQueueProvider());
                var processor =
                    new ScheduledTaskProcessor<TaskWithRunAutoResetEvent>(context) { ScheduleInfo = schedule };
                TaskWithRunAutoResetEvent.Counter = 0;

                //Act
                processor.Start();
                s_RunAutoResetEvent.WaitOne(5000).ShouldBeTrue("Task did not get to setup phase in 5 seconds");
                processor.Stop();

                //Assert
                TaskWithRunAutoResetEvent.Counter.ShouldEqual(1);
            }
        }

        private class SimpleTask : TaskBase
        {
            public static int Counter;

            public override void Run()
            {
                Console.WriteLine("Running: {0:hh:mm:ss,fff}", DateTime.Now);
                Counter++;
                Thread.Sleep(100);
            }
        }

        private class FaultyTask : TaskBase
        {
            public static int Counter;

            public override void Run()
            {
                Counter++;
                throw new InvalidOperationException();
            }
        }

        private class TaskWithSetupAutoResetEvent : TaskBase
        {
            public static int Counter;

            public override void Run()
            {
                Console.WriteLine("Running: {0:hh:mm:ss,fff}", DateTime.Now);
                Counter++;
                Thread.Sleep(100);
            }

            public override void Setup(ITaskExecutionContext context)
            {
                Console.WriteLine("Setup: {0:hh:mm:ss,fff}", DateTime.Now);
                s_SetupAutoResetEvent.Set();
                Thread.Sleep(20);
            }
        }

        private class TaskWithRunAutoResetEvent : TaskBase
        {
            public static int Counter;

            public override void Run()
            {
                Console.WriteLine("Running: {0:hh:mm:ss,fff}", DateTime.Now);
                Counter++;
                s_RunAutoResetEvent.Set();
                Thread.Sleep(100);

            }
        }

        private class TaskWithExceptionThrownWhileRunning : TaskBase
        {
            public static int Counter;

            public override void Run()
            {
                Console.WriteLine("Running: {0:hh:mm:ss,fff}", DateTime.Now);
                Counter++;
                s_RunAutoResetEvent.Set();
                Thread.Sleep(100);
                s_Context.CancellationTokenSource.Token.ThrowIfCancellationRequested();
                Thread.Sleep(100);

            }
        }

        //private class TestDiagnosticsLogger : IDiagnosticsLogger
        //{
        //    public readonly List<DiagnosticsLogItem> Logged;

        //    public TestDiagnosticsLogger()
        //    {
        //        Logged = new List<DiagnosticsLogItem>();
        //    }
        //    public void Log(DiagnosticsLogItem entry)
        //    {
        //        Logged.Add(entry);
        //    }
        //}
    }

}
