using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Queue;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Queue
{
    [TestClass]
    public class MemoryNamedQueueTest
    {
        [TestMethod]
        public void GetMessageInParallelWorks()
        {
            // --- Arrange
            var testQueue = new MemoryNamedQueue("name");
            testQueue.PutMessage("content1", 30);
            testQueue.PutMessage("content2", 30);
            testQueue.PutMessage("content3", 30);
            const int TO = 10;
            var result = new IEnumerable<IPoppedMessage>[TO];
            var expected = testQueue.PeekMessages(3);
            var expIdList = expected.Select(queuedMessage => queuedMessage.Id).ToList();

            // --- Act
            Parallel.For(0, TO, i => result[i] = testQueue.GetMessages(5, 20));

            // --- Assert
            var resultIdList = (
                from poppedMessage in result
                from message in poppedMessage
                select message.Id).ToList();

            foreach (var expId in expIdList)
            {
                var got = resultIdList.Count(resultId => expId == resultId);
                got.ShouldEqual(1);
            }
        }

        [TestMethod]
        public void DeleteGotMessageOnSameThreadWorks()
        {
            // --- Arrange
            var testQueue = new MemoryNamedQueue("name");
            testQueue.PutMessage("content", 30);

            // --- Act

            var gotmsg = testQueue.GetMessages(1, 30).ToArray()[0];
            testQueue.DeleteMessage(gotmsg);

            // --- Assert
            testQueue.PeekMessages(3).Count().ShouldEqual(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteGotMessageOnDifferentThreadsFails()
        {
            // --- Arrange
            var testQueue = new MemoryNamedQueue("name");
            testQueue.PutMessage("content", 30);
            var getTask = new Task(() =>
            {
                Thread.Sleep(1050);
                testQueue.GetMessages(1, 20);
            });

            // --- Act
            var msg = testQueue.GetMessages(1, 1).ToArray()[0];
            getTask.Start();
            Thread.Sleep(1200);
            testQueue.DeleteMessage(msg);

            // --- Assert
            getTask.Wait();
        }

        [TestMethod]
        public void GetMessageAfterTimeOutWorks()
        {
            // --- Arrange
            var testQueue = new MemoryNamedQueue("name");
            testQueue.PutMessage("content", 30);
            var expectedMessage = testQueue.GetMessages(1, 0).First();
            Thread.Sleep(500);
            var getTask = new Task(() =>
            {
                var tresult = testQueue.GetMessages(1, 1);
                tresult.Count().ShouldEqual(1);
            });


            // --- Act
            getTask.Start();
            Thread.Sleep(100);
            var result1 = testQueue.GetMessages(1, 20).ToList();
            Thread.Sleep(1100);
            var result2 = testQueue.GetMessages(1, 20).ToList();

            // --- Assert
            result1.Count.ShouldEqual(0);
            result2.Count.ShouldEqual(1);
            var resultMessage = result2.First();
            resultMessage.MessageText.ShouldEqual(expectedMessage.MessageText);
            resultMessage.PopReceipt.ShouldNotEqual(expectedMessage.PopReceipt);
            resultMessage.InsertionTime.ShouldEqual(expectedMessage.InsertionTime);
            resultMessage.ExpirationTime.ShouldEqual(expectedMessage.ExpirationTime);
            resultMessage.DequeueCount.ShouldEqual(expectedMessage.DequeueCount + 2);
            resultMessage.Id.ShouldEqual(expectedMessage.Id);
            getTask.Wait();
        }

        [TestMethod]
        public void PeekMessageWithTimeOutWorks()
        {
            // --- Arrange
            var testQueue = new MemoryNamedQueue("name");
            testQueue.PutMessage("content", 10);
            testQueue.GetMessages(1, 5);

            // --- Act
            var expectedMessage = testQueue.PeekMessages(1);

            // --- Assert
            expectedMessage.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void GetApproximateMessageCountWorks()
        {
            // --- Arrange
            var testQueue = new MemoryNamedQueue("name");
            testQueue.PutMessage("content1", 30);
            testQueue.PutMessage("content2", 30);
            testQueue.PutMessage("content3", 30);

            // --- Act
            var count1 = testQueue.GetApproximateMessageCount();
            var messages = testQueue.GetMessages(2, 30).ToList();
            var count2 = testQueue.GetApproximateMessageCount();
            // ReSharper disable PossibleMultipleEnumeration
            testQueue.DeleteMessage(messages[0]);
            testQueue.DeleteMessage(messages[1]);
            // ReSharper restore PossibleMultipleEnumeration
            var count3 = testQueue.GetApproximateMessageCount();

            // --- Assert
            count1.ShouldEqual(3);
            count2.ShouldEqual(3);
            count3.ShouldEqual(1);
        }
    }
}
