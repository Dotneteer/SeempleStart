using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.ServiceObjects;
using Seemplest.MsSql.DataAccess;
using Seemplest.Core.Tracing;
using Seemplest.MsSql.Records;
using Seemplest.MsSql.Tracing;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.Tracing
{
    [TestClass]
    public class SqlTableTraceLoggerTest
    {
        const string DB_CONN = "connStr=Seemplest";

        [TestInitialize]
        public void Cleanup()
        {
            var db = new SqlDatabase(DB_CONN);
            db.Execute(@"delete from [Diagnostics].[Trace]");
        }

        [TestMethod]
        public void SqlTableTraceLoggerWorksAsExpected()
        {
            // --- Arrange
            var tracer = new SqlTableTraceLogger(DB_CONN);

            // --- Act
            tracer.LogInfo("InfoOperation", "Info Message", "Detailed Info Message");
            tracer.LogSuccess("SuccessOperation", "Success Message", "Detailed Success Message");
            tracer.LogWarning("WarningOperation", "Warning Message", "Detailed Warning Message");
            tracer.LogError("ErrorOperation", "Error Message");
            tracer.LogFatalError("FatalOperation", "Fatal Message");

            // --- Assert
            var db = new SqlDatabase(DB_CONN);
            var items = db.Fetch<TraceRecord>("order by Id");
            items.ShouldHaveCountOf(5);
            var item = items[0];
            item.Type.ShouldEqual((int) TraceLogItemType.Informational);
            item.BusinessTransactionId.ShouldBeNull();
            item.SessionId.ShouldBeNull();
            item.OperationInstanceId.ShouldBeNull();
            item.TenantId.ShouldBeNull();
            item.OperationType.ShouldEqual("InfoOperation");
            item.Message.ShouldEqual("Info Message");
            item.DetailedMessage.ShouldEqual("Detailed Info Message");
            item.ServerName.ShouldEqual(Environment.MachineName);
        }
    }
}
