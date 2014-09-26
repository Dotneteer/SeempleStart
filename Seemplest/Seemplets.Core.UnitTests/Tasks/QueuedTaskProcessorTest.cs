using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.Configuration.ResourceConnections;
using Seemplest.Core.Queue;
using Seemplest.Core.Tasks;
using Seemplest.Core.Tasks.Configuration;
using Seemplest.Core.TypeResolution;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Tasks
{
    [TestClass]
    public class QueuedTaskProcessorTest
    {
        private const string REQUEST_QUEUE = "requestQueue";
        private const string RESPONSE_QUEUE = "responseQueue";

        private static DefaultTaskExecutionContext s_Context;
        private static TestPeekPolicy s_PeekPolicy;
        private static AutoResetEvent s_TaskSetupAutoResetEvent;
        private static AutoResetEvent s_TasksCompletedAutoResetEvent;
        private static AutoResetEvent s_TaskRunningAutoResetEvent;

        [TestInitialize]
        public void TestInit()
        {
            AppConfigurationManager.Reset();
            TypeResolver.Reset();
            ResourceConnectionProviderRegistry.Reset();
            ResourceConnectionFactory.Reset();
            var settings = new TaskExecutionContextSettings("queue");
            s_Context = new DefaultTaskExecutionContext(settings);

            s_PeekPolicy = new TestPeekPolicy(TimeSpan.FromMilliseconds(10));
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void WithoutResultWorks()
        {
            // --- Arrange
            var processor = new QueuedTaskProcessor<TaskWithCompletedResetEvent, string>(s_Context)
            {
                DoNotPeekWhenCheckingTasks = false,
                MaxDequeuCountBeforeDrop = 5,
                MaxMessagesReadFromQueue = 25,
                PeekPolicy = s_PeekPolicy,
                VisibilityTimeoutInSeconds = 30,
                RequestQueueKey = REQUEST_QUEUE,
                TaskSleepInMilliseconds = 20
            };
            var queue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(REQUEST_QUEUE);
            queue.Clear();
            queue.PutMessage("testmessage1", 70);
            queue.PutMessage("testmsg2", 70);
            TaskWithCompletedResetEvent.DisposeTimes = 0;
            TaskWithCompletedResetEvent.RunTimes = 0;
            TaskWithCompletedResetEvent.SetupTimes = 0;
            s_TasksCompletedAutoResetEvent = new AutoResetEvent(false);
            bool completedSuccessfully;
            using (s_TasksCompletedAutoResetEvent)
            {
                // --- Act
                processor.Start();
                // --- Wait for both tasks to complete (Sets, if the 2. message's dispose is finished)
                completedSuccessfully = s_TasksCompletedAutoResetEvent.WaitOne(10000);
                processor.Stop();
            }

            // ---Assert
            TaskWithCompletedResetEvent.DisposeTimes.ShouldEqual(2);
            TaskWithCompletedResetEvent.RunTimes.ShouldEqual(2);
            TaskWithCompletedResetEvent.SetupTimes.ShouldEqual(2);
            completedSuccessfully.ShouldBeTrue("Tasks did not complete in 10 seconds");
            processor.Dispose();
        }

        [TestMethod]
        public void WithResultWorks()
        {
            // --- Arrange
            var processor = new QueuedTaskProcessor<ResultTaskWithCompletedResetEvent, string, int>(s_Context)
            {
                DoNotPeekWhenCheckingTasks = false,
                MaxDequeuCountBeforeDrop = 5,
                MaxMessagesReadFromQueue = 25,
                PeekPolicy = s_PeekPolicy,
                VisibilityTimeoutInSeconds = 30,
                RequestQueueKey = REQUEST_QUEUE,
                ResponseQueueKey = RESPONSE_QUEUE,
                TaskSleepInMilliseconds = 20
            };
            var queue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(REQUEST_QUEUE);
            var responseQueue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(RESPONSE_QUEUE);
            queue.Clear();
            //responseQueue.Clear();
            queue.PutMessage("testmessage1", 70);
            queue.PutMessage("testmsg2", 70);
            ResultTaskWithCompletedResetEvent.DisposeTimes = 0;
            ResultTaskWithCompletedResetEvent.RunTimes = 0;
            ResultTaskWithCompletedResetEvent.SetupTimes = 0;
            s_TasksCompletedAutoResetEvent = new AutoResetEvent(false);
            bool completedSuccessfully;
            using (s_TasksCompletedAutoResetEvent)
            {
                // --- Act
                processor.Start();
                //wait for both tasks to complete (Sets, if the 2. message's dispose is finished)
                completedSuccessfully = s_TasksCompletedAutoResetEvent.WaitOne(10000);
                processor.Stop();
            }
            // --- Assert

            ResultTaskWithCompletedResetEvent.SetupTimes.ShouldEqual(2);
            ResultTaskWithCompletedResetEvent.RunTimes.ShouldEqual(2);
            ResultTaskWithCompletedResetEvent.DisposeTimes.ShouldEqual(2);
            completedSuccessfully.ShouldBeTrue("Tasks did not complete in 10 seconds");
            ResultTaskWithCompletedResetEvent.ResultDict.ContainsValue("testmessage1").ShouldBeTrue();
            ResultTaskWithCompletedResetEvent.ResultDict.ContainsValue("testmsg2").ShouldBeTrue();
            var messages = responseQueue.GetMessages(2, 40);
            foreach (var message in messages)
            {
                ResultTaskWithCompletedResetEvent.ResultDict[Convert.ToInt32(message.MessageText)]
                    .Length.ShouldEqual(Convert.ToInt32(message.MessageText));
            }
            processor.Dispose();
        }

        [TestMethod]
        public void StopWorks()
        {
            // --- Arrange
            var processor = new QueuedTaskProcessor<TaskWithSetupEvent, string>(s_Context)
            {
                DoNotPeekWhenCheckingTasks = false,
                MaxDequeuCountBeforeDrop = 10,
                MaxMessagesReadFromQueue = 25,
                PeekPolicy = s_PeekPolicy,
                VisibilityTimeoutInSeconds = 30,
                RequestQueueKey = "requestQueue",
                TaskSleepInMilliseconds = 20
            };

            var queue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(REQUEST_QUEUE);
            queue.Clear();
            queue.PutMessage("testmessage1", 70);
            queue.PutMessage("testmsg2", 70);
            TaskWithSetupEvent.DisposeTimes = 0;
            TaskWithSetupEvent.RunTimes = 0;
            TaskWithSetupEvent.SetupTimes = 0;
            s_TaskSetupAutoResetEvent = new AutoResetEvent(false);
            bool completedSuccessfully;
            using (s_TaskSetupAutoResetEvent)
            {
                // --- Act
                processor.Start();
                //wait for the first task to finish setup
                completedSuccessfully = s_TaskSetupAutoResetEvent.WaitOne(10000);
                processor.Stop();
            }
            // --- Assert
            completedSuccessfully.ShouldBeTrue("Tasks did not complete in 10 seconds");
            TaskWithSetupEvent.SetupTimes.ShouldEqual(1, "Setup");
            TaskWithSetupEvent.RunTimes.ShouldEqual(0, "Run");
            TaskWithSetupEvent.DisposeTimes.ShouldEqual(1, "Dispose");
            processor.Dispose();
        }

        [TestMethod]
        public void DefaultPeekPolicyWorksAsExpected()
        {
            // --- Arrange
            var queue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(REQUEST_QUEUE);
            queue.Clear();
            using (var queueProcessor = new TestQueueProcessor<DummyTask>(6))
            {
                // --- Act
                queueProcessor.Start();
                queueProcessor.CheckedGivenTimes.WaitOne(10000);

                // --- Assert
                queueProcessor.PeekTimes.Count.ShouldEqual(6);
                queueProcessor.PeekTimes[1].TotalMilliseconds.ShouldBeBetween(50, 175);
                queueProcessor.PeekTimes[2].TotalMilliseconds.ShouldBeBetween(150, 385);
                queueProcessor.PeekTimes[3].TotalMilliseconds.ShouldBeBetween(350, 500);
                queueProcessor.PeekTimes[4].TotalMilliseconds.ShouldBeBetween(450, 600);
                queueProcessor.PeekTimes[5].TotalMilliseconds.ShouldBeBetween(450, 600);
            }
        }

        [TestMethod]
        public void WithGivenPeekPolicyWorksAsExpected()
        {
            // --- Arrange
            var queue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(REQUEST_QUEUE);
            queue.Clear();
            using (var queueProcessor = new TestQueueProcessor<DummyTask>(6))
            {
                queueProcessor.PeekPolicy = new TestPeekPolicy(TimeSpan.FromMilliseconds(50));
                // --- Act
                queueProcessor.Start();
                queueProcessor.CheckedGivenTimes.WaitOne(10000);

                // --- Assert
                queueProcessor.PeekTimes.Count.ShouldEqual(6);
                queueProcessor.PeekTimes[1].TotalMilliseconds.ShouldBeBetween(20, 100);
                queueProcessor.PeekTimes[2].TotalMilliseconds.ShouldBeBetween(20, 100);
                queueProcessor.PeekTimes[3].TotalMilliseconds.ShouldBeBetween(20, 100);
                queueProcessor.PeekTimes[4].TotalMilliseconds.ShouldBeBetween(20, 100);
                queueProcessor.PeekTimes[5].TotalMilliseconds.ShouldBeBetween(20, 100);
            }
        }

        [TestMethod]
        public void CantGiveHigherDefaultWaitTimeThanMaximum()
        {
            // --- Arrange
            var queue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(REQUEST_QUEUE);
            queue.Clear();
            using (var queueProcessor = new TestQueueProcessor<DummyTask>(3))
            {
                // --- Act
                queueProcessor.TaskSleepInMilliseconds = 5000;

                // --- Assert
                queueProcessor.Start();
                queueProcessor.CheckedGivenTimes.WaitOne(5000);
                queueProcessor.PeekTimes.Count.ShouldEqual(3);
                queueProcessor.PeekTimes[1].TotalMilliseconds.ShouldBeBetween(450, 600);
                queueProcessor.PeekTimes[2].TotalMilliseconds.ShouldBeBetween(450, 600);
            }
        }

        [TestMethod]
        public void StopWithTimeOutDoesNotThrowExeption()
        {
            // --- Arrange
            var queue = ResourceConnectionFactory.CreateResourceConnection<INamedQueue>(REQUEST_QUEUE);
            queue.Clear();
            for (var i = 0; i < 10; i++)
            {
                queue.PutMessage(i.ToString(CultureInfo.InvariantCulture), 5);
            }
            using (var queueProcessor = new TestQueueProcessor<LongDummyTask>(5))
            {
                queueProcessor.StopTimeout = TimeSpan.FromMilliseconds(200);
                using (s_TaskRunningAutoResetEvent = new AutoResetEvent(false))
                {
                    queueProcessor.Start();
                    // --- Act
                    s_TaskRunningAutoResetEvent.WaitOne(4000);
                    queueProcessor.Stop();
                }
            }
        }

        private class DummyTask : TaskBase<string>
        {
            public override void Run()
            {
            }
        }

        private class LongDummyTask : TaskBase<string>
        {
            public override void Run()
            {
                s_TaskRunningAutoResetEvent.Set();
                Thread.Sleep(5000);
            }
        }

        private sealed class TestQueueProcessor<T> : QueuedTaskProcessor<T, string>
            where T : class, ITask<string>, new()
        {
            public readonly Dictionary<int, TimeSpan> PeekTimes = new Dictionary<int, TimeSpan>();
            private int HowManyTimesToCheck { get; set; }
            public readonly AutoResetEvent CheckedGivenTimes = new AutoResetEvent(false);
            private int _checkedTimes;
            private DateTime _lastCheck;

            /// <summary>
            /// Creates a new task processor that is bound to the specified execution
            /// context.
            /// </summary>
            public TestQueueProcessor(int howManyTimesToCheck)
                : base(s_Context)
            {
                DoNotPeekWhenCheckingTasks = false;
                MaxDequeuCountBeforeDrop = 2;
                MaxMessagesReadFromQueue = 5;
                VisibilityTimeoutInSeconds = 100;
                RequestQueueKey = REQUEST_QUEUE;
                TaskSleepInMilliseconds = 100;
                HowManyTimesToCheck = howManyTimesToCheck;
                MaxPeekWaitTime = TimeSpan.FromMilliseconds(500);
                _lastCheck = DateTime.Now;
                _checkedTimes = 0;
            }

            public override void Dispose()
            {
                CheckedGivenTimes.Dispose();
                PeekTimes.Clear();
                base.Dispose();
            }

            protected override bool HasAnyTaskToExecute()
            {
                var result = base.HasAnyTaskToExecute();
                PeekTimes.Add(_checkedTimes, DateTime.Now.Subtract(_lastCheck));
                _lastCheck = DateTime.Now;
                _checkedTimes++;
                if (_checkedTimes == HowManyTimesToCheck)
                {
                    CheckedGivenTimes.Set();
                    Stop();
                }
                return result;
            }

            public override void Start()
            {
                _checkedTimes = 0;
                PeekTimes.Clear();
                base.Start();
            }
        }

        private class TestPeekPolicy : IQueuePeekPolicy
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            private DateTime LastTime { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            private TimeSpan Interval { get; set; }

            public TestPeekPolicy(TimeSpan interval)
            {
                Interval = interval;
                LastTime = DateTime.Now;
            }

            public void SetTaskExecutionContext(ITaskExecutionContext context)
            {
            }

            public int NextPeekTimeInMilliseconds()
            {
                return (int)Interval.TotalMilliseconds;
            }
        }

        private class ResultTaskWithCompletedResetEvent : TaskBase<string, int>
        {
            public static int DisposeTimes { get; set; }
            public static int SetupTimes { get; set; }
            public static int RunTimes { get; set; }
            public static readonly Dictionary<int, string> ResultDict = new Dictionary<int, string>();
            private readonly object _locker = new object();

            public override void Dispose()
            {
                lock (_locker)
                {
                    DisposeTimes++;
                    base.Dispose();
                    if (DisposeTimes == 2)
                        s_TasksCompletedAutoResetEvent.Set();
                }
            }

            public override void Setup(ITaskExecutionContext context)
            {
                lock (_locker) SetupTimes++;
            }

            public override void Run()
            {
                lock (_locker)
                {
                    RunTimes++;
                    Result = Argument.Length;
                    ResultDict.Add(Result, Argument);
                }
            }
        }

        private class TaskWithCompletedResetEvent : TaskBase<string>
        {
            public static int DisposeTimes { get; set; }
            public static int SetupTimes { get; set; }
            public static int RunTimes { get; set; }
            private readonly object _locker = new object();

            public override void Dispose()
            {
                lock (_locker)
                {
                    DisposeTimes++;
                    base.Dispose();
                    if (DisposeTimes == 2)
                        s_TasksCompletedAutoResetEvent.Set();
                }
            }

            public override void Setup(ITaskExecutionContext context)
            {
                lock (_locker) SetupTimes++;
                Thread.Sleep(200);
            }

            public override void Run()
            {
                lock (_locker) RunTimes++;
            }
        }

        private class TaskWithSetupEvent : TaskBase<string>
        {
            public static int DisposeTimes { get; set; }
            public static int SetupTimes { get; set; }
            public static int RunTimes { get; set; }
            private readonly object _locker = new object();

            public override void Dispose()
            {
                lock (_locker) DisposeTimes++;
                base.Dispose();
            }

            public override void Setup(ITaskExecutionContext context)
            {
                lock (_locker) SetupTimes++;
                s_TaskSetupAutoResetEvent.Set();
                Thread.Sleep(1000);
            }

            public override void Run()
            {
                lock (_locker) RunTimes++;
            }
        }
    }
}
