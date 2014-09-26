using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.Tracing;
using Seemplest.Core.WindowsEventLog;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.WindowsEventLog
{
    [TestClass]
    public class WindowsEventLogTest
    {
        private const string SEEMPLEST_LOG = "S_Log";
        private const string SEEMPLEST_LOG2 = "S_Log2";
        private const string SEEMPLEST_SOURCE = "Seemplest Source";

        private const string LOG_ROOT = @"C:\Temp";
        private const string LOG_FILE = @"log.txt";

        [TestInitialize]
        public void TestInit()
        {
            if (EventLog.Exists(SEEMPLEST_LOG)) EventLog.Delete(SEEMPLEST_LOG);
            if (EventLog.Exists(SEEMPLEST_LOG2)) EventLog.Delete(SEEMPLEST_LOG2);
            var configSettings = new AppConfigurationSettings(typeof (AppConfigProvider));
            AppConfigurationManager.Configure(configSettings);
            WindowsEventLogger.LogSourceMapper = new DefaultLogSourceNameMapper();
            WindowsEventLogger.Reset();
        }

        [TestMethod]
        public void WithNameAttributeWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithStringNameAttribute>("Message");

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.ShouldEqual("Message");
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void LogWithDefaultMessageWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithStringNameAttribute>();

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.ShouldEqual("Default message");
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void LogWithExceptionWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithStringNameAttribute>(new NullReferenceException());

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.StartsWith("An unexcepted exception").ShouldBeTrue();
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void LogWithLongMessageWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithStringNameAttribute>("Haho".PadRight(100000, '.'));

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.StartsWith("Haho...").ShouldBeTrue();
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void LogWithLongMessageAndExceptionWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithStringNameAttribute>("Haho".PadRight(100000, '.'), new NullReferenceException());

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.StartsWith("Haho...").ShouldBeTrue();
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void WithTypeAttributeWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithTypeNameAttribute>("Message");

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.ShouldEqual("Message");
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void LongMessageWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithStringNameAttribute>("Message".PadRight(40000));

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.Trim().ShouldEqual("Message");
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void WithInstancePrefixWorksAsExpected()
        {
            // --- Arrange
            var configSettings = new AppConfigurationSettings(
                typeof(AppConfigProvider), null, null, "TestInstancePrefix", "TestInstanceName");
            AppConfigurationManager.Configure(configSettings);
            WindowsEventLogger.LogSourceMapper = new DefaultLogSourceNameMapper();
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithStringNameAttribute>("Message");

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.ShouldEqual("Message");
            lastentry.Source.ShouldEqual("TestInstancePrefix" + SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void WithoutTypeAttributeWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithoutTypeAttribute>("Message");

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.ShouldEqual("Message");
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void OnlyWithRequiredAttributesWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.Log<WithOnlyRequiredAttributes>("Message");

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(0)");
            lastentry.InstanceId.ShouldEqual(0);
            lastentry.Message.ShouldEqual("Message");
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Warning);
            eventLog.Dispose();
        }

        [TestMethod]
        public void ExceptionLoggingWorksAsExpected()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);
            const string TEST_EX = "This is a test exception";
            const string TEST_INNER_EX = "This is an inner test exception";

            // --- Act
            var innerEx = new InvalidOperationException(TEST_EX);
            var ex = new InvalidOperationException(TEST_INNER_EX, innerEx);

            WindowsEventLogger.Log<WithExceptionAttributes>("Message:", ex);

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(0)");
            lastentry.InstanceId.ShouldEqual(0);
            lastentry.Message.StartsWith("Message:").ShouldBeTrue();
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Error);
            eventLog.Dispose();
        }

        [TestMethod]
        public void WorksInParalell()
        {
            // --- Arrange
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            Parallel.For(0, 40,
                num => WindowsEventLogger.Log<WithStringNameAttribute>(String.Format("Message{0}", num)));

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(40);
            var messagelist = new List<string>();
            foreach (EventLogEntry lastentry in after)
            {
                lastentry.Category.ShouldEqual("(5)");
                lastentry.InstanceId.ShouldEqual(3);
                messagelist.Add(lastentry.Message);
                lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
                lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            }
            for (int i = 0; i < 40; i++)
            {
                messagelist.Contains(String.Format("Message{0}", i)).ShouldBeTrue(
                    String.Format("Message{0} was not found", i));
            }
            eventLog.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void WithoutSourceAttributeFails()
        {
            try
            {
                WindowsEventLogger.Log<WrongDefinition1>("message");
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void WithoutNameAttributeFails()
        {
            try
            {
                WindowsEventLogger.Log<WrongDefinition2>("message");
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithWrongNameAttributeFails()
        {
            try
            {
                WindowsEventLogger.Log<WrongDefinition3>("message");
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EventLogNameBaseWithoutNameAttributeFails()
        {
            try
            {
                WindowsEventLogger.Log<WrongDefinition4>("message");
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException.InnerException;
            }
        }

        [TestMethod]
        public void DefaultLogNameMapperWorksAsExpected()
        {
            // --- Arrange
            var mapper1 = new DefaultLogNameMapper();
            var mapper2 = new DefaultLogNameMapper("Dummy");

            // --- Act
            var name11 = mapper1.Map(WindowsEventLogger.DEFAULT_APP_LOG);
            var name12 = mapper1.Map("Hi");
            var name21 = mapper2.Map(WindowsEventLogger.DEFAULT_APP_LOG);
            var name22 = mapper2.Map("Hello");

            // --- Assert
            name11.ShouldEqual("Application");
            name12.ShouldEqual("Hi");
            name21.ShouldEqual("Dummy");
            name22.ShouldEqual("Hello");
        }

        [TestMethod]
        public void DefaultLogSourceMapperWorksAsExpected()
        {
            // --- Arrange
            var configSettings = new AppConfigurationSettings(
                typeof(AppConfigProvider), null, null, "TestInstancePrefix", "TestInstanceName");
            AppConfigurationManager.Configure(configSettings);
            var mapper1 = new DefaultLogSourceNameMapper();
            configSettings = new AppConfigurationSettings(typeof(AppConfigProvider));
            AppConfigurationManager.Configure(configSettings);
            var mapper2 = new DefaultLogSourceNameMapper();
            var mapper3 = new DefaultLogSourceNameMapper("Dummy");

            // --- Act
            var name1 = mapper1.Map("Hi");
            var name2 = mapper2.Map("Hello");
            var name3 = mapper3.Map("Howdy");

            // --- Assert
            name1.ShouldEqual("TestInstancePrefixHi");
            name2.ShouldEqual("Hello");
            name3.ShouldEqual("DummyHowdy");
        }

        [TestMethod]
        public void LogWithRedirectionWorksAsExpected()
        {
            // --- Arrange
            if (!Directory.Exists(LOG_ROOT))
            {
                Directory.CreateDirectory(LOG_ROOT);
            }
            File.Delete(Path.Combine(LOG_ROOT, LOG_FILE));
            var tracer = new FileTraceLogger(LOG_FILE, LOG_ROOT, flushAfter: 1);
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.RedirectLogTo(tracer);
            WindowsEventLogger.Log<WithStringNameAttribute>();

            // --- Assert
            EventLog.Exists(SEEMPLEST_LOG2).ShouldBeFalse();
            eventLog.Dispose();

            var text = File.ReadAllText(Path.Combine(LOG_ROOT, LOG_FILE));
            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            lines.ShouldHaveCountOf(1);
            var parts = lines[0].Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            parts[1].ShouldEqual("Informational");
            parts[4].ShouldEqual("Windows Event Trace");
            parts[5].ShouldEqual("Default message");
            parts[6].ShouldEqual("EventId: 3, CategoryId: 5");
        }

        [TestMethod]
        public void LogAfterResetWorksAsExpected()
        {
            // --- Arrange
            if (!Directory.Exists(LOG_ROOT))
            {
                Directory.CreateDirectory(LOG_ROOT);
            }
            File.Delete(Path.Combine(LOG_ROOT, LOG_FILE));
            var tracer = new FileTraceLogger(LOG_FILE, LOG_ROOT, flushAfter: 1);
            var eventLog = new EventLog(SEEMPLEST_LOG2);
            WindowsEventLogger.RedirectLogTo(tracer);
            WindowsEventLogger.Log<WithStringNameAttribute>();

            // --- Act
            WindowsEventLogger.Reset();
            WindowsEventLogger.Log<WithStringNameAttribute>();

            // --- Assert
            var after = eventLog.Entries;
            var afterCount = after.Count;
            afterCount.ShouldEqual(1);
            var lastentry = after[after.Count - 1];
            lastentry.Category.ShouldEqual("(5)");
            lastentry.InstanceId.ShouldEqual(3);
            lastentry.Message.ShouldEqual("Default message");
            lastentry.Source.ShouldEqual(SEEMPLEST_SOURCE);
            lastentry.EntryType.ShouldEqual(EventLogEntryType.Information);
            eventLog.Dispose();
        }

        [TestMethod]
        public void LogRedirectionConvertsTypesAsExpected()
        {
            // --- Arrange
            if (!Directory.Exists(LOG_ROOT))
            {
                Directory.CreateDirectory(LOG_ROOT);
            }
            File.Delete(Path.Combine(LOG_ROOT, LOG_FILE));
            var tracer = new FileTraceLogger(LOG_FILE, LOG_ROOT, flushAfter: 1);
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.RedirectLogTo(tracer);
            WindowsEventLogger.Log<SampleError>();
            WindowsEventLogger.Log<SampleWarning>();
            WindowsEventLogger.Log<SampleInformation>();
            WindowsEventLogger.Log<SampleFailureAudit>();
            WindowsEventLogger.Log<SampleSuccessAudit>();

            // --- Assert
            EventLog.Exists(SEEMPLEST_LOG2).ShouldBeFalse();
            eventLog.Dispose();

            var text = File.ReadAllText(Path.Combine(LOG_ROOT, LOG_FILE));
            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            lines.ShouldHaveCountOf(5);
            var expectedCats = new[] {"Error", "Warning", "Informational", "Error", "Success"};
            for (var i = 0; i < 5; i++)
            {
                var parts = lines[i].Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                parts[1].ShouldEqual(expectedCats[i]);
            }
        }

        [TestMethod]
        public void LogWithRedirectionWorksWithExceptionMessage()
        {
            // --- Arrange
            if (!Directory.Exists(LOG_ROOT))
            {
                Directory.CreateDirectory(LOG_ROOT);
            }
            File.Delete(Path.Combine(LOG_ROOT, LOG_FILE));
            var tracer = new FileTraceLogger(LOG_FILE, LOG_ROOT, flushAfter: 1);
            var eventLog = new EventLog(SEEMPLEST_LOG2);

            // --- Act
            WindowsEventLogger.RedirectLogTo(tracer);
            WindowsEventLogger.Log<SampleError>(new NullReferenceException());

            // --- Assert
            EventLog.Exists(SEEMPLEST_LOG2).ShouldBeFalse();
            eventLog.Dispose();

            var text = File.ReadAllText(Path.Combine(LOG_ROOT, LOG_FILE));
            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            lines.ShouldHaveCountOf(3);
            var parts = lines[0].Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            parts[1].ShouldEqual("Error");
        }

        [EventLogName(SEEMPLEST_LOG2)]
        public class MyLogName: EventLogNameBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.Information)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [EventMessage("Default message")]
        [SkipLogSourceInstallation]
        public class WithStringNameAttribute : LogEventBase
        { }

        [EventLogName(typeof(MyLogName))]
        [EventType(EventLogEntryType.Information)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [SkipLogSourceInstallation]
        public class WithTypeNameAttribute : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [SkipLogSourceInstallation]
        public class WithoutTypeAttribute : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.Warning)]
        [EventSource(SEEMPLEST_SOURCE)]
        [SkipLogSourceInstallation]
        public class WithOnlyRequiredAttributes : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.Error)]
        [EventSource(SEEMPLEST_SOURCE)]
        [SkipLogSourceInstallation]
        public class WithExceptionAttributes : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG)]
        [EventId(3)]
        [EventCategoryId(5)]
        [SkipLogSourceInstallation]
        public class WrongDefinition1 : LogEventBase
        { }

        [EventType(EventLogEntryType.Information)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [SkipLogSourceInstallation]
        public class WrongDefinition2 : LogEventBase
        { }

        [EventLogName(typeof(int))]
        [EventType(EventLogEntryType.Information)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [SkipLogSourceInstallation]
        public class WrongDefinition3 : LogEventBase
        { }

        public class MyWrongLogName : EventLogNameBase
        { }

        [EventLogName(typeof(MyWrongLogName))]
        [EventType(EventLogEntryType.Information)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [SkipLogSourceInstallation]
        public class WrongDefinition4 : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.Error)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [EventMessage("Default message")]
        [SkipLogSourceInstallation]
        public class SampleError : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.Warning)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [EventMessage("Default message")]
        [SkipLogSourceInstallation]
        public class SampleWarning : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.FailureAudit)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [EventMessage("Default message")]
        [SkipLogSourceInstallation]
        public class SampleFailureAudit : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.Information)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [EventMessage("Default message")]
        [SkipLogSourceInstallation]
        public class SampleInformation : LogEventBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        [EventType(EventLogEntryType.SuccessAudit)]
        [EventSource(SEEMPLEST_SOURCE)]
        [EventId(3)]
        [EventCategoryId(5)]
        [EventMessage("Default message")]
        [SkipLogSourceInstallation]
        public class SampleSuccessAudit : LogEventBase
        { }
    }
}
