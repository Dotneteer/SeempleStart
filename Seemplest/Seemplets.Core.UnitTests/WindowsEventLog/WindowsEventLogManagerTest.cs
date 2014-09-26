using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Common;
using Seemplest.Core.WindowsEventLog;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.WindowsEventLog
{
    [TestClass]
    public class WindowsEventLogManagerTest
    {
        private const string SEEMPLEST_LOG = "S_Log";
        private const string SEEMPLEST_LOG2 = "S_Log2";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstallFailsWithNullData()
        {
            // --- Act
            WindowsEventLogManager.InstallEventLogSources(null);
        }

        [TestMethod]
        public void InstallWorksAsExpected()
        {
            // --- Arrange
            const string LOG1 = "_myLog1";
            const string LOG2 = "_myLog2";
            if (EventLog.Exists(LOG1)) EventLog.Delete(LOG1);
            if (EventLog.Exists(LOG2)) EventLog.Delete(LOG2);
            var data = new EventLogCreationData();
            data.Add(LOG1, "Source1");
            data.Add(LOG1, "Source2");
            data.Add(LOG2, "Source3");
            data.Add(LOG2, "Source4");
            
            // --- Act
            var result = WindowsEventLogManager.InstallEventLogSources(data);

            // --- Assert
            result.Errors.ShouldHaveCountOf(0);
            result.AffectedSources.ShouldHaveCountOf(2);
            result.AffectedSources[LOG1].ShouldHaveCountOf(2);
            result.AffectedSources[LOG1].ShouldContain("Source1");
            result.AffectedSources[LOG1].ShouldContain("Source2");
            result.AffectedSources[LOG2].ShouldHaveCountOf(2);
            result.AffectedSources[LOG2].ShouldContain("Source3");
            result.AffectedSources[LOG2].ShouldContain("Source4");

            EventLog.LogNameFromSourceName("Source1", ".").ShouldEqual(LOG1);
            EventLog.LogNameFromSourceName("Source2", ".").ShouldEqual(LOG1);
            EventLog.LogNameFromSourceName("Source3", ".").ShouldEqual(LOG2);
            EventLog.LogNameFromSourceName("Source4", ".").ShouldEqual(LOG2);
        }

        [TestMethod]
        public void InstallWorksAsExpectedWithMappers()
        {
            // --- Arrange
            const string LOG1 = "_myLog1";
            const string LOG2 = "_myLog2";
            if (EventLog.Exists(LOG1 + "L")) EventLog.Delete(LOG1);
            if (EventLog.Exists(LOG2 + "L")) EventLog.Delete(LOG2);
            var data = new EventLogCreationData();
            var mapper = new DummyNameMapper();
            data.Add(LOG1, "Source1", mapper, mapper);
            data.Add(LOG1, "Source2", mapper, mapper);
            data.Add(LOG2, "Source3", mapper, mapper);
            data.Add(LOG2, "Source4", mapper, mapper);

            // --- Act
            var result = WindowsEventLogManager.InstallEventLogSources(data);

            // --- Assert
            result.Errors.ShouldHaveCountOf(0);
            result.AffectedSources.ShouldHaveCountOf(2);
            result.AffectedSources[LOG1 + "L"].ShouldHaveCountOf(2);
            result.AffectedSources[LOG1 + "L"].ShouldContain("Source1L");
            result.AffectedSources[LOG1 + "L"].ShouldContain("Source2L");
            result.AffectedSources[LOG2 + "L"].ShouldHaveCountOf(2);
            result.AffectedSources[LOG2 + "L"].ShouldContain("Source3L");
            result.AffectedSources[LOG2 + "L"].ShouldContain("Source4L");

            EventLog.LogNameFromSourceName("Source1L", ".").ShouldEqual(LOG1 + "L");
            EventLog.LogNameFromSourceName("Source2L", ".").ShouldEqual(LOG1 + "L");
            EventLog.LogNameFromSourceName("Source3L", ".").ShouldEqual(LOG2 + "L");
            EventLog.LogNameFromSourceName("Source4L", ".").ShouldEqual(LOG2 + "L");
        }

        [TestMethod]
        public void InstallOverrridesExistingSource()
        {
            // --- Arrange
            const string LOG1 = "_myLog1";
            const string LOG2 = "_myLog2";
            if (EventLog.Exists(LOG1)) EventLog.Delete(LOG1);
            if (EventLog.Exists(LOG2)) EventLog.Delete(LOG2);
            var data = new EventLogCreationData();
            data.Add(LOG1, "Source1");
            data.Add(LOG1, "Source2");
            data.Add(LOG2, "Source3");
            data.Add(LOG2, "Source1"); // --- Overrides LOG1

            // --- Act
            var result = WindowsEventLogManager.InstallEventLogSources(data);

            // --- Assert
            result.Errors.ShouldHaveCountOf(0);
            result.AffectedSources.ShouldHaveCountOf(2);
            result.AffectedSources[LOG1].ShouldHaveCountOf(2);
            result.AffectedSources[LOG1].ShouldContain("Source1");
            result.AffectedSources[LOG1].ShouldContain("Source2");
            result.AffectedSources[LOG2].ShouldHaveCountOf(2);
            result.AffectedSources[LOG2].ShouldContain("Source3");
            result.AffectedSources[LOG2].ShouldContain("Source1");

            EventLog.LogNameFromSourceName("Source1", ".").ShouldEqual(LOG2);
            EventLog.LogNameFromSourceName("Source2", ".").ShouldEqual(LOG1);
            EventLog.LogNameFromSourceName("Source3", ".").ShouldEqual(LOG2);
        }

        [TestMethod]
        public void InstallWorksFromAssemblyExpected()
        {
            // --- Arrange
            if (EventLog.Exists(SEEMPLEST_LOG)) EventLog.Delete(SEEMPLEST_LOG);
            if (EventLog.Exists(SEEMPLEST_LOG2)) EventLog.Delete(SEEMPLEST_LOG2);
            var data = new EventLogCreationData();
            data.Clear();
            data.MergeSourcesFromAssembly(Assembly.GetExecutingAssembly());

            // --- Act
            var result = WindowsEventLogManager.InstallEventLogSources(data);

            // --- Assert
            result.Errors.ShouldHaveCountOf(0);
            result.AffectedSources.ShouldHaveCountOf(2);
            result.AffectedSources[SEEMPLEST_LOG].ShouldHaveCountOf(2);
            result.AffectedSources[SEEMPLEST_LOG].ShouldContain("Source1");
            result.AffectedSources[SEEMPLEST_LOG].ShouldContain("Source2");
            result.AffectedSources[SEEMPLEST_LOG2].ShouldHaveCountOf(1);
            result.AffectedSources[SEEMPLEST_LOG2].ShouldContain("Source3");

            EventLog.LogNameFromSourceName("Source1", ".").ShouldEqual(SEEMPLEST_LOG);
            EventLog.LogNameFromSourceName("Source2", ".").ShouldEqual(SEEMPLEST_LOG);
            EventLog.LogNameFromSourceName("Source3", ".").ShouldEqual(SEEMPLEST_LOG2);
        }

        [TestMethod]
        public void InstallSignsSourceIssues()
        {
            // --- Arrange
            const string LOG1 = "My very long name 1";
            const string LOG2 = "My very long name 2";
            if (EventLog.Exists(LOG1)) EventLog.Delete(LOG1);
            var data = new EventLogCreationData();
            data.Add(LOG1, "Source1");
            data.Add(LOG2, "Source2");

            // --- Act
            var result = WindowsEventLogManager.InstallEventLogSources(data);
            if (EventLog.Exists(LOG1)) EventLog.Delete(LOG1);
            if (EventLog.Exists(LOG2)) EventLog.Delete(LOG2);

            // --- Assert
            result.Errors.ShouldHaveCountOf(1);
        }

        [TestMethod]
        public void InstallSignsSourceIssues2()
        {
            // --- Arrange
            const string LOG1 = null;
            // ReSharper disable AssignNullToNotNullAttribute
            if (EventLog.Exists(LOG1)) EventLog.Delete(LOG1);
            // ReSharper restore AssignNullToNotNullAttribute
            var data = new EventLogCreationData();
            data.Add(LOG1, "Source1");

            // --- Act
            var result = WindowsEventLogManager.InstallEventLogSources(data);
            // ReSharper disable AssignNullToNotNullAttribute
            if (EventLog.Exists(LOG1)) EventLog.Delete(LOG1);
            // ReSharper restore AssignNullToNotNullAttribute

            // --- Assert
            result.Errors.ShouldHaveCountOf(1);
            Console.WriteLine(result.Errors.First().Value.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EventLogCreationDataAddFailesWithWrongType()
        {
            // --- Arrange
            var data = new EventLogCreationData();

            // --- Act
            data.Add(typeof(int));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFailsWithNullData()
        {
            // --- Act
            WindowsEventLogManager.RemoveEventLogSources(null);
        }

        [TestMethod]
        public void RemoveWorksAsExpected()
        {
            // --- Arrange
            InstallWorksAsExpected();
            const string LOG1 = "_myLog1";
            const string LOG2 = "_myLog2";
            var data = new EventLogCreationData();
            data.Add(LOG1, "Source1");
            data.Add(LOG1, "Source2");
            data.Add(LOG2, "Source3");
            data.Add(LOG2, "Source4");

            // --- Act
            var result = WindowsEventLogManager.RemoveEventLogSources(data);

            // --- Assert
            result.Errors.ShouldHaveCountOf(0);
            result.AffectedSources.ShouldHaveCountOf(2);
            result.AffectedSources[LOG1].ShouldHaveCountOf(2);
            result.AffectedSources[LOG1].ShouldContain("Source1");
            result.AffectedSources[LOG1].ShouldContain("Source2");
            result.AffectedSources[LOG2].ShouldHaveCountOf(2);
            result.AffectedSources[LOG2].ShouldContain("Source3");
            result.AffectedSources[LOG2].ShouldContain("Source4");

            EventLog.SourceExists("Source1", ".").ShouldBeFalse();
            EventLog.SourceExists("Source2", ".").ShouldBeFalse();
            EventLog.SourceExists("Source3", ".").ShouldBeFalse();
            EventLog.SourceExists("Source4", ".").ShouldBeFalse();
        }

        [TestMethod]
        public void RemoveSkipsNonExistingSources()
        {
            // --- Arrange
            InstallWorksAsExpected();
            const string LOG1 = "_myLog1";
            const string LOG2 = "_myLog2";
            var data = new EventLogCreationData();
            data.Add(LOG1, "Source1");
            data.Add(LOG1, "Source2");
            data.Add(LOG2, "Source3");
            data.Add(LOG2, "Source4");
            WindowsEventLogManager.InstallEventLogSources(data);
            EventLog.DeleteEventSource("Source1");

            // --- Act
            data.Clear();
            data.Add(LOG1, "Source1");
            data.Add(LOG1, "Source2");
            data.Add(LOG2, "Source3");
            data.Add(LOG1, "Source4"); // --- Original owner is LOG2
            var result = WindowsEventLogManager.RemoveEventLogSources(data);

            // --- Assert
            result.Errors.ShouldHaveCountOf(0);
            result.AffectedSources.ShouldHaveCountOf(2);
            result.AffectedSources[LOG1].ShouldHaveCountOf(1);
            result.AffectedSources[LOG1].ShouldContain("Source2");
            result.AffectedSources[LOG2].ShouldHaveCountOf(1);
            result.AffectedSources[LOG2].ShouldContain("Source3");

            EventLog.SourceExists("Source1", ".").ShouldBeFalse();
            EventLog.SourceExists("Source2", ".").ShouldBeFalse();
            EventLog.SourceExists("Source3", ".").ShouldBeFalse();
            EventLog.SourceExists("Source4", ".").ShouldBeTrue();
        }

        private class DummyNameMapper : INameMapper
        {
            public string Map(string name)
            {
                return name + "L";
            }
        }

        [EventLogName(SEEMPLEST_LOG)]
        public class MyLogName1 : EventLogNameBase
        { }

        [EventLogName(SEEMPLEST_LOG2)]
        public class MyLogName2 : EventLogNameBase
        { }

        [EventLogName(typeof(MyLogName1))]
        [EventType(EventLogEntryType.Information)]
        [EventSource("Source1")]
        [EventId(3)]
        [EventCategoryId(5)]
        public class MyEvent1 : LogEventBase
        { }

        [EventLogName(typeof(MyLogName1))]
        [EventType(EventLogEntryType.Information)]
        [EventSource("Source1")]
        [EventId(3)]
        [EventCategoryId(5)]
        public class MyEvent2 : LogEventBase
        { }

        [EventLogName(typeof(MyLogName1))]
        [EventType(EventLogEntryType.Information)]
        [EventSource("Source2")]
        [EventId(3)]
        [EventCategoryId(5)]
        public class MyEvent3 : LogEventBase
        { }

        [EventLogName(typeof(MyLogName2))]
        [EventType(EventLogEntryType.Information)]
        [EventSource("Source3")]
        [EventId(3)]
        [EventCategoryId(5)]
        public class MyEvent4 : LogEventBase
        { }
    }
}
