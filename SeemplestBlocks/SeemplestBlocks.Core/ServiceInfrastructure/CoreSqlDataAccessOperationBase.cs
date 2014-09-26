using System.Data.SqlClient;
using System.Text;
using Seemplest.Core.Tracing;
using Seemplest.MsSql.DataAccess;
using SeemplestBlocks.Core.Diagnostics;

namespace SeemplestBlocks.Core.ServiceInfrastructure
{
    /// <summary>
    /// This abstract class is intended to be the base class of all data access layers
    /// </summary>
    public abstract class CoreSqlDataAccessOperationBase : SqlDataAccessOperationBase
    {
        /// <summary>
        /// Initializes the data access object with the specified connection information
        /// </summary>
        /// <param name="connectionOrName">Connection information</param>
        protected CoreSqlDataAccessOperationBase(string connectionOrName)
            : base(connectionOrName)
        {
        }

        /// <summary>
        /// Override this method to catch the event when a command is about to be executed.
        /// </summary>
        /// <param name="command">Command being executed</param>
        protected override void OnCommandExecuting(SqlCommand command)
        {
            if (command == null) return;
            var sb = new StringBuilder();
            if (command.Connection != null)
            {
                sb.AppendFormat("Connection: {0}\r\n", command.Connection.ConnectionString);
            }
            sb.AppendFormat("{0}\r\n", command.CommandText);
            foreach (SqlParameter par in command.Parameters)
            {
                sb.AppendFormat("  {0} ({1}): <'{2}'>\r\n", par.ParameterName, par.SqlDbType, par.Value);
            }
            var logItem = new TraceLogItem
            {
                OperationType = "SQLCommand",
                Type = TraceLogItemType.Informational,
                Message = sb.ToString()
            };
            Tracer.Log(logItem);
        }
    }
}