using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Queue;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Queue
{
    [TestClass]
    public class MemoryNamedQueueProviderTest
    {
        [TestMethod]
        public void GetQueueWorksAsExpected()
        {
            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            testProvider.CreateQueue("queue1");

            // --- Act
            var queue1 = testProvider.GetQueue("queue1");
            var queue2 = testProvider.GetQueue("queue2");

            // --- Assert
            queue1.ShouldNotBeNull();
            queue2.ShouldBeNull();
        }

        [TestMethod]
        public void ListQueuesWorksAsExpected()
        {
            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            testProvider.CreateQueue("queue1");
            testProvider.CreateQueue("queue2");

            // --- Act
            var queues = testProvider.ListQueues();

            // --- Assert
            // ReSharper disable PossibleMultipleEnumeration
            queues.ShouldHaveCountOf(2);
            queues.Select(q => q.Name).ShouldContain("queue1");
            queues.Select(q => q.Name).ShouldContain("queue2");
            // ReSharper restore PossibleMultipleEnumeration
        }

        [TestMethod]
        public void DeleteQueueWorksAsExpected()
        {
            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            testProvider.CreateQueue("queue1");

            // --- Act
            var count1 = testProvider.ListQueues().Count();
            testProvider.DeleteQueue("queue2");
            var count2 = testProvider.ListQueues().Count();
            testProvider.DeleteQueue("queue1");
            var count3 = testProvider.ListQueues().Count();

            // --- Assert
            count1.ShouldEqual(1);
            count2.ShouldEqual(1);
            count3.ShouldEqual(0);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateSameQueueNameParalellFails()
        {
            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            const int TO = 5;

            // --- Act
            try
            {
                Parallel.For(0, TO, i => testProvider.CreateQueue("testqueue"));
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
            // --- Assert

        }

        [TestMethod]
        public void GetQueueInParalellWorks()
        {
            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            testProvider.CreateQueue("name1");
            testProvider.CreateQueue("name2");
            testProvider.CreateQueue("name3");
            var testQueueArray = new MemoryNamedQueue[2];
            const int TO = 2;
            var expected = (MemoryNamedQueue)testProvider.GetQueue("name2");

            // ---- Act
            Parallel.For(0, TO, i => testQueueArray[i] = (MemoryNamedQueue)testProvider.GetQueue("name2"));

            // --- Assert
            testQueueArray[0].ShouldNotBeNull();
            testQueueArray[1].ShouldNotBeNull();
            testQueueArray[0].Name.ShouldEqual(expected.Name);
            testQueueArray[1].Name.ShouldEqual(expected.Name);

        }

        [TestMethod]
        public void DeleteQueueWorks()
        {

            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            testProvider.CreateQueue("name");

            // --- Act
            testProvider.DeleteQueue("name");

            // --- Assert
            testProvider.CreateQueue("name");
            var resultQueue = testProvider.GetQueue("name");
            resultQueue.ShouldNotBeNull();
            resultQueue.Name.ShouldEqual("name");
            resultQueue.PeekMessages(2).Count().ShouldEqual(0);

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AccessToQueueAfterDeletionFails()
        {
            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            var accesstester = testProvider.CreateQueue("TestQueueName");
            accesstester.PutMessage("content", 50);

            // --- Act
            testProvider.DeleteQueue("TestQueueName");

            // --- Assert
            accesstester.GetMessages(1, 50);
        }

        [TestMethod]
        public void SameNamedQueuesAreTheSame()
        {
            // --- Arrange
            var testProvider = new MemoryNamedQueueProvider();
            var accesstester = testProvider.CreateQueue("name");
            var accesstester2 = testProvider.GetQueue("name");
            accesstester.PutMessage("content", 50);

            // --- Act
            var result = accesstester2.GetMessages(1, 50).ToList();

            // --- Assert
            result.Count.ShouldEqual(1);
            var resultMessage = result.First();
            resultMessage.MessageText.ShouldEqual("content");
        }
    }
}
