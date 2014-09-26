using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Tracing;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests
{
    [TestClass]
    public class FileTraceLoggerTest
    {
        private const string LOG_ROOT = @"C:\Temp";
        private const string LOG_FILE = @"log.txt";

        [TestInitialize]
        public void Initialize()
        {
            if (!Directory.Exists(LOG_ROOT))
            {
                Directory.CreateDirectory(LOG_ROOT);
            }
            File.Delete(Path.Combine(LOG_ROOT, LOG_FILE));
        }

        [TestMethod]
        public void SingleScenarioWorks()
        {
            // --- Arrange
            var tracer = new FileTraceLogger(LOG_FILE, LOG_ROOT, flushAfter: 3);
            var traceItem = new TraceLogItem
            {
                TimestampUtc = new DateTime(2012, 1, 1, 8, 0, 0),
                Type = TraceLogItemType.Informational,
                ServerName = "Server",
                ThreadId = 123,
                OperationType = "TestOp",
                Message = "Message"
            };

            // --- Act
            for (var i = 0; i < 5; i++) tracer.Log(traceItem);
            tracer.Dispose();

            // --- Assert
            var text = File.ReadAllText(Path.Combine(LOG_ROOT, LOG_FILE));
            var lines = text.Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                line.ShouldEqual("2012.01.01. 8:00:00\tInformational\tServer\t123\tTestOp\tMessage\t");
            }
            lines.ShouldHaveCountOf(5);
        }

        [TestMethod]
        public void ComplexScenarioWorks()
        {
            // --- Arrange
            if (File.Exists(Path.Combine(LOG_ROOT, "08", LOG_FILE)))
            {
                File.Delete(Path.Combine(LOG_ROOT, "08", LOG_FILE));
            }
            if (File.Exists(Path.Combine(LOG_ROOT, "09", LOG_FILE)))
            {
                File.Delete(Path.Combine(LOG_ROOT, "09", LOG_FILE));
            }
            var tracer = new FileTraceLogger(LOG_FILE, LOG_ROOT, "hh", flushAfter: 3);
            var traceItem1 = new TraceLogItem
            {
                // 8 o'clock
                TimestampUtc = new DateTime(2012, 1, 1, 8, 0, 0),
                Type = TraceLogItemType.Informational,
                ServerName = "Server",
                ThreadId = 123,
                OperationType = "TestOp",
                Message = "Message"
            };
            var traceItem2 = new TraceLogItem
            {
                // 9 o'clock
                TimestampUtc = new DateTime(2012, 1, 1, 9, 0, 0),
                Type = TraceLogItemType.Informational,
                ServerName = "Server",
                ThreadId = 123,
                OperationType = "TestOp",
                Message = "Message"
            };

            // --- Act
            for (var i = 0; i < 3; i++)
            {
                tracer.Log(traceItem1);
                tracer.Log(traceItem2);
                tracer.Log(traceItem1);
                tracer.Log(traceItem1);
            }
            tracer.Dispose();

            // --- Assert
            var text1 = File.ReadAllText(Path.Combine(LOG_ROOT, "08", LOG_FILE));
            var lines1 = text1.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            lines1.ShouldHaveCountOf(9);
            foreach (var line in lines1)
            {
                line.ShouldEqual("2012.01.01. 8:00:00\tInformational\tServer\t123\tTestOp\tMessage\t");
            }
            var text2 = File.ReadAllText(Path.Combine(LOG_ROOT, "09", LOG_FILE));
            var lines2 = text2.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            lines2.ShouldHaveCountOf(3);
            foreach (var line in lines2)
            {
                line.ShouldEqual("2012.01.01. 9:00:00\tInformational\tServer\t123\tTestOp\tMessage\t");
            }
        }

    }
}
