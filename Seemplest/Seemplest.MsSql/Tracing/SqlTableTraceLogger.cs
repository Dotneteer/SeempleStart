using Seemplest.MsSql.DataAccess;
using Seemplest.Core.Tracing;
using Seemplest.MsSql.Records;

namespace Seemplest.MsSql.Tracing
{
    /// <summary>
    /// This class logs trace entries into a SQL Server table.
    /// </summary>
    public class SqlTableTraceLogger: TraceLoggerBase
    {
        private readonly string _sqlConnectionOrName;

        /// <summary>
        /// Initializes the logger with the specified connection
        /// </summary>
        /// <param name="sqlConnectionOrName"></param>
        public SqlTableTraceLogger(string sqlConnectionOrName)
        {
            _sqlConnectionOrName = sqlConnectionOrName;
        }

        /// <summary>
        /// Override to specify how the trace entry should be logged.
        /// </summary>
        /// <param name="item">Trace entry</param>
        protected override void DoLog(TraceLogItem item)
        {
            using (var db = new SqlDatabase(_sqlConnectionOrName))
            {
                var logRecord = new TraceRecord
                    {
                        // ReSharper disable PossibleInvalidOperationException
                        Timestamp = item.TimestampUtc.Value,
                        // ReSharper restore PossibleInvalidOperationException
                        Type = (int)item.Type,
                        OperationType = item.OperationType,
                        SessionId = item.SessionId,
                        BusinessTransactionId = item.BusinessTransactionId,
                        OperationInstanceId = item.OperationInstanceId,
                        TenantId = item.TenantId,
                        Message = item.Message,
                        DetailedMessage = item.DetailedMessage,
                        ServerName = item.ServerName,
                        // ReSharper disable PossibleInvalidOperationException
                        ThreadId = item.ThreadId.Value
                        // ReSharper restore PossibleInvalidOperationException
                    };
                db.Insert(logRecord);
            }
        }
    }
}