using System;
using System.Threading;
using Seemplest.Core.Configuration;
using Seemplest.Core.ServiceObjects;

namespace Seemplest.Core.Tracing
{
    /// <summary>
    /// This interface defines the information describing a trace log item in regard
    /// of a business operation.
    /// </summary>
    public class TraceLogItem: IOperationData
    {
        /// <summary>
        /// UTC timestamp information of the operation
        /// </summary>
        /// <remarks>TraceFormatter token: {ts}</remarks>
        public DateTime? TimestampUtc { get; set; }

        /// <summary>
        /// Optional session ID of the operation
        /// </summary>
        /// <remarks>TraceFormatter token: {s}</remarks>
        public string SessionId { get; set; }

        /// <summary>
        /// Optional business transaction ID of the operation
        /// </summary>
        /// <remarks>TraceFormatter token: {b}</remarks>
        public string BusinessTransactionId { get; set; }

        /// <summary>
        /// Gets the ID of the operation instance
        /// </summary>
        /// <remarks>TraceFormatter token: {oi}</remarks>
        public string OperationInstanceId { get; set; }

        /// <summary>
        /// Gets the type of the operation
        /// </summary>
        /// <remarks>TraceFormatter token: {ot}</remarks>
        public string OperationType { get; set; }

        /// <summary>Gets or sets the ID of the tenant</summary>
        /// <remarks>TraceFormatter token: {ti}</remarks>
        public string TenantId { get; set; }

        /// <summary>Gets or sets the message of the entry</summary>
        /// <remarks>TraceFormatter token: {m}</remarks>
        public string Message { get; set; }

        /// <summary>Gets or sets the message related data of the entry</summary>
        /// <remarks>TraceFormatter token: {d}</remarks>
        public string DetailedMessage { get; set; }

        /// <summary>Gets or sets the type of the message</summary>
        /// <remarks>TraceFormatter token: {it}</remarks>
        public TraceLogItemType Type { get; set; }

        /// <summary>Gets or sets the optional server name</summary>
        /// <remarks>TraceFormatter token: {sn}</remarks>
        public string ServerName { get; set; }

        /// <summary>Gets or sets the Id of the thread raising the message</summary>
        /// <remarks>TraceFormatter token: {th}</remarks>
        public int? ThreadId { get; set; }

        /// <summary>
        /// Fills up properties that are not defined explicitly.
        /// </summary>
        public void EnsureProperties()
        {
            // --- Provide a timestamp
            if (!TimestampUtc.HasValue)
            {
                TimestampUtc = EnvironmentInfo.GetCurrentDateTimeUtc().ToLocalTime();
            }

            // --- Provide the current machine's name as server name
            if (ServerName == null)
            {
                ServerName = EnvironmentInfo.GetMachineName();
            }

            // --- Provide thread information
            if (!ThreadId.HasValue)
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId;
            }
        }
    }
}