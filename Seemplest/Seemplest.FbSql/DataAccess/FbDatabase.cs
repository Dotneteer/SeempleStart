using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Nito.AsyncEx.Synchronous;
using Seemplest.Core.Common;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.DataAccess.Mapping;
using Seemplest.Core.DataAccess.Tracking;
using Seemplest.Core.Exceptions;
using IDataRecord = Seemplest.Core.DataAccess.DataRecords.IDataRecord;
using IsolationLevel = System.Data.IsolationLevel;

namespace Seemplest.FbSql.DataAccess
{
    /// <summary>
    /// This class is responsible for providing the connection to a SQL Server and executing 
    /// database commands.
    /// </summary>
    public class FbDatabase : IDisposable
    {
        private const string PARAM_PREFIX = "@";

        private bool _transactionCancelled;

        // --- Record changes are collected in this list
        private readonly List<Tuple<IDataRecord, ChangedRecordState>> _changes =
            new List<Tuple<IDataRecord, ChangedRecordState>>();

        // --- This stack assignes transactions with the related change log
        private readonly Stack<int> _transactionLogIndexes = new Stack<int>();

        // --- Regex for recognizing SELECT-like SQL statements
        private static readonly Regex s_RxSelect = new Regex(@"\A\s*(SELECT|EXECUTE)\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // --- Regex for recognizing FROM
        static readonly Regex s_RxFrom = new Regex(@"\A\s*FROM\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // --- Regex for recognizing ORDER BY
        static readonly Regex s_RxOrderBy =
            new Regex(@"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
              RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        // --- Regex for recognizing DISTINCT
        static readonly Regex s_RxDistinct = new Regex(@"\ADISTINCT\s",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        // --- Regex for extracting SELECT list
        static readonly Regex s_RxColumns =
            new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the operation mode of this instance
        /// </summary>
        public SqlOperationMode OperationMode { get; private set; }

        /// <summary>
        /// Gets the direct Execute mode of this instance
        /// </summary>
        public SqlDirectExecuteMode DirectExecuteMode { get; private set; }

        /// <summary>
        /// Gets the handling mode of rowversion columns
        /// </summary>
        public SqlRowVersionHandling RowVersionHandling { get; private set; }

        /// <summary>
        /// Gets the connection name used to obtain the connection string to
        /// the SQL server database
        /// </summary>
        public string ConnectionOrName { get; private set; }

        /// <summary>
        /// Gets the connection string to the SQL Server database
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Stores the current connection information
        /// </summary>
        public FbConnection Connection { get; private set; }

        /// <summary>
        /// Gets the transaction object belongign to this database.
        /// </summary>
        public FbTransaction Transaction { get; private set; }

        /// <summary>
        /// Gets the current depth of shared connections
        /// </summary>
        public int SharedConnectionDepth { get; private set; }

        /// <summary>
        /// Gets the current depth of transactions
        /// </summary>
        public int TransactionDepth { get; private set; }

        //#region Lifecycle management

        /// <summary>
        /// Initializes this instance to use the specified connection information.
        /// </summary>
        /// <param name="connectionOrName"></param>
        public FbDatabase(string connectionOrName)
        {
            ConnectionOrName = connectionOrName;
            ConnectionString = SqlHelper.GetConnectionString(connectionOrName);
            OperationMode = SqlOperationMode.ReadWrite;
            DirectExecuteMode = SqlDirectExecuteMode.Enable;
            RowVersionHandling = SqlRowVersionHandling.RaiseException;
        }

        /// <summary>
        /// Initializes this instance to use the specified connection information,
        /// operation mode, execution mode, and rowversion handling.
        /// </summary>
        /// <param name="connectionOrName"></param>
        /// <param name="operationMode"></param>
        /// <param name="executeMode"></param>
        /// <param name="rowVersionHandling"></param>
        public FbDatabase(string connectionOrName,
            SqlOperationMode operationMode = SqlOperationMode.ReadWrite,
            SqlDirectExecuteMode executeMode = SqlDirectExecuteMode.Enable,
            SqlRowVersionHandling rowVersionHandling = SqlRowVersionHandling.RaiseException)
        {
            ConnectionOrName = connectionOrName;
            ConnectionString = SqlHelper.GetConnectionString(connectionOrName);
            OperationMode = operationMode;
            DirectExecuteMode = executeMode;
            RowVersionHandling = rowVersionHandling;
        }

        /// <summary>
        /// This event is raised when the tracking info is collected at the instance disposal.
        /// </summary>
        public event EventHandler<TrackingInfoEventArgs> TrackingCompleted;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // --- Uncommitted transactions should be rolled back
            if (TransactionDepth > 0)
            {
                _transactionCancelled = true;
                var task = CleanupTransactionAsync();
                task.WaitAndUnwrapException();
            }

            // --- Dispose the connection object
            Connection?.Dispose();
            if (!IsTracked()) return;

            // --- Manage tracking event
            var trackingEvent = new TrackingInfoEventArgs(CollectTrackingInfo());
            _transactionLogIndexes.Clear();
            _changes.Clear();
            TrackingCompleted?.Invoke(this, trackingEvent);
            if (trackingEvent.TrackingException != null)
            {
                throw new TrackingAbortedException(trackingEvent.TrackingException);
            }
        }

        /// <summary>
        /// Collects the accumulated change set from individual changes
        /// </summary>
        /// <returns></returns>
        private SqlDatabaseChangeSet CollectTrackingInfo()
        {
            var changeSet = new SqlDatabaseChangeSet();

            // --- Iterate through all individual changes
            foreach (var change in _changes)
            {
                var record = change.Item1;
                var changeType = change.Item2;

                // --- Obtain the table of the record
                var metadata = RecordMetadataManager.GetMetadata(record.GetRecordType());
                var tableKey = EscapeSqlTableName(metadata.SchemaName, metadata.TableName);
                SqlTableChangeSet tableChangeSet;
                if (!changeSet.TryGetValue(tableKey, out tableChangeSet))
                {
                    tableChangeSet = new SqlTableChangeSet();
                    changeSet.ChangeSet.Add(tableKey, tableChangeSet);
                }

                // --- Prepare the records primary key
                var pk = new PrimaryKeyValue(record, metadata);
                SqlRecordChangeSet recordChangeSet;
                tableChangeSet.TryGetValue(pk, out recordChangeSet);

                // --- Set the state of the data record according to the change
                switch (changeType)
                {
                    case ChangedRecordState.Attached:
                        if (recordChangeSet == null)
                        {
                            tableChangeSet.ChangeSet.Add(pk, CreateChangeSetForAttach(record, metadata));
                        }
                        else if (recordChangeSet.State == ChangedRecordState.Attached)
                        {
                            tableChangeSet.ChangeSet[pk] = CreateChangeSetForAttach(record, metadata);
                        }
                        else
                        {
                            recordChangeSet.InternalIssueList.Add(CreateIssue(
                                $"This record has already been attached to this tracking context with {recordChangeSet.State} state", record));
                        }
                        break;

                    case ChangedRecordState.Inserted:
                        if (recordChangeSet == null)
                        {
                            tableChangeSet.ChangeSet.Add(pk, CreateChangeSetForInsert(record, metadata));
                        }
                        else
                        {
                            recordChangeSet.InternalIssueList.Add(CreateIssue(
                                $"This record has already been attached to this tracking context with {recordChangeSet.State} state", record));
                        }
                        break;

                    case ChangedRecordState.Updated:
                        if (recordChangeSet != null)
                        {
                            if (recordChangeSet.State == ChangedRecordState.Deleted)
                            {
                                recordChangeSet.InternalIssueList.Add(CreateIssue(
                                    "This record has already been deleted from this tracking context", record));
                            }
                            else
                            {
                                var oldState = recordChangeSet.State;
                                recordChangeSet = CreateChangeSetForUpdate(record, metadata, recordChangeSet);
                                if (oldState == ChangedRecordState.Inserted)
                                {
                                    recordChangeSet.State = ChangedRecordState.Inserted;
                                }
                                tableChangeSet.ChangeSet[pk] = recordChangeSet;
                            }
                        }
                        else
                        {
                            recordChangeSet = CreateChangeSetForInsert(record, metadata);
                            recordChangeSet.State = ChangedRecordState.Updated;
                            tableChangeSet.ChangeSet[pk] = recordChangeSet;
                            recordChangeSet.InternalIssueList.Add(CreateIssue(
                                "This record has not been attached to this tracking context", record));
                        }
                        break;

                    case ChangedRecordState.Deleted:
                        if (recordChangeSet != null)
                        {
                            if (recordChangeSet.State == ChangedRecordState.Inserted)
                            {
                                tableChangeSet.ChangeSet.Remove(pk);
                            }
                            else
                            {
                                tableChangeSet.ChangeSet[pk] = CreateChangeSetForDelete(record, metadata);
                            }
                        }
                        else
                        {
                            recordChangeSet = CreateChangeSetForDelete(record, metadata);
                            tableChangeSet.ChangeSet[pk] = recordChangeSet;
                            recordChangeSet.InternalIssueList.Add(CreateIssue(
                                "This record has not been attached to this tracking context", record));
                        }
                        break;
                }
            }

            // --- Eliminate unchanged fields, records and tables
            foreach (var table in changeSet.Values)
            {
                foreach (var record in table.Values)
                {
                    record.EliminateNonChangedFields();
                }
            }
            changeSet.EliminateUnchangedTables();
            // --- Retrieve the collected changes
            return changeSet;
        }

        /// <summary>
        /// Creates a tracking issue for the specified data record
        /// </summary>
        /// <param name="issue">Issue description</param>
        /// <param name="record">Record with issue</param>
        /// <returns></returns>
        private static TrackingIssue CreateIssue(string issue, IDataRecord record)
        {
            return new TrackingIssue(record.Clone(), issue);
        }

        /// <summary>
        /// Creates a record change set for the specified record and metadata in attached state
        /// </summary>
        /// <param name="record">Record instance</param>
        /// <param name="metadata">Metadata instance</param>
        /// <returns>Newly created record change set</returns>
        private static SqlRecordChangeSet CreateChangeSetForAttach(IDataRecord record, DataRecordDescriptor metadata)
        {
            var recordChangeSet = new SqlRecordChangeSet { State = ChangedRecordState.Attached };
            foreach (var dataColumn in metadata.DataColumns)
            {
                var value = dataColumn.PropertyInfo.GetValue(record);
                recordChangeSet.ChangeSet.Add(dataColumn.ColumnName,
                    new SqlFieldChange(value, value));
            }
            return recordChangeSet;
        }

        /// <summary>
        /// Creates a record change set for the specified record and metadata in insertd state
        /// </summary>
        /// <param name="record">Record instance</param>
        /// <param name="metadata">Metadata instance</param>
        /// <returns>Newly created record change set</returns>
        private static SqlRecordChangeSet CreateChangeSetForInsert(IDataRecord record, DataRecordDescriptor metadata)
        {
            var recordChangeSet = new SqlRecordChangeSet { State = ChangedRecordState.Inserted };
            foreach (var dataColumn in metadata.DataColumns)
            {
                recordChangeSet.ChangeSet.Add(dataColumn.ColumnName,
                    new SqlFieldChange(null, dataColumn.PropertyInfo.GetValue(record)));
            }
            return recordChangeSet;
        }

        /// <summary>
        /// Creates a record change set for the specified record and metadata in updated state
        /// </summary>
        /// <param name="record">Record instance</param>
        /// <param name="metadata">Metadata instance</param>
        /// <param name="recordChangeSet">Previous record change set</param>
        /// <returns>Newly created record change set</returns>
        private static SqlRecordChangeSet CreateChangeSetForUpdate(IDataRecord record, DataRecordDescriptor metadata,
            IReadOnlyDictionary<string, SqlFieldChange> recordChangeSet)
        {
            var newChangeSet = new SqlRecordChangeSet { State = ChangedRecordState.Updated };
            foreach (var dataColumn in metadata.DataColumns)
            {
                var oldField = recordChangeSet[dataColumn.ColumnName];
                newChangeSet.ChangeSet.Add(dataColumn.ColumnName,
                    new SqlFieldChange(oldField.PreviousValue, dataColumn.PropertyInfo.GetValue(record)));
            }
            return newChangeSet;
        }

        /// <summary>
        /// Creates a record change set for the specified record and metadata in deleted state
        /// </summary>
        /// <param name="record">Record instance</param>
        /// <param name="metadata">Metadata instance</param>
        /// <returns>Newly created record change set</returns>
        private static SqlRecordChangeSet CreateChangeSetForDelete(IDataRecord record, DataRecordDescriptor metadata)
        {
            var recordChangeSet = new SqlRecordChangeSet { State = ChangedRecordState.Deleted };
            foreach (var dataColumn in metadata.DataColumns)
            {
                recordChangeSet.ChangeSet.Add(dataColumn.ColumnName,
                    new SqlFieldChange(dataColumn.PropertyInfo.GetValue(record), null));
            }
            return recordChangeSet;
        }

        //#endregion

        //#region Connection and transaction management

        ///// <summary>
        ///// Opens a shared connection that allows nesting.
        ///// </summary>
        //public void OpenSharedConnection()
        //{
        //    var task = OpenSharedConnectionAsync();
        //    task.WaitAndUnwrapException();
        //}

        ///// <summary>
        ///// Opens a shared connection that allows nesting -- async.
        ///// </summary>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task OpenSharedConnectionAsync(CancellationToken token = default(CancellationToken))
        //{
        //    if (SharedConnectionDepth == 0)
        //    {
        //        // --- There is no open connection
        //        Connection = new SqlConnection(ConnectionString);
        //        CloseBrokenConnection(Connection);
        //        if (Connection.State == ConnectionState.Closed)
        //        {
        //            await Connection.OpenAsync(token);
        //        }
        //        Connection = OnConnectionOpened(Connection);
        //    }
        //    SharedConnectionDepth++;
        //}

        /// <summary>
        /// Close a previously opened connection 
        /// </summary>
        public void CloseSharedConnection()
        {
            if (SharedConnectionDepth <= 0) return;
            SharedConnectionDepth--;
            if (SharedConnectionDepth != 0) return;
            OnConnectionClosing(Connection);
            Connection.Dispose();
            Connection = null;
        }

        /// <summary>
        /// Close a previously opened connection -- async
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        public Task CloseSharedConnectionAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => CloseSharedConnection(), token);
        }

        ///// <summary>
        ///// Starts a new transaction with the specified optional isolation level.
        ///// </summary>
        ///// <param name="isolationLevel">Isolation level to use.</param>
        //public void BeginTransaction(IsolationLevel? isolationLevel = null)
        //{
        //    var task = BeginTransactionAsync(isolationLevel);
        //    task.WaitAndUnwrapException();
        //}

        ///// <summary>
        ///// Starts a new transaction with the specified optional isolation level.
        ///// </summary>
        ///// <param name="isolationLevel">Isolation level to use.</param>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task BeginTransactionAsync(IsolationLevel? isolationLevel = null, CancellationToken token = default(CancellationToken))
        //{
        //    TransactionDepth++;
        //    _transactionLogIndexes.Push(_changes.Count);
        //    if (TransactionDepth != 1) return;
        //    await OpenSharedConnectionAsync(token);
        //    Transaction = isolationLevel == null
        //        ? Connection.BeginTransaction()
        //        : Connection.BeginTransaction(isolationLevel.Value);
        //    _transactionCancelled = false;
        //    OnBeginTransaction();
        //}

        ///// <summary>
        ///// Aborts the current transaction.
        ///// </summary>
        //public void AbortTransaction()
        //{
        //    var task = AbortTransactionAsync();
        //    task.WaitAndUnwrapException();
        //}

        ///// <summary>
        ///// Aborts the current transaction.
        ///// </summary>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task AbortTransactionAsync(CancellationToken token = default(CancellationToken))
        //{
        //    if (TransactionDepth == 0)
        //    {
        //        throw new InvalidOperationException("There is no open transaction to abort.");
        //    }
        //    _transactionCancelled = true;

        //    // --- Cleanup the log for the aborted transaction
        //    var abortIndex = _transactionLogIndexes.Pop();
        //    _changes.RemoveRange(abortIndex, _changes.Count - abortIndex);

        //    // --- Clean up the transaction 
        //    if ((--TransactionDepth) == 0)
        //    {
        //        await CleanupTransactionAsync(token);
        //    }
        //}

        ///// <summary>
        ///// Completes the current transaction.
        ///// </summary>
        //public void CompleteTransaction()
        //{
        //    var task = CompleteTransactionAsync();
        //    task.WaitAndUnwrapException();
        //}

        ///// <summary>
        ///// Completes the current transaction -- async
        ///// </summary>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task CompleteTransactionAsync(CancellationToken token = default(CancellationToken))
        //{
        //    if (TransactionDepth == 0)
        //    {
        //        throw new InvalidOperationException("There is no open transaction to complete.");
        //    }

        //    // --- all changes should be kept...
        //    var abortIndex = _transactionLogIndexes.Pop();

        //    // --- ...unless the transaction has already been cancelled
        //    if (_transactionCancelled)
        //    {
        //        _changes.RemoveRange(abortIndex, _changes.Count - abortIndex);
        //    }

        //    // --- Clean up the transaction 
        //    if ((--TransactionDepth) == 0)
        //    {
        //        await CleanupTransactionAsync(token);
        //    }
        //}

        //#endregion

        //#region SQL Command execution

        ///// <summary>
        ///// Sets the command timeout value to use when executing a command.
        ///// </summary>
        //public int CommandTimeout { get; set; }

        ///// <summary>
        ///// Sets a one-time command timeout value that is reset right after executing the next command.
        ///// </summary>
        //public int OneTimeCommandTimeout { get; set; }

        ///// <summary>
        ///// The last SQL string exceuted in a command
        ///// </summary>
        //public string LastSql { get; private set; }

        ///// <summary>
        ///// The last SQL arguments used in a command
        ///// </summary>
        //public object[] LastArgs { get; private set; }

        ///// <summary>
        ///// The last command executed
        ///// </summary>
        //public string LastCommand
        //{
        //    get { return FormatCommand(LastSql, LastArgs); }
        //}

        ///// <summary>
        ///// Gets or sets the object used to map CLR instances to <see cref="SqlParameter"/> instances.
        ///// </summary>
        //public ISqlParameterMapper ParameterMapper { get; set; }

        ///// <summary>
        ///// Executes the specified SQL batch with the provided parameters.
        ///// </summary>
        ///// <param name="sql">SQL string</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>Number of rows affected</returns>
        //public int Execute(string sql, params object[] args)
        //{
        //    return Execute(new SqlExpression(sql, args));
        //}

        ///// <summary>
        ///// Executes the specified SQL batch with the provided parameters -- async
        ///// </summary>
        ///// <param name="sql">SQL string</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>Number of rows affected</returns>
        //public Task<int> ExecuteAsync(string sql, params object[] args)
        //{
        //    return ExecuteAsync(new SqlExpression(sql, args));
        //}

        ///// <summary>
        ///// Executes the specified SQL batch with the provided parameters -- async
        ///// </summary>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL string</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>Number of rows affected</returns>
        //public Task<int> ExecuteAsync(CancellationToken token, string sql, params object[] args)
        //{
        //    return ExecuteAsync(new SqlExpression(sql, args), token);
        //}

        ///// <summary>
        ///// Executes the specified SQL batch.
        ///// </summary>
        ///// <param name="sqlExpr">SQL statement</param>
        ///// <returns>Number of rows affected</returns>
        //public int Execute(SqlExpression sqlExpr)
        //{
        //    var task = ExecuteAsync(sqlExpr);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Executes the specified SQL batch -- async
        ///// </summary>
        ///// <param name="sqlExpr">SQL statement</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>Number of rows affected</returns>
        //public async Task<int> ExecuteAsync(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    // --- Check if direct execution is enabled or not
        //    if (DirectExecuteMode != SqlDirectExecuteMode.Enable)
        //    {
        //        throw new InvalidOperationException(
        //            "Using the Execute family of operation is not allowed.");
        //    }

        //    // --- Prepare and execute the command
        //    var sql = sqlExpr.SqlText;
        //    var args = sqlExpr.Arguments;
        //    try
        //    {
        //        await OpenSharedConnectionAsync(token);
        //        try
        //        {
        //            using (var cmd = CreateCommand(Connection, sql, args))
        //            {
        //                var result = await cmd.ExecuteNonQueryAsync(token);
        //                OnExecutedCommand(cmd);
        //                return result;
        //            }
        //        }
        //        finally
        //        {
        //            CloseSharedConnection();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        OnException(ex);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Executes an SQL batch that returns a scalar value.
        ///// </summary>
        ///// <typeparam name="T">Type of scalar value</typeparam>
        ///// <param name="sqlString">SQL string</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The value resulted from executing this SQL batch</returns>
        //public T ExecuteScalar<T>(string sqlString, params object[] args)
        //{
        //    return ExecuteScalar<T>(new SqlExpression(sqlString, args));
        //}

        ///// <summary>
        ///// Executes an SQL batch that returns a scalar value -- async
        ///// </summary>
        ///// <typeparam name="T">Type of scalar value</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sqlString">SQL string</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The value resulted from executing this SQL batch</returns>
        //public Task<T> ExecuteScalarAsync<T>(CancellationToken token, string sqlString, params object[] args)
        //{
        //    return ExecuteScalarAsync<T>(new SqlExpression(sqlString, args), token);
        //}

        ///// <summary>
        ///// Executes an SQL batch that returns a scalar value -- async
        ///// </summary>
        ///// <typeparam name="T">Type of scalar value</typeparam>
        ///// <param name="sqlString">SQL string</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The value resulted from executing this SQL batch</returns>
        //public Task<T> ExecuteScalarAsync<T>(string sqlString, params object[] args)
        //{
        //    return ExecuteScalarAsync<T>(new SqlExpression(sqlString, args));
        //}

        ///// <summary>
        ///// Executes an SQL batch that returns a scalar value.
        ///// </summary>
        ///// <typeparam name="T">Type of scalar value</typeparam>
        ///// <param name="sqlExpr">SQL string</param>
        ///// <returns>The value resulted from executing this SQL batch</returns>
        //public T ExecuteScalar<T>(SqlExpression sqlExpr)
        //{
        //    var task = ExecuteScalarAsync<T>(sqlExpr);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Executes an SQL batch that returns a scalar value.
        ///// </summary>
        ///// <typeparam name="T">Type of scalar value</typeparam>
        ///// <param name="sqlExpr">SQL string</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The value resulted from executing this SQL batch</returns>
        //public async Task<T> ExecuteScalarAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    // --- Check if direct execution is enabled or not
        //    if (DirectExecuteMode != SqlDirectExecuteMode.Enable)
        //    {
        //        throw new InvalidOperationException(
        //            "Using the Execute family of operation is not allowed.");
        //    }

        //    // --- Prepare and execute the command
        //    var sql = sqlExpr.SqlText;
        //    var args = sqlExpr.Arguments;
        //    try
        //    {
        //        await OpenSharedConnectionAsync(token);
        //        try
        //        {
        //            using (var cmd = CreateCommand(Connection, sql, args))
        //            {
        //                var val = await cmd.ExecuteScalarAsync(token) ?? default(T);
        //                OnExecutedCommand(cmd);

        //                var t = typeof(T);
        //                var u = Nullable.GetUnderlyingType(t);
        //                if (u == null)
        //                {
        //                    return (T)Convert.ChangeType(val, t);
        //                }
        //                else
        //                {
        //                    return val == null || val == DBNull.Value
        //                               ? default(T)
        //                               : (T)Convert.ChangeType(val, u);
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            CloseSharedConnection();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        OnException(ex);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Formats the specified SQL command for diagnostics purposes
        ///// </summary>
        ///// <param name="cmd">Command to format</param>
        ///// <returns>Formatted SQL command</returns>
        //public string FormatCommand(SqlCommand cmd)
        //{
        //    return FormatCommand(cmd.CommandText,
        //        (from IDataParameter parameter in cmd.Parameters
        //         select parameter.Value).ToArray());
        //}

        ///// <summary>
        ///// Formats the specified SQL command for diagnostics purposes
        ///// </summary>
        ///// <param name="sql">SQL command string</param>
        ///// <param name="args">Command arguments</param>
        ///// <returns>Formatted SQL command</returns>
        //public string FormatCommand(string sql, object[] args)
        //{
        //    var sb = new StringBuilder();
        //    if (sql == null) return String.Empty;
        //    sb.Append(sql);
        //    if (args != null && args.Length > 0)
        //    {
        //        sb.Append("\n");
        //        for (var i = 0; i < args.Length; i++)
        //        {
        //            sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", PARAM_PREFIX, i, args[i].GetType().Name, args[i]);
        //        }
        //        sb.Remove(sb.Length - 1, 1);
        //    }
        //    return sb.ToString();
        //}

        //#endregion

        //#region Event methods

        ///// <summary>
        ///// This method is called when a new <see cref="SqlConnection"/> instance is 
        ///// about to be open.
        ///// </summary>
        ///// <param name="conn">Connection instance</param>
        ///// <returns>Connection instance to open</returns>
        //public virtual SqlConnection OnConnectionOpened(SqlConnection conn)
        //{
        //    return conn;
        //}

        /// <summary>
        /// This method is called when a <see cref="FbConnection"/> instance is about to
        /// be closed.
        /// </summary>
        /// <param name="conn">Connection instance</param>
        public virtual void OnConnectionClosing(FbConnection conn)
        {
        }

        ///// <summary>
        ///// Override this method to handle additional activites when a new transaction is started.
        ///// </summary>
        //public virtual void OnBeginTransaction()
        //{
        //}

        /// <summary>
        /// Override this method to handle additional activites when a new transaction is ended.
        /// </summary>
        public virtual void OnEndTransaction()
        {
        }

        ///// <summary>
        ///// This method is called when a <see cref="SqlCommand"/> is about to be executed.
        ///// </summary>
        ///// <param name="command">Command to execute</param>
        //public virtual void OnExecutingCommand(SqlCommand command)
        //{
        //}

        ///// <summary>
        ///// This method is called when a <see cref="SqlCommand"/> has been executed.
        ///// </summary>
        ///// <param name="command">Command that has been executed</param>
        //public virtual void OnExecutedCommand(SqlCommand command)
        //{
        //}

        ///// <summary>
        ///// Override this event to handle an exception caught during command execution.
        ///// </summary>
        ///// <param name="x"></param>
        //public virtual void OnException(Exception x)
        //{
        //    Debug.WriteLine(x.ToString());
        //    Debug.WriteLine(LastCommand);
        //}

        //#endregion

        //#region Single record queries

        ///// <summary>
        ///// Retrieves the first object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public T First<T>(string sql, params object[] args)
        //{
        //    return Query<T>(sql, args).First();
        //}

        ///// <summary>
        ///// Retrieves the first object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public async Task<T> FirstAsync<T>(string sql, params object[] args)
        //{
        //    return (await QueryAsync<T>(sql, args)).First();
        //}

        ///// <summary>
        ///// Retrieves the first object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public T First<T>(SqlExpression sqlExpr)
        //{
        //    return Query<T>(sqlExpr).First();
        //}

        ///// <summary>
        ///// Retrieves the first object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public async Task<T> FirstAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    return (await QueryAsync<T>(sqlExpr, token)).First();
        //}

        ///// <summary>
        ///// Retrieves the first object and maps it into another object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        //public T FirstInto<T>(T instance, string sql, params object[] args)
        //{
        //    var task = FirstIntoAsync(instance, sql, args);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves the first object and maps it into another object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        //public async Task<T> FirstIntoAsync<T>(T instance, string sql, params object[] args)
        //{
        //    var records = await DoQueryAsync(new SqlExpression(sql, args), instance, default(CancellationToken), true);
        //    return records.First();
        //}

        ///// <summary>
        ///// Retrieves the first object and maps it into another object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        //public async Task<T> FirstIntoAsync<T>(CancellationToken token, T instance, string sql, params object[] args)
        //{
        //    var records = await DoQueryAsync(new SqlExpression(sql, args), instance, token, true);
        //    return records.First();
        //}

        ///// <summary>
        ///// Retrieves the first object and maps it into another object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <returns>The single object retrieved</returns>
        //public T FirstInto<T>(T instance, SqlExpression sqlExpr)
        //{
        //    var task = DoQueryAsync(sqlExpr, instance, default(CancellationToken), true);
        //    task.WaitAndUnwrapException();
        //    return task.Result.First();
        //}

        ///// <summary>
        ///// Retrieves the first object and maps it into another object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single object retrieved</returns>
        //public async Task<T> FirstIntoAsync<T>(T instance, SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    var records = await DoQueryAsync(sqlExpr, instance, token, true);
        //    return records.First();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public T FirstOrDefault<T>(string sql, params object[] args)
        //{
        //    return Query<T>(sql, args).FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public async Task<T> FirstOrDefaultAsync<T>(string sql, params object[] args)
        //{
        //    return (await QueryAsync<T>(sql, args)).FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public async Task<T> FirstOrDefaultAsync<T>(CancellationToken token, string sql, params object[] args)
        //{
        //    return (await QueryAsync<T>(token, sql, args)).FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public T FirstOrDefault<T>(SqlExpression sqlExpr)
        //{
        //    return Query<T>(sqlExpr).FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public async Task<T> FirstOrDefaultAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    return (await QueryAsync<T>(sqlExpr, token)).FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- and maps it into another object 
        ///// using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public T FirstOrDefaultInto<T>(T instance, string sql, params object[] args)
        //{
        //    var task = DoQueryAsync(new SqlExpression(sql, args), instance, default(CancellationToken), true);
        //    task.WaitAndUnwrapException();
        //    return task.Result.FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- and maps it into another object -- async
        ///// using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public async Task<T> FirstOrDefaultIntoAsync<T>(T instance, string sql, params object[] args)
        //{
        //    var records = await DoQueryAsync(new SqlExpression(sql, args), instance, default(CancellationToken), true);
        //    return records.FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- and maps it into another object -- async
        ///// using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public async Task<T> FirstOrDefaultIntoAsync<T>(CancellationToken token, T instance, string sql, params object[] args)
        //{
        //    var records = await DoQueryAsync(new SqlExpression(sql, args), instance, token, true);
        //    return records.FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- and maps it into another 
        ///// object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public T FirstOrDefaultInto<T>(T instance, SqlExpression sqlExpr)
        //{
        //    var task = DoQueryAsync(sqlExpr, instance, default(CancellationToken), true);
        //    task.WaitAndUnwrapException();
        //    return task.Result.FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves the first object -- or the default if no objects found -- and maps it into another -- async
        ///// object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public async Task<T> FirstOrDefaultIntoAsync<T>(T instance, SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    var records = await DoQueryAsync(sqlExpr, instance, token, true);
        //    return records.FirstOrDefault();
        //}

        ///// <summary>
        ///// Retrieves a single object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public T Single<T>(string sql, params object[] args)
        //{
        //    return Query<T>(sql, args).Single();
        //}

        ///// <summary>
        ///// Retrieves a single object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public async Task<T> SingleAsync<T>(string sql, params object[] args)
        //{
        //    return (await QueryAsync<T>(sql, args)).Single();
        //}

        ///// <summary>
        ///// Retrieves a single object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Data record type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        //public async Task<T> SingleAsync<T>(CancellationToken token, string sql, params object[] args)
        //{
        //    return (await QueryAsync<T>(token, sql, args)).Single();
        //}

        ///// <summary>
        ///// Retrieves a single object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public T Single<T>(SqlExpression sqlExpr)
        //{
        //    return Query<T>(sqlExpr).Single();
        //}

        ///// <summary>
        ///// Retrieves a single object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public async Task<T> SingleAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    return (await QueryAsync<T>(sqlExpr, token)).Single();
        //}

        ///// <summary>
        ///// Retrieves a single object and maps it into another object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public T SingleInto<T>(T instance, string sql, params object[] args)
        //{
        //    var task = SingleIntoAsync(instance, new SqlExpression(sql, args));
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves a single object and maps it into another object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public async Task<T> SingleIntoAsync<T>(T instance, string sql, params object[] args)
        //{
        //    return (await DoQueryAsync(new SqlExpression(sql, args), instance)).Single();
        //}

        ///// <summary>
        ///// Retrieves a single object and maps it into another object using the specified SQL batch -- async
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public async Task<T> SingleIntoAsync<T>(CancellationToken token, T instance, string sql, params object[] args)
        //{
        //    return (await DoQueryAsync(new SqlExpression(sql, args), instance, token)).Single();
        //}

        ///// <summary>
        ///// Retrieves a single object and maps it into another object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public T SingleInto<T>(T instance, SqlExpression sqlExpr)
        //{
        //    var task = SingleIntoAsync(instance, sqlExpr);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves a single object and maps it into another object using the specified SQL batch
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="instance">Poco instance to map the column values to</param>
        ///// <param name="sqlExpr">SQL expression</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single object retrieved</returns>
        ///// <remarks>Raises an exception, if not a single object found</remarks>
        //public async Task<T> SingleIntoAsync<T>(T instance, SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    return (await DoQueryAsync(sqlExpr, instance, token)).Single();
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements.
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public T SingleById<T>(object pkFirst, params object[] pkOthers)
        //{
        //    var task = SingleByIdAsync<T>(pkFirst, pkOthers);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public Task<T> SingleByIdAsync<T>(object pkFirst, params object[] pkOthers)
        //{
        //    return SingleByIdAsync<T>(default(CancellationToken), pkFirst, pkOthers);
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public Task<T> SingleByIdAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        //{
        //    var pkValues = new object[pkOthers.Length + 1];
        //    pkValues[0] = pkFirst;
        //    for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
        //    return SingleByIdAsync<T>(pkValues, token);
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements.
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkValues">Primary key elements</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public T SingleById<T>(IEnumerable<object> pkValues)
        //{
        //    var task = SingleByIdAsync<T>(pkValues);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkValues">Primary key elements</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public async Task<T> SingleByIdAsync<T>(IEnumerable<object> pkValues,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    var values = pkValues.ToList();
        //    var records = await FetchByPrimaryKeyAsync<T>(values, token);
        //    if (records.Count == 1) return records[0];
        //    if (records.Count == 0) throw new RecordNotFoundException(typeof(T), values.ToArray());
        //    throw new MultipleRecordFoundException(typeof(T), values.ToArray());
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements.
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public T SingleOrDefaultById<T>(object pkFirst, params object[] pkOthers)
        //{
        //    var task = SingleOrDefaultByIdAsync<T>(pkFirst, pkOthers);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public Task<T> SingleOrDefaultByIdAsync<T>(object pkFirst, params object[] pkOthers)
        //{
        //    return SingleOrDefaultByIdAsync<T>(default(CancellationToken), pkFirst, pkOthers);
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public Task<T> SingleOrDefaultByIdAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        //{
        //    var pkValues = new object[pkOthers.Length + 1];
        //    pkValues[0] = pkFirst;
        //    for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
        //    return SingleOrDefaultByIdAsync<T>(pkValues, token);
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements.
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkValues">Primary key elements</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public T SingleOrDefaultById<T>(IEnumerable<object> pkValues)
        //{
        //    var task = SingleOrDefaultByIdAsync<T>(pkValues);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves a single record by the specified primary key elements -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkValues">Primary key elements</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The single record if that can be found.</returns>
        ///// <exception cref="RecordNotFoundException">No record can be found</exception>
        ///// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        //public async Task<T> SingleOrDefaultByIdAsync<T>(IEnumerable<object> pkValues,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    var values = pkValues.ToList();
        //    var records = await FetchByPrimaryKeyAsync<T>(values, token);
        //    if (records.Count > 1) throw new MultipleRecordFoundException(typeof(T), values.ToArray());
        //    return (records.Count == 0) ? default(T) : records[0];
        //}

        ///// <summary>
        ///// Checks if a record with the specified primary key values exists in the database
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>True, if the record exists; otherwise, false</returns>
        //public bool Exists<T>(object pkFirst, params object[] pkOthers)
        //{
        //    var task = ExistsAsync<T>(pkFirst, pkOthers);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Checks if a record with the specified primary key values exists in the database
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>True, if the record exists; otherwise, false</returns>
        //public Task<bool> ExistsAsync<T>(object pkFirst, params object[] pkOthers)
        //{
        //    return ExistsAsync<T>(default(CancellationToken), pkFirst, pkOthers);
        //}

        ///// <summary>
        ///// Checks if a record with the specified primary key values exists in the database
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        ///// <returns>True, if the record exists; otherwise, false</returns>
        //public Task<bool> ExistsAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        //{
        //    var pkValues = new object[pkOthers.Length + 1];
        //    pkValues[0] = pkFirst;
        //    for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
        //    return ExistsAsync<T>(pkValues, token);
        //}

        ///// <summary>
        ///// Checks if a record with the specified primary key values exists in the database
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkValues">Primary key elements</param>
        ///// <returns>True, if the record exists; otherwise, false</returns>
        //public bool Exists<T>(IEnumerable<object> pkValues)
        //{
        //    var task = ExistsAsync<T>(pkValues);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Checks if a record with the specified primary key values exists in the database
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkValues">Primary key elements</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>True, if the record exists; otherwise, false</returns>
        //public async Task<bool> ExistsAsync<T>(IEnumerable<object> pkValues,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return (await FetchByPrimaryKeyAsync<T>(pkValues.ToList(), token)).Count > 0;
        //}

        ///// <summary>
        ///// Fetches all records filtered by the specified primary key values -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="values">Primary key values</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>List of record filtered by the primary key</returns>
        //private Task<List<T>> FetchByPrimaryKeyAsync<T>(IReadOnlyList<object> values,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    // --- Check if the correct number of key element has been provided
        //    var metadata = RecordMetadataManager.GetMetadata<T>();
        //    if (metadata.PrimaryKeyColumns.Count != values.Count)
        //    {
        //        throw new InvalidOperationException(
        //            string.Format("Type {0} has primary key item count {1}, but the number of provided item is {2}.",
        //            typeof(T).FullName, metadata.PrimaryKeyColumns.Count, values.Count));
        //    }

        //    // --- Prepare the expression to execute
        //    var sqlExpr = SqlExpression.New.Select<T>().From<T>();
        //    for (var i = 0; i < values.Count; i++)
        //    {
        //        if (values[i] == null)
        //        {
        //            throw new InvalidOperationException(
        //                string.Format("Value of primary key element {0} cannot be null",
        //                metadata.PrimaryKeyColumns[i].ColumnName));
        //        }
        //        sqlExpr.Where(
        //            string.Format("{0}=@0", EscapeSqlIdentifier(metadata.PrimaryKeyColumns[i].ColumnName)),
        //            values[i]);
        //    }
        //    return FetchAsync<T>(sqlExpr, token);
        //}

        //#endregion

        //#region Fetch operations

        ///// <summary>
        ///// Fetches records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public List<T> Fetch<T>(string sql, params object[] args)
        //{
        //    return Fetch<T>(new SqlExpression(sql, args));
        //}

        ///// <summary>
        ///// Fetches records from the database with the specified SQL fragment -- async
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public async Task<List<T>> FetchAsync<T>(string sql, params object[] args)
        //{
        //    return await FetchAsync<T>(new SqlExpression(sql, args));
        //}

        ///// <summary>
        ///// Fetches records from the database with the specified SQL fragment -- async
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public async Task<List<T>> FetchAsync<T>(CancellationToken token, string sql, params object[] args)
        //{
        //    return await FetchAsync<T>(new SqlExpression(sql, args), token);
        //}

        ///// <summary>
        ///// Fetches records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="sql">SQL fragment</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public List<T> Fetch<T>(SqlExpression sql)
        //{
        //    return Query<T>(sql).ToList();
        //}

        ///// <summary>
        ///// Fetches records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public async Task<List<T>> FetchAsync<T>(SqlExpression sql, CancellationToken token = default(CancellationToken))
        //{
        //    return (await QueryAsync<T>(sql, token)).ToList();
        //}

        ///// <summary>
        ///// Fetches records from the database.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public List<T> Fetch<T>()
        //{
        //    return Fetch<T>("");
        //}

        ///// <summary>
        ///// Fetches records from the database -- async
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public async Task<List<T>> FetchAsync<T>(CancellationToken token = default(CancellationToken))
        //{
        //    return (await FetchAsync<T>(token, ""));
        //}

        ///// <summary>
        ///// Retrieves an enumeration mapped to the specified poco type.
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public IEnumerable<T> Query<T>(string sql, params object[] args)
        //{
        //    return Query<T>(new SqlExpression(sql, args));
        //}

        ///// <summary>
        ///// Retrieves an enumeration mapped to the specified poco type - async
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] args)
        //{
        //    return QueryAsync<T>(new SqlExpression(sql, args));
        //}

        ///// <summary>
        ///// Retrieves an enumeration mapped to the specified poco type - async
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public Task<IEnumerable<T>> QueryAsync<T>(CancellationToken token, string sql, params object[] args)
        //{
        //    return QueryAsync<T>(new SqlExpression(sql, args), token);
        //}

        ///// <summary>
        ///// Retrieves an enumeration mapped to the specified poco type, using the specified SQL object.
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="sqlExpr">SQL object</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public IEnumerable<T> Query<T>(SqlExpression sqlExpr)
        //{
        //    var task = QueryAsync<T>(sqlExpr);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Retrieves an enumeration mapped to the specified poco type, using the specified SQL object.
        ///// </summary>
        ///// <typeparam name="T">Poco type</typeparam>
        ///// <param name="sqlExpr">SQL object</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The list of pocos fetched from the database.</returns>
        //public Task<IEnumerable<T>> QueryAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    return DoQueryAsync(sqlExpr, default(T), token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Tuple<List<T1>, List<T2>> FetchMultiple<T1, T2>(string sql, params object[] args)
        //{
        //    var task = FetchMultipleAsync<T1, T2>(sql, args);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>>> FetchMultipleAsync<T1, T2>(string sql, params object[] args)
        //{
        //    return FetchMultipleAsync<T1, T2>(default(CancellationToken), sql, args);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>>> FetchMultipleAsync<T1, T2>(CancellationToken token, string sql, params object[] args)
        //{
        //    return FetchMultipleAsync<T1, T2, IDoNotMap, IDoNotMap, Tuple<List<T1>, List<T2>>>(
        //        new Func<List<T1>, List<T2>, Tuple<List<T1>, List<T2>>>(
        //            (t1, t2) => new Tuple<List<T1>, List<T2>>(t1, t2)),
        //        new SqlExpression(sql, args), token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Tuple<List<T1>, List<T2>, List<T3>> FetchMultiple<T1, T2, T3>(string sql, params object[] args)
        //{
        //    var task = FetchMultipleAsync<T1, T2, T3>(sql, args);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>, List<T3>>> FetchMultipleAsync<T1, T2, T3>(string sql, params object[] args)
        //{
        //    return FetchMultipleAsync<T1, T2, T3>(default(CancellationToken), sql, args);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>, List<T3>>> FetchMultipleAsync<T1, T2, T3>(
        //    CancellationToken token, string sql, params object[] args)
        //{
        //    return FetchMultipleAsync<T1, T2, T3, IDoNotMap, Tuple<List<T1>, List<T2>, List<T3>>>(
        //        new Func<List<T1>, List<T2>, List<T3>, Tuple<List<T1>, List<T2>, List<T3>>>(
        //            (t1, t2, t3) => new Tuple<List<T1>, List<T2>, List<T3>>(t1, t2, t3)),
        //        new SqlExpression(sql, args), token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="T4">Data record type #4</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Tuple<List<T1>, List<T2>, List<T3>, List<T4>> FetchMultiple<T1, T2, T3, T4>(string sql, params object[] args)
        //{
        //    var task = FetchMultipleAsync<T1, T2, T3, T4>(sql, args);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="T4">Data record type #4</typeparam>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(string sql, params object[] args)
        //{
        //    return FetchMultipleAsync<T1, T2, T3, T4>(default(CancellationToken), sql, args);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="T4">Data record type #4</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="sql">SQL batch</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(
        //    CancellationToken token, string sql, params object[] args)
        //{
        //    return FetchMultipleAsync<T1, T2, T3, T4, Tuple<List<T1>, List<T2>, List<T3>, List<T4>>>(
        //        new Func<List<T1>, List<T2>, List<T3>, List<T4>, Tuple<List<T1>, List<T2>, List<T3>, List<T4>>>(
        //            (t1, t2, t3, t4) => new Tuple<List<T1>, List<T2>, List<T3>, List<T4>>(t1, t2, t3, t4)),
        //        new SqlExpression(sql, args), token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Tuple<List<T1>, List<T2>> FetchMultiple<T1, T2>(SqlExpression sqlExpr)
        //{
        //    var task = FetchMultipleAsync<T1, T2>(sqlExpr);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>>> FetchMultipleAsync<T1, T2>(SqlExpression sqlExpr,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return FetchMultipleAsync<T1, T2, IDoNotMap, IDoNotMap, Tuple<List<T1>, List<T2>>>(
        //        new Func<List<T1>, List<T2>, Tuple<List<T1>, List<T2>>>(
        //            (t1, t2) => new Tuple<List<T1>, List<T2>>(t1, t2)),
        //        sqlExpr, token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Tuple<List<T1>, List<T2>, List<T3>> FetchMultiple<T1, T2, T3>(SqlExpression sqlExpr)
        //{
        //    var task = FetchMultipleAsync<T1, T2, T3>(sqlExpr);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>, List<T3>>> FetchMultipleAsync<T1, T2, T3>(
        //    SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    return FetchMultipleAsync<T1, T2, T3, IDoNotMap, Tuple<List<T1>, List<T2>, List<T3>>>(
        //        new Func<List<T1>, List<T2>, List<T3>, Tuple<List<T1>, List<T2>, List<T3>>>(
        //            (t1, t2, t3) => new Tuple<List<T1>, List<T2>, List<T3>>(t1, t2, t3)),
        //        sqlExpr, token);
        //}


        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="T4">Data record type #4</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Tuple<List<T1>, List<T2>, List<T3>, List<T4>> FetchMultiple<T1, T2, T3, T4>(SqlExpression sqlExpr)
        //{
        //    var task = FetchMultipleAsync<T1, T2, T3, T4>(sqlExpr);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="T4">Data record type #4</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        //public Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(
        //    SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        //{
        //    return FetchMultipleAsync<T1, T2, T3, T4, Tuple<List<T1>, List<T2>, List<T3>, List<T4>>>(
        //        new Func<List<T1>, List<T2>, List<T3>, List<T4>, Tuple<List<T1>, List<T2>, List<T3>, List<T4>>>(
        //            (t1, t2, t3, t4) => new Tuple<List<T1>, List<T2>, List<T3>, List<T4>>(t1, t2, t3, t4)),
        //        sqlExpr, token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="TRet">Correlated data record type</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="correlator">Function that correlates the result sets</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public TRet FetchMultiple<T1, T2, TRet>(SqlExpression sqlExpr,
        //    Func<List<T1>, List<T2>, TRet> correlator)
        //{
        //    var task = FetchMultipleAsync(sqlExpr, correlator);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="TRet">Correlated data record type</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="correlator">Function that correlates the result sets</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Task<TRet> FetchMultipleAsync<T1, T2, TRet>(SqlExpression sqlExpr,
        //    Func<List<T1>, List<T2>, TRet> correlator,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return FetchMultipleAsync<T1, T2, IDoNotMap, IDoNotMap, TRet>(correlator, sqlExpr, token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="TRet">Correlated data record type</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="correlator">Function that correlates the result sets</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public TRet FetchMultiple<T1, T2, T3, TRet>(SqlExpression sqlExpr,
        //    Func<List<T1>, List<T2>, List<T3>, TRet> correlator)
        //{
        //    var task = FetchMultipleAsync(sqlExpr, correlator);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query -- async
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="TRet">Correlated data record type</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="correlator">Function that correlates the result sets</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Task<TRet> FetchMultipleAsync<T1, T2, T3, TRet>(SqlExpression sqlExpr,
        //    Func<List<T1>, List<T2>, List<T3>, TRet> correlator,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return FetchMultipleAsync<T1, T2, T3, IDoNotMap, TRet>(correlator, sqlExpr, token);
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="T4">Data record type #3</typeparam>
        ///// <typeparam name="TRet">Correlated data record type</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="correlator">Function that correlates the result sets</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public TRet FetchMultiple<T1, T2, T3, T4, TRet>(SqlExpression sqlExpr,
        //    Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> correlator)
        //{
        //    var task = FetchMultipleAsync(sqlExpr, correlator);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches multiple result sets in one query.
        ///// </summary>
        ///// <typeparam name="T1">Data record type #1</typeparam>
        ///// <typeparam name="T2">Data record type #2</typeparam>
        ///// <typeparam name="T3">Data record type #3</typeparam>
        ///// <typeparam name="T4">Data record type #3</typeparam>
        ///// <typeparam name="TRet">Correlated data record type</typeparam>
        ///// <param name="sqlExpr">SQL Expression that defines the query</param>
        ///// <param name="correlator">Function that correlates the result sets</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        //public Task<TRet> FetchMultipleAsync<T1, T2, T3, T4, TRet>(SqlExpression sqlExpr,
        //    Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> correlator,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return FetchMultipleAsync<T1, T2, T3, T4, TRet>(correlator, sqlExpr, token);
        //}

        ///// <summary>
        ///// Executes the query 
        ///// </summary>
        ///// <typeparam name="T">Entity type of the query result</typeparam>
        ///// <param name="sqlExpr">SQL statement</param>
        ///// <param name="instance">Instance to fill with the data</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="firstOnly">Query only the first record, if found</param>
        ///// <returns>Query resultset</returns>
        //private async Task<IEnumerable<T>> DoQueryAsync<T>(SqlExpression sqlExpr, T instance = default(T),
        //    CancellationToken token = default(CancellationToken), bool firstOnly = false)
        //{
        //    sqlExpr = sqlExpr.CompleteSelect<T>();
        //    var sql = sqlExpr.SqlText;
        //    var args = sqlExpr.Arguments;

        //    await OpenSharedConnectionAsync(token);
        //    try
        //    {
        //        using (var cmd = CreateCommand(Connection, sql, args))
        //        {
        //            IDataReader r;
        //            try
        //            {
        //                r = await cmd.ExecuteReaderAsync(token);
        //                OnExecutedCommand(cmd);
        //            }
        //            catch (Exception x)
        //            {
        //                OnException(x);
        //                throw;
        //            }

        //            var records = new List<T>();
        //            using (r)
        //            {
        //                var recordFactory = DataReaderMappingManager.GetMapperFor(r, instance);
        //                while (true)
        //                {
        //                    T dataRecord;
        //                    try
        //                    {
        //                        if (!r.Read()) break;
        //                        dataRecord = recordFactory(r, instance);
        //                    }
        //                    catch (Exception x)
        //                    {
        //                        OnException(x);
        //                        throw;
        //                    }
        //                    AttachToTrackingContext(dataRecord);
        //                    records.Add(dataRecord);
        //                    if (firstOnly) break;
        //                }
        //                return records;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        CloseSharedConnection();
        //    }
        //}

        ///// <summary>
        ///// Fetches a correlated set of entities using a query returning multiple
        ///// result sets.
        ///// </summary>
        ///// <typeparam name="T1">First type</typeparam>
        ///// <typeparam name="T2">Second type</typeparam>
        ///// <typeparam name="T3">Third type</typeparam>
        ///// <typeparam name="T4">Fourth type</typeparam>
        ///// <typeparam name="TRet">Correlated return type</typeparam>
        ///// <param name="correlator">Function that correlates the result sets</param>
        ///// <param name="sqlExpr">SQL expression defyning the query</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>Correlated result set</returns>
        //private async Task<TRet> FetchMultipleAsync<T1, T2, T3, T4, TRet>(object correlator, SqlExpression sqlExpr,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    var sql = sqlExpr.SqlText;
        //    var args = sqlExpr.Arguments;

        //    var factories = new Func<IDataReader, Delegate>[]
        //        {
        //            r => DataReaderMappingManager.GetMapperFor<T1>(r),
        //            r => DataReaderMappingManager.GetMapperFor<T2>(r),
        //            r => DataReaderMappingManager.GetMapperFor<T3>(r),
        //            r => DataReaderMappingManager.GetMapperFor<T4>(r)
        //        };
        //    var setCount = typeof(T3) == typeof(IDoNotMap)
        //        ? 2
        //        : (typeof(T4) == typeof(IDoNotMap) ? 3 : 4);

        //    await OpenSharedConnectionAsync(token);
        //    try
        //    {
        //        using (var cmd = CreateCommand(Connection, sql, args))
        //        {
        //            IDataReader r;
        //            try
        //            {
        //                r = await cmd.ExecuteReaderAsync(token);
        //                OnExecutedCommand(cmd);
        //            }
        //            catch (Exception x)
        //            {
        //                OnException(x);
        //                throw;
        //            }

        //            using (r)
        //            {
        //                var typeIndex = 0;
        //                var list1 = new List<T1>();
        //                var list2 = new List<T2>();
        //                var list3 = new List<T3>();
        //                var list4 = new List<T4>();
        //                do
        //                {
        //                    if (typeIndex >= setCount) break;
        //                    var recordFactory = factories[typeIndex](r);
        //                    while (true)
        //                    {
        //                        try
        //                        {
        //                            if (!r.Read()) break;
        //                            switch (typeIndex)
        //                            {
        //                                case 0: list1.Add(((Func<IDataReader, T1, T1>)recordFactory)(r, default(T1))); break;
        //                                case 1: list2.Add(((Func<IDataReader, T2, T2>)recordFactory)(r, default(T2))); break;
        //                                case 2: list3.Add(((Func<IDataReader, T3, T3>)recordFactory)(r, default(T3))); break;
        //                                case 3: list4.Add(((Func<IDataReader, T4, T4>)recordFactory)(r, default(T4))); break;
        //                            }
        //                        }
        //                        catch (Exception x)
        //                        {
        //                            OnException(x);
        //                            throw;
        //                        }
        //                    }
        //                    typeIndex++;
        //                } while (r.NextResult());

        //                switch (setCount)
        //                {
        //                    case 2:
        //                        return ((Func<List<T1>, List<T2>, TRet>)correlator)(list1, list2);
        //                    case 3:
        //                        return ((Func<List<T1>, List<T2>, List<T3>, TRet>)correlator)(list1, list2, list3);
        //                    default:
        //                        return ((Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet>)correlator)(list1, list2, list3, list4);
        //                }
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        CloseSharedConnection();
        //    }
        //}

        //#endregion

        //#region Page operations

        ///// <summary>
        ///// Fetches a page of records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageLength">Number of items per page</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <returns>The paged result of the query.</returns>
        //public Page<T> Page<T>(long pageIndex, long pageLength, SqlExpression sql)
        //{
        //    return Page<T>(pageIndex, pageLength, sql.SqlText, sql.Arguments);
        //}

        ///// <summary>
        ///// Fetches a page of records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageLength">Number of items per page</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The paged result of the query.</returns>
        //public async Task<Page<T>> PageAsync<T>(long pageIndex, long pageLength, SqlExpression sql,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return await PageAsync<T>(token, pageIndex, pageLength, sql.SqlText, sql.Arguments);
        //}

        ///// <summary>
        ///// Fetches a page of records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageLength">Number of items per page</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The paged result of the query.</returns>
        //public Page<T> Page<T>(long pageIndex, long pageLength, string sql, params object[] args)
        //{
        //    var task = PageAsync<T>(default(CancellationToken), pageIndex, pageLength, sql, args);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches a page of records from the database with the specified SQL fragment -- async
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageLength">Number of items per page</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The paged result of the query.</returns>
        //public async Task<Page<T>> PageAsync<T>(long pageIndex, long pageLength, string sql, params object[] args)
        //{
        //    return await PageAsync<T>(default(CancellationToken), pageIndex, pageLength, sql, args);
        //}

        ///// <summary>
        ///// Fetches a page of records from the database with the specified SQL fragment -- async
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageLength">Number of items per page</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The paged result of the query.</returns>
        //public async Task<Page<T>> PageAsync<T>(CancellationToken token, long pageIndex, long pageLength, string sql, params object[] args)
        //{
        //    string sqlCount, sqlPage;
        //    BuildPageQueries<T>(pageLength * pageIndex, pageLength, sql, ref args, out sqlCount, out sqlPage);

        //    // --- Save the one-time command time out and use it for both queries
        //    var saveTimeout = OneTimeCommandTimeout;

        //    // Setup the paged result
        //    var result = new Page<T>
        //    {
        //        CurrentPage = pageIndex,
        //        ItemsPerPage = pageLength,
        //        TotalItems = await ExecuteScalarAsync<long>(token, sqlCount, args)
        //    };
        //    result.TotalPages = result.TotalItems / pageLength;
        //    if ((result.TotalItems % pageLength) != 0) result.TotalPages++;
        //    OneTimeCommandTimeout = saveTimeout;

        //    // --- Get the records
        //    result.Items = await FetchAsync<T>(token, sqlPage, args);
        //    return result;
        //}

        ///// <summary>
        ///// Fetches a chunk of records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="skip">Number of records to skip from the beginning of the record set</param>
        ///// <param name="take">Number of records to take</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <returns>The paged result of the query.</returns>
        //public List<T> SkipTake<T>(long skip, long take, SqlExpression sql)
        //{
        //    return SkipTake<T>(skip, take, sql.SqlText, sql.Arguments);
        //}

        ///// <summary>
        ///// Fetches a chunk of records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="skip">Number of records to skip from the beginning of the record set</param>
        ///// <param name="take">Number of records to take</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <returns>The paged result of the query.</returns>
        //public async Task<List<T>> SkipTakeAsync<T>(long skip, long take, SqlExpression sql,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return await SkipTakeAsync<T>(token, skip, take, sql.SqlText, sql.Arguments);
        //}
        ///// <summary>
        ///// Fetches a chunk of records from the database with the specified SQL fragment.
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="skip">Number of records to skip from the beginning of the record set</param>
        ///// <param name="take">Number of records to take</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The paged result of the query.</returns>
        //public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        //{
        //    var task = SkipTakeAsync<T>(skip, take, sql, args);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Fetches a chunk of records from the database with the specified SQL fragment -- async
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="skip">Number of records to skip from the beginning of the record set</param>
        ///// <param name="take">Number of records to take</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The paged result of the query.</returns>
        //public async Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, params object[] args)
        //{
        //    string sqlCount, sqlPage;
        //    BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
        //    return await FetchAsync<T>(sqlPage, args);
        //}

        ///// <summary>
        ///// Fetches a chunk of records from the database with the specified SQL fragment -- async
        ///// </summary>
        ///// <typeparam name="T">Type of poco to fetch.</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="skip">Number of records to skip from the beginning of the record set</param>
        ///// <param name="take">Number of records to take</param>
        ///// <param name="sql">SQL fragment</param>
        ///// <param name="args">Array of query parameters</param>
        ///// <returns>The paged result of the query.</returns>
        //public async Task<List<T>> SkipTakeAsync<T>(CancellationToken token, long skip, long take, string sql, params object[] args)
        //{
        //    string sqlCount, sqlPage;
        //    BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
        //    return await FetchAsync<T>(token, sqlPage, args);
        //}

        //#endregion

        //#region Data modification operations

        ///// <summary>
        ///// Inserts a new record into the database and retrieves the newly inserted record.
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">record instance</param>
        ///// <param name="insertIdentity">True shows that identity value is inserted explicitly</param>
        ///// <param name="withGet">True indicates that the newly inserted record should be read back</param>
        ///// <remarks>The insert and get operations are atomic.</remarks>
        //public void Insert<T>(T record, bool insertIdentity = false, bool withGet = true)
        //{
        //    var task = InsertAsync(record, insertIdentity, withGet);
        //    task.WaitAndUnwrapException();
        //}

        ///// <summary>
        ///// Inserts a new record into the database and retrieves the newly inserted record -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">record instance</param>
        ///// <param name="insertIdentity">True shows that identity value is inserted explicitly</param>
        ///// <param name="withGet">True indicates that the newly inserted record should be read back</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <remarks>The insert and get operations are atomic.</remarks>
        //public async Task InsertAsync<T>(T record, bool insertIdentity = false, bool withGet = true,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    // --- Read-only mode prevents this operation to run
        //    if (OperationMode == SqlOperationMode.ReadOnly)
        //    {
        //        throw new InvalidOperationException(
        //            "The database instance is in read-only mode, so INSERT is not allowed.");
        //    }

        //    // --- Do the operation
        //    try
        //    {
        //        // ReSharper disable CompareNonConstrainedGenericWithNull
        //        if (record == null) throw new ArgumentNullException("record");
        //        // ReSharper restore CompareNonConstrainedGenericWithNull

        //        // --- Prepare the SQL expression to execute
        //        var metadata = RecordMetadataManager.GetMetadata<T>();
        //        List<string> columnNames;
        //        List<string> columnValues;
        //        List<object> rawValues;
        //        string identityFieldName;
        //        PrepareInsertData(record, insertIdentity, out identityFieldName,
        //            out columnNames, out columnValues, out rawValues);

        //        // --- Start executing it
        //        await OpenSharedConnectionAsync(token);
        //        try
        //        {
        //            // --- Create and excute the insert command
        //            var shouldGetBack = withGet || IsTracked();
        //            using (var cmd = CreateCommand(Connection, ""))
        //            {
        //                var tableName = EscapeSqlTableName(metadata.SchemaName, metadata.TableName);
        //                PrepareInsertCommand(cmd, identityFieldName, insertIdentity, shouldGetBack, tableName,
        //                    columnNames, columnValues, rawValues);

        //                // --- Read back the inserted record with the OUTPUT clause
        //                if (shouldGetBack)
        //                {
        //                    IDataReader r = await cmd.ExecuteReaderAsync(token);
        //                    OnExecutedCommand(cmd);
        //                    using (r)
        //                    {
        //                        var recordFactory = DataReaderMappingManager.GetMapperFor(r, record);
        //                        r.Read();
        //                        recordFactory(r, record);
        //                    }
        //                }
        //                else
        //                {
        //                    await cmd.ExecuteNonQueryAsync(token);
        //                    OnExecutedCommand(cmd);
        //                }

        //                // --- Track insertion
        //                TrackInsert(record);
        //            }
        //        }
        //        finally
        //        {
        //            CloseSharedConnection();
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        OnException(x);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Updates the specified record in the database
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <remarks>The record must have a primary key</remarks>
        //public void Update<T>(T record)
        //{
        //    var task = UpdateInternalAsync(record, IsTracked() ? UpdateMode.GetCurrent : UpdateMode.Simple);
        //    task.WaitAndUnwrapException();
        //}

        ///// <summary>
        ///// Updates the specified record in the database -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <remarks>The record must have a primary key</remarks>
        //public async Task UpdateAsync<T>(T record, CancellationToken token = default(CancellationToken))
        //{
        //    await UpdateInternalAsync(record, IsTracked() ? UpdateMode.GetCurrent : UpdateMode.Simple, token);
        //}

        ///// <summary>
        ///// Updates the specified record in the database and retrieves the record
        ///// before the modification
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <remarks>The recors must have a primary key</remarks>
        //public T UpdateAndGetPrevious<T>(T record)
        //{
        //    var task = UpdateInternalAsync(record, UpdateMode.GetPrevious);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Updates the specified record in the database and retrieves the record -- async
        ///// before the modification
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <remarks>The recors must have a primary key</remarks>
        //public async Task<T> UpdateAndGetPreviousAsync<T>(T record, CancellationToken token = default(CancellationToken))
        //{
        //    var outRecord = await UpdateInternalAsync(record, UpdateMode.GetPrevious, token);
        //    return outRecord;
        //}

        ///// <summary>
        ///// Updates the specified record in the database and retrieves the record
        ///// after the modification
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <remarks>The recors must have a primary key</remarks>
        //public T UpdateAndGetCurrent<T>(T record)
        //{
        //    var task = UpdateInternalAsync(record, UpdateMode.GetCurrent);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Updates the specified record in the database and retrieves the record -- async
        ///// after the modification
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <remarks>The recors must have a primary key</remarks>
        //public async Task<T> UpdateAndGetCurrentAsync<T>(T record, CancellationToken token = default(CancellationToken))
        //{
        //    var outRecord = await UpdateInternalAsync(record, UpdateMode.GetCurrent, token);
        //    return outRecord;
        //}

        ///// <summary>
        ///// Implements the update operation
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <param name="mode">Update mode to apply</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <remarks>The record must have a primary key</remarks>
        //private async Task<T> UpdateInternalAsync<T>(T record, UpdateMode mode, CancellationToken token = default(CancellationToken))
        //{
        //    // --- Read-only mode prevents this operation to run
        //    if (OperationMode == SqlOperationMode.ReadOnly)
        //    {
        //        throw new InvalidOperationException(
        //            "The database instance is in read-only mode, so UPDATE is not allowed.");
        //    }

        //    // --- Do the operation
        //    var outRecord = default(T);
        //    try
        //    {
        //        // ReSharper disable CompareNonConstrainedGenericWithNull
        //        if (record == null) throw new ArgumentNullException("record");
        //        // ReSharper restore CompareNonConstrainedGenericWithNull

        //        // --- Prepare the SQL expression to execute
        //        var metadata = RecordMetadataManager.GetMetadata<T>();

        //        // --- Check if data record is used
        //        if (metadata.IsSimplePoco)
        //        {
        //            throw new InvalidOperationException(
        //                string.Format("Type {0} is not a data record type, it cannot be used in Update<> operation.", typeof(T)));
        //        }

        //        // --- Check for primary key existence
        //        if (metadata.PrimaryKeyColumns.Count == 0)
        //        {
        //            throw new InvalidOperationException(
        //                string.Format("No primary key values found for {0}, so it can be used in UPDATE.", typeof(T)));
        //        }

        //        // --- Check for update column specification
        //        var recordAsDataRecord = record as IDataRecord;
        //        // ReSharper disable PossibleNullReferenceException
        //        var modifiedColumns = recordAsDataRecord.GetModifiedColumns();
        //        // ReSharper restore PossibleNullReferenceException

        //        // --- Prepare UPDATE
        //        var sql = new StringBuilder();
        //        var tableName = EscapeSqlTableName(metadata.SchemaName, metadata.TableName);
        //        sql.AppendFormat("update {0} set ", tableName);

        //        // --- Prepare SET columns
        //        var columnIndex = 0;
        //        var rawValues = new List<object>();
        //        foreach (var column in modifiedColumns)
        //        {
        //            var dataColumn = metadata[column];
        //            if (dataColumn.IsVersionColumn) continue;

        //            // --- Obtain column value
        //            var value = dataColumn.PropertyInfo.GetValue(record);

        //            // --- Use a custom target converter if provided
        //            if (dataColumn.TargetConverter != null)
        //            {
        //                value = dataColumn.TargetConverter.ConvertToDataType(value);
        //            }
        //            value = FixDateTimeValue(value);
        //            if (columnIndex > 0) sql.Append(", ");
        //            rawValues.Add(value);
        //            sql.AppendFormat("{0} = @{1}", EscapeSqlIdentifier(dataColumn.ColumnName), columnIndex++);
        //        }

        //        // --- Prepare the OUTPUT clause
        //        switch (mode)
        //        {
        //            case UpdateMode.GetPrevious:
        //                sql.Append(" output deleted.* ");
        //                break;
        //            case UpdateMode.GetCurrent:
        //                sql.Append(" output inserted.* ");
        //                break;
        //        }

        //        // --- Prepare the WHERE clause
        //        sql.Append(" where ");
        //        var pkClauseAdded = false;
        //        foreach (var pkColumn in metadata.PrimaryKeyColumns)
        //        {
        //            var pkValue = pkColumn.PropertyInfo.GetValue(record);
        //            if (pkClauseAdded) sql.Append(" and ");
        //            sql.AppendFormat("{0} = @{1}", EscapeSqlIdentifier(pkColumn.ColumnName), columnIndex++);
        //            pkClauseAdded = true;
        //            rawValues.Add(pkValue);
        //        }

        //        // --- Prepare version column where clause
        //        var versionColumn = metadata.DataColumns.FirstOrDefault(c => c.IsVersionColumn);
        //        if (RowVersionHandling != SqlRowVersionHandling.DoNotUseVersions && versionColumn != null)
        //        {
        //            sql.AppendFormat(" and {0} = @{1}", EscapeSqlIdentifier(versionColumn.ColumnName), columnIndex);
        //            rawValues.Add(versionColumn.PropertyInfo.GetValue(record));
        //        }

        //        await OpenSharedConnectionAsync(token);
        //        try
        //        {
        //            var resultCount = 0;

        //            // --- Execute the command
        //            var toTrack = record;
        //            using (var cmd = CreateCommand(Connection, sql.ToString(), rawValues.ToArray()))
        //            {
        //                if (mode == UpdateMode.Simple)
        //                {
        //                    resultCount = cmd.ExecuteNonQuery();
        //                    OnExecutedCommand(cmd);
        //                }
        //                else
        //                {
        //                    IDataReader r = await cmd.ExecuteReaderAsync(token);
        //                    OnExecutedCommand(cmd);
        //                    using (r)
        //                    {
        //                        var recordFactory = DataReaderMappingManager.GetMapperFor(r, record);
        //                        if (r.Read())
        //                        {
        //                            resultCount = 1;
        //                            outRecord = Activator.CreateInstance<T>();
        //                            recordFactory(r, outRecord);
        //                            if (mode == UpdateMode.GetCurrent) toTrack = outRecord;
        //                        }
        //                    }
        //                }
        //            }

        //            // --- Check for concurrency issues
        //            if (resultCount == 0 && versionColumn != null && RowVersionHandling == SqlRowVersionHandling.RaiseException)
        //            {
        //                throw new DBConcurrencyException(
        //                    string.Format("A Concurrency update occurred in table '{0}' for primary key " +
        //                        "value(s) = '{1}' and version = '{2}'",
        //                        tableName,
        //                        string.Join(", ", metadata.PrimaryKeyColumns.Select(col => col.PropertyInfo.GetValue(record)).ToArray()),
        //                        TypeConversionHelper.ByteArrayToString((byte[])versionColumn.PropertyInfo.GetValue(record))));
        //            }

        //            // --- Track the record update
        //            TrackUpdate(toTrack);
        //        }
        //        finally
        //        {
        //            CloseSharedConnection();
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        OnException(x);
        //        throw;
        //    }
        //    return outRecord;
        //}

        ///// <summary>
        ///// Deletes the specified record from the database
        ///// </summary>
        ///// <typeparam name="T">Reocord type</typeparam>
        ///// <param name="record">Record to delete</param>
        //public bool Delete<T>(T record)
        //{
        //    var task = DeleteInternalAsync(record, IsTracked());
        //    task.WaitAndUnwrapException();
        //    return task.Result.Item1;
        //}

        ///// <summary>
        ///// Deletes the specified record from the database -- async
        ///// </summary>
        ///// <typeparam name="T">Reocord type</typeparam>
        ///// <param name="record">Record to delete</param>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task<bool> DeleteAsync<T>(T record, CancellationToken token = default(CancellationToken))
        //{
        //    return (await DeleteInternalAsync(record, IsTracked(), token)).Item1;
        //}

        ///// <summary>
        ///// Deletes the specified record from the database and retrieves
        ///// the record before the deletion.
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="record">Record to delete</param>
        //public T DeleteAndGetPrevious<T>(T record)
        //{
        //    var task = DeleteInternalAsync(record, true);
        //    task.WaitAndUnwrapException();
        //    return task.Result.Item2;
        //}

        ///// <summary>
        ///// Deletes the specified record from the database and retrieves -- async
        ///// the record before the deletion.
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="record">Record to delete</param>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task<T> DeleteAndGetPreviousAsync<T>(T record, CancellationToken token = default(CancellationToken))
        //{
        //    var result = await DeleteInternalAsync(record, true, token);
        //    return result.Item2;
        //}

        ///// <summary>
        ///// Deletes the specified record from the database
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        //public bool DeleteById<T>(object pkFirst, params object[] pkOthers)
        //{
        //    var task = DeleteByIdAsync<T>(pkFirst, pkOthers);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Deletes the specified record from the database -- async
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        //public async Task<bool> DeleteByIdAsync<T>(object pkFirst, params object[] pkOthers)
        //{
        //    var pkValues = new object[pkOthers.Length + 1];
        //    pkValues[0] = pkFirst;
        //    for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
        //    return await DeleteByIdAsync<T>(pkValues);
        //}

        ///// <summary>
        ///// Deletes the specified record from the database -- async
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="token">Optional cancellation token</param>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        //public async Task<bool> DeleteByIdAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        //{
        //    var pkValues = new object[pkOthers.Length + 1];
        //    pkValues[0] = pkFirst;
        //    for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
        //    return await DeleteByIdAsync<T>(pkValues, token);
        //}

        ///// <summary>
        ///// Deletes a record from the database using its primary key values
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkValues">Collection of primary key values</param>
        //public bool DeleteById<T>(IEnumerable<object> pkValues)
        //{
        //    var task = DeleteByIdAsync<T>(pkValues);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Deletes a record from the database using its primary key values
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkValues">Collection of primary key values</param>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task<bool> DeleteByIdAsync<T>(IEnumerable<object> pkValues,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    return (await DeleteInternalAsync<T>(pkValues, IsTracked(), token)).Item1;
        //}

        ///// <summary>
        ///// Deletes the specified record from the database
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        //public T DeleteByIdAndGetPrevious<T>(object pkFirst, params object[] pkOthers)
        //{
        //    var pkValues = new object[pkOthers.Length + 1];
        //    pkValues[0] = pkFirst;
        //    for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
        //    return DeleteByIdAndGetPrevious<T>(pkValues);
        //}

        ///// <summary>
        ///// Deletes the specified record from the database -- async
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        //public async Task<T> DeleteByIdAndGetPreviousAsync<T>(object pkFirst, params object[] pkOthers)
        //{
        //    return await DeleteByIdAndGetPreviousAsync<T>(default(CancellationToken), pkFirst, pkOthers);
        //}

        ///// <summary>
        ///// Deletes the specified record from the database -- async
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="token">Cancellation token</param>
        ///// <param name="pkFirst">First element of the primary key</param>
        ///// <param name="pkOthers">Other elements of the primary key</param>
        //public async Task<T> DeleteByIdAndGetPreviousAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        //{
        //    var pkValues = new object[pkOthers.Length + 1];
        //    pkValues[0] = pkFirst;
        //    for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
        //    return await DeleteByIdAndGetPreviousAsync<T>(pkValues, token);
        //}

        ///// <summary>
        ///// Deletes a record from the database using its primary key values
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkValues">Collection of primary key values</param>
        //public T DeleteByIdAndGetPrevious<T>(IEnumerable<object> pkValues)
        //{
        //    var task = DeleteByIdAndGetPreviousAsync<T>(pkValues);
        //    task.WaitAndUnwrapException();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Deletes a record from the database using its primary key values
        ///// </summary>
        ///// <typeparam name="T">Reocrd type</typeparam>
        ///// <param name="pkValues">Collection of primary key values</param>
        ///// <param name="token">Optional cancellation token</param>
        //public async Task<T> DeleteByIdAndGetPreviousAsync<T>(IEnumerable<object> pkValues,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    var result = await DeleteInternalAsync<T>(pkValues, true, token);
        //    return result.Item2;
        //}

        ///// <summary>
        ///// Implements the delete operation -- async
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="pkValues">Collection of primary key values</param>
        ///// <param name="withGet">Should get the previous record?</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <remarks>The record must have a primary key</remarks>
        //private async Task<Tuple<bool, T>> DeleteInternalAsync<T>(IEnumerable<object> pkValues, bool withGet,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    // --- Read-only mode prevents this operation to run
        //    if (OperationMode == SqlOperationMode.ReadOnly)
        //    {
        //        throw new InvalidOperationException(
        //            "The database instance is in read-only mode, so DELETE is not allowed.");
        //    }

        //    // --- Do the operation
        //    if (pkValues == null) throw new ArgumentNullException("pkValues");
        //    var values = pkValues.ToList();
        //    try
        //    {
        //        // --- Prepare the SQL expression to execute
        //        var metadata = RecordMetadataManager.GetMetadata<T>();

        //        // --- Check if data record is used
        //        if (metadata.IsSimplePoco)
        //        {
        //            throw new InvalidOperationException(
        //                string.Format("Type {0} is not a data record type, it cannot be used in a Delete<> operation.", typeof(T)));
        //        }

        //        // --- Check for primary key existence
        //        if (metadata.PrimaryKeyColumns.Count == 0)
        //        {
        //            throw new InvalidOperationException(
        //                string.Format("No primary key values found for {0}, so it can be used in DELETE.", typeof(T)));
        //        }

        //        // --- Prepare DELETE
        //        var sql = new StringBuilder();
        //        var tableName = EscapeSqlTableName(metadata.SchemaName, metadata.TableName);
        //        sql.AppendFormat("delete from {0}", tableName);

        //        // --- Prepare the OUTPUT clause
        //        var shouldGetBack = withGet || IsTracked();
        //        if (shouldGetBack)
        //        {
        //            sql.Append(" output deleted.*");
        //        }

        //        // --- Prepare the WHERE clause
        //        var columnIndex = 0;
        //        var rawValues = new List<object>();
        //        sql.Append(" where ");
        //        var pkClauseAdded = false;
        //        foreach (var pkColumn in metadata.PrimaryKeyColumns)
        //        {
        //            var pkValue = values[columnIndex];
        //            if (pkClauseAdded) sql.Append(" and ");
        //            sql.AppendFormat("{0} = @{1}", EscapeSqlIdentifier(pkColumn.ColumnName), columnIndex++);
        //            pkClauseAdded = true;
        //            rawValues.Add(pkValue);
        //        }

        //        await OpenSharedConnectionAsync(token);
        //        try
        //        {
        //            // --- Execute the command
        //            using (var cmd = CreateCommand(Connection, sql.ToString(), rawValues.ToArray()))
        //            {
        //                if (!shouldGetBack)
        //                {
        //                    var rows = await cmd.ExecuteNonQueryAsync(token);
        //                    OnExecutedCommand(cmd);
        //                    return new Tuple<bool, T>(rows > 0, default(T));
        //                }
        //                else
        //                {
        //                    IDataReader r = await cmd.ExecuteReaderAsync(token);
        //                    OnExecutedCommand(cmd);
        //                    using (r)
        //                    {
        //                        var tempRecord = Activator.CreateInstance<T>();
        //                        var recordFactory = DataReaderMappingManager.GetMapperFor(r, tempRecord);
        //                        if (r.Read())
        //                        {
        //                            var prevRecord = Activator.CreateInstance<T>();
        //                            recordFactory(r, prevRecord);
        //                            TrackDelete(prevRecord);
        //                            return new Tuple<bool, T>(true, prevRecord);
        //                        }
        //                        else
        //                        {
        //                            return new Tuple<bool, T>(false, default(T));
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            CloseSharedConnection();
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        OnException(x);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Implements the delete operation
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <param name="withGet">Should get the previous record?</param>
        ///// <param name="token">Optional cancellation token</param>
        ///// <remarks>The record must have a primary key</remarks>
        //private async Task<Tuple<bool, T>> DeleteInternalAsync<T>(T record, bool withGet,
        //    CancellationToken token = default(CancellationToken))
        //{
        //    // ReSharper disable CompareNonConstrainedGenericWithNull
        //    if (record == null)
        //    // ReSharper restore CompareNonConstrainedGenericWithNull
        //    {
        //        throw new ArgumentNullException("record");
        //    }
        //    return await DeleteInternalAsync<T>(GetPrimaryKeyValue(record), withGet, token);
        //}

        //#endregion

        //#region Miscellaneous methods

        ///// <summary>
        ///// Gets the escaped column name of the specified identifier
        ///// </summary>
        ///// <param name="identifier">SQL identifier</param>
        ///// <returns>Escaped name</returns>
        //public static string EscapeSqlIdentifier(string identifier)
        //{
        //    return string.Format("[{0}]", identifier);
        //}

        /// <summary>
        /// Gets the escaped SQL name of the specified table
        /// </summary>
        /// <param name="schemaName">SQL schema name</param>
        /// <param name="tableName">SQL table name</param>
        /// <returns>Escaped table name</returns>
        public static string EscapeSqlTableName(string schemaName, string tableName)
        {
            return string.Format("[{0}].[{1}]", schemaName ?? "dbo", tableName);
        }

        /// <summary>
        /// Gets the escaped SQL name of the table specified by record type
        /// </summary>
        /// <returns>Escaped table name</returns>
        public static string EscapeSqlTableName<T>()
        {
            var metadata = RecordMetadataManager.GetMetadata<T>();
            return EscapeSqlTableName(metadata.SchemaName, metadata.TableName);
        }

        //#endregion

        //#region Change tracking

        /// <summary>
        /// Gets the flag indicating if this database is tracked for modifications
        /// </summary>
        /// <returns></returns>
        public bool IsTracked()
        {
            return (int)OperationMode >= (int)SqlOperationMode.Tracked;
        }

        ///// <summary>
        ///// Attaches the specified record to the tracking context
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Data record instance</param>
        //public void AttachToTrackingContext<T>(T record)
        //{
        //    TrackRecord(record, ChangedRecordState.Attached);
        //}

        ///// <summary>
        ///// Tracks the insertion of the specified record
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Data record instance</param>
        //public void TrackInsert<T>(T record)
        //{
        //    TrackRecord(record, ChangedRecordState.Inserted);
        //}

        ///// <summary>
        ///// Tracks the modification of the specified record
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Data record instance</param>
        //public void TrackUpdate<T>(T record)
        //{
        //    TrackRecord(record, ChangedRecordState.Updated);
        //}

        ///// <summary>
        ///// Tracks the deletion of the specified record
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Data record instance</param>
        //public void TrackDelete<T>(T record)
        //{
        //    TrackRecord(record, ChangedRecordState.Deleted);
        //}

        ///// <summary>
        ///// Tracks the specified record with the given state type
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Data record instance</param>
        ///// <param name="state">State type</param>
        //private void TrackRecord<T>(T record, ChangedRecordState state)
        //{
        //    if (!IsTracked()) return;
        //    var dataRecord = record as IDataRecord;
        //    // ReSharper disable PossibleNullReferenceException
        //    _changes.Add(new Tuple<IDataRecord, ChangedRecordState>(dataRecord.Clone(), state));
        //    // ReSharper restore PossibleNullReferenceException
        //}

        //#endregion

        //#region Helpers

        ///// <summary>
        ///// Prepares the values for the INSERT statement
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">Record instance</param>
        ///// <param name="insertIdentity">Should use identity insertion?</param>
        ///// <param name="identityFieldName">Name of the identity field</param>
        ///// <param name="columnNames">Column names list</param>
        ///// <param name="columnValues">Column parameters list</param>
        ///// <param name="rawValues">Raw column values</param>
        //private static void PrepareInsertData<T>(T record, bool insertIdentity, out string identityFieldName,
        //    out List<string> columnNames, out List<string> columnValues, out List<object> rawValues)
        //{
        //    var metadata = RecordMetadataManager.GetMetadata<T>();
        //    columnNames = new List<string>();
        //    columnValues = new List<string>();
        //    rawValues = new List<object>();
        //    identityFieldName = null;
        //    var columnIndex = 0;
        //    foreach (var column in metadata.DataColumns)
        //    {
        //        // --- Don't insert calculated columns and version columns
        //        if (column.IsCalculated || column.IsVersionColumn) continue;

        //        // --- Check for identity field
        //        if (column.IsAutoGenerated)
        //        {
        //            identityFieldName = column.ColumnName;
        //            if (!insertIdentity) continue;
        //        }

        //        // --- Obtain column value
        //        var val = column.PropertyInfo.GetValue(record);

        //        // --- Use a custom target converter if provided
        //        if (column.TargetConverter != null)
        //        {
        //            val = column.TargetConverter.ConvertToDataType(val);
        //        }
        //        val = FixDateTimeValue(val);

        //        // --- Add column info
        //        columnNames.Add(EscapeSqlIdentifier(column.ColumnName));
        //        columnValues.Add(string.Format("{0}{1}", PARAM_PREFIX, columnIndex++));
        //        rawValues.Add(val);
        //    }
        //}

        ///// <summary>
        ///// Prepares the specified SQL command for insert operation
        ///// </summary>
        ///// <param name="cmd">SQL command instance</param>
        ///// <param name="identityFieldName">Name of the identity field</param>
        ///// <param name="insertIdentity">Should the identity value explicitly inserted?</param>
        ///// <param name="useOutput">Should the output of the INSERT statement used?</param>
        ///// <param name="tableName">Name of the table to insert into</param>
        ///// <param name="columnNames">Column names</param>
        ///// <param name="columnValues">Column parameter names</param>
        ///// <param name="rawValues">Column values</param>
        //private void PrepareInsertCommand(SqlCommand cmd, string identityFieldName, bool insertIdentity,
        //    bool useOutput, string tableName, List<string> columnNames, List<string> columnValues, List<object> rawValues)
        //{
        //    var sql = new StringBuilder();
        //    if (identityFieldName != null && insertIdentity)
        //    {
        //        sql.AppendFormat("set identity_insert {0} on\n", tableName);
        //    }
        //    sql.Append(columnNames.Count > 0
        //                   ? string.Format("insert into {0} ({1}){2} values ({3})",
        //                                   tableName,
        //                                   string.Join(", ", columnNames.ToArray()),
        //                                   useOutput ? " output inserted.*" : "",
        //                                   string.Join(", ", columnValues.ToArray()))
        //                   : string.Format("insert into {0} default values",
        //                                   tableName));
        //    if (identityFieldName != null)
        //    {
        //        if (insertIdentity) sql.AppendFormat("set identity_insert {0} off\n", tableName);
        //        if (!useOutput) sql.Append("\nselect scope_identity() as NewID");
        //    }
        //    cmd.CommandText = sql.ToString();
        //    rawValues.ForEach(par => AddParam(cmd, par));
        //}

        /// <summary>
        /// Internal helper to cleanup transaction stuff -- async
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        private async Task CleanupTransactionAsync(CancellationToken token = default(CancellationToken))
        {
            OnEndTransaction();
            if (_transactionCancelled)
            {
                Transaction.Rollback();
            }
            else
            {
                Transaction.Commit();
            }
            Transaction.Dispose();
            Transaction = null;
            await CloseSharedConnectionAsync(token);
        }

        ///// <summary>
        ///// Creates a new <see cref="SqlCommand"/> instance
        ///// </summary>
        ///// <param name="connection">Connection to use</param>
        ///// <param name="sql">SQL command text</param>
        ///// <param name="args">SQL command arguments</param>
        ///// <returns></returns>
        //private SqlCommand CreateCommand(SqlConnection connection, string sql, params object[] args)
        //{
        //    // --- Perform parameter prefix replacements
        //    sql = sql.Replace("@@", "@");

        //    // --- Create the command and add parameters
        //    var command = connection.CreateCommand();
        //    command.Connection = connection;
        //    command.CommandText = sql;
        //    command.Transaction = Transaction;
        //    foreach (var item in args) AddParam(command, item);
        //    if (!String.IsNullOrEmpty(sql)) DoPreExecute(command);
        //    return command;
        //}

        ///// <summary>
        ///// Adds a parameter to a database command
        ///// </summary>
        ///// <param name="cmd">Command to add the parameter for</param>
        ///// <param name="item">Parameter object</param>
        //private void AddParam(SqlCommand cmd, object item)
        //{
        //    var mapper = ParameterMapper ?? new DefaultSqlParameterMapper();
        //    var parameterName = string.Format("{0}{1}", PARAM_PREFIX, cmd.Parameters.Count);
        //    var parameter = mapper.MapParameterValue(parameterName, item);
        //    if (parameter == null)
        //    {
        //        throw new InvalidOperationException(
        //            string.Format("The specified item with type '{0}' cannot be mapped to an SqlParameter",
        //            item.GetType().FullName));
        //    }
        //    cmd.Parameters.Add(parameter);
        //}

        ///// <summary>
        ///// Prepares a command to execute
        ///// </summary>
        ///// <param name="command">Command to execute</param>
        //private void DoPreExecute(SqlCommand command)
        //{
        //    // --- Setup command timeout
        //    if (OneTimeCommandTimeout != 0)
        //    {
        //        command.CommandTimeout = OneTimeCommandTimeout;
        //        OneTimeCommandTimeout = 0;
        //    }
        //    else if (CommandTimeout != 0)
        //    {
        //        command.CommandTimeout = CommandTimeout;
        //    }

        //    // --- Call hook
        //    OnExecutingCommand(command);

        //    // --- Save it
        //    LastSql = command.CommandText;
        //    LastArgs = (from SqlParameter parameter in command.Parameters select parameter.Value).ToArray();
        //}

        ///// <summary>
        ///// Fixes an SQL DateTime value to the acceptable minimum value
        ///// </summary>
        ///// <param name="value">value to fix</param>
        ///// <returns>Fixed DateTime value</returns>
        //public static object FixDateTimeValue(object value)
        //{
        //    // --- Manage DateTime issues with SQL Server
        //    if (!(value is DateTime)) return value;
        //    var dateTimeValue = (DateTime)value;
        //    if (dateTimeValue.Year <= 1754)
        //    {
        //        value = new DateTime(1754, 1, 1);
        //    }
        //    return value;
        //}

        ///// <summary>
        ///// Builds page queries with the specified parameters.
        ///// </summary>
        ///// <typeparam name="T">Entity type retrieved from the query</typeparam>
        ///// <param name="skip">Number of items to skip</param>
        ///// <param name="take">Number of items to take</param>
        ///// <param name="sql">SQL string</param>
        ///// <param name="args">SQL arguments</param>
        ///// <param name="sqlCount">SQL query for obtaing the record count</param>
        ///// <param name="sqlPage">SQL query obtaining the page.</param>
        //private static void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args,
        //    out string sqlCount, out string sqlPage)
        //{
        //    // --- Add auto select clause
        //    sql = AddSelectClause<T>(sql);

        //    // --- Split the SQL into the bits we need
        //    string sqlSelectRemoved;
        //    string sqlOrderBy;
        //    if (!SplitSqlForPaging(sql, out sqlCount, out sqlSelectRemoved, out sqlOrderBy))
        //    {
        //        throw new InvalidOperationException("Unable to parse SQL statement for paged query");
        //    }

        //    // --- Build the SQL for the actual final result
        //    sqlSelectRemoved = s_RxOrderBy.Replace(sqlSelectRemoved, "");
        //    if (s_RxDistinct.IsMatch(sqlSelectRemoved))
        //    {
        //        sqlSelectRemoved = "paging_inner.* from (select " + sqlSelectRemoved + ") paging_inner";
        //    }
        //    sqlPage =
        //        string.Format(
        //            "select * from (select row_number() over ({0}) row_number, {1}) " +
        //            "paged_select where row_number>@{2} AND row_number<=@{3}",
        //            sqlOrderBy ?? "order by (select null)",
        //            sqlSelectRemoved, args.Length, args.Length + 1);
        //    args = args.Concat(new object[] { skip, skip + take }).ToArray();
        //}

        ///// <summary>
        ///// Splits an SQL string for paging.
        ///// </summary>
        ///// <param name="sql">Original SQL statement</param>
        ///// <param name="sqlCount">SQL statement for obtaining the record count</param>
        ///// <param name="sqlSelectRemoved">SQL statement withou the select clause</param>
        ///// <param name="sqlOrderBy">SQL ORDER BY clause</param>
        ///// <returns>True, if successfully cut; otherwise, false.</returns>
        //private static bool SplitSqlForPaging(string sql, out string sqlCount,
        //    out string sqlSelectRemoved, out string sqlOrderBy)
        //{
        //    sqlSelectRemoved = null;
        //    sqlCount = null;
        //    sqlOrderBy = null;

        //    // --- Extract the columns from "SELECT <whatever> FROM"
        //    var m = s_RxColumns.Match(sql);
        //    if (!m.Success) return false;

        //    // --- Save column list and replace with COUNT(*)
        //    var g = m.Groups[1];
        //    sqlSelectRemoved = sql.Substring(g.Index);

        //    if (s_RxDistinct.IsMatch(sqlSelectRemoved))
        //        sqlCount = sql.Substring(0, g.Index) + "count(" + m.Groups[1].ToString().Trim() + ") " + sql.Substring(g.Index + g.Length);
        //    else
        //        sqlCount = sql.Substring(0, g.Index) + "count(*) " + sql.Substring(g.Index + g.Length);

        //    // --- Look for an "ORDER BY <whatever>" clause
        //    m = s_RxOrderBy.Match(sqlCount);
        //    if (!m.Success) return true;
        //    g = m.Groups[0];
        //    sqlOrderBy = g.ToString();
        //    sqlCount = sqlCount.Substring(0, g.Index) + sqlCount.Substring(g.Index + g.Length);
        //    return true;
        //}

        ///// <summary>
        ///// Adds a SELECT clause to the specified SQL string
        ///// </summary>
        ///// <typeparam name="T">Type representing the result of a query</typeparam>
        ///// <param name="sql">SQL string</param>
        ///// <returns></returns>
        //static string AddSelectClause<T>(string sql)
        //{
        //    if (s_RxSelect.IsMatch(sql)) return sql;
        //    var pd = RecordMetadataManager.GetMetadata<T>();
        //    var tableName = EscapeSqlTableName(pd.SchemaName, pd.TableName);
        //    var cols = string.Join(", ", pd.DataColumns.Select(dc => dc.ColumnName).ToArray());
        //    sql = !s_RxFrom.IsMatch(sql)
        //        ? string.Format("select {0} from {1} {2}", cols, tableName, sql)
        //        : string.Format("select {0} {1}", cols, sql);
        //    return sql;
        //}

        ///// <summary>
        ///// Gets the primary key values of the specified record.
        ///// </summary>
        ///// <typeparam name="T">Record type</typeparam>
        ///// <param name="record">record instance</param>
        ///// <returns></returns>
        //private static IEnumerable<object> GetPrimaryKeyValue<T>(T record)
        //{
        //    return RecordMetadataManager
        //        .GetMetadata<T>()
        //        .PrimaryKeyColumns.Select(c => c.PropertyInfo.GetValue(record));
        //}

        //[ExcludeFromCodeCoverage]
        //private static void CloseBrokenConnection(IDbConnection connection)
        //{
        //    if (connection.State == ConnectionState.Broken) connection.Close();
        //}

        ///// <summary>
        ///// This markup interface is used in multiple fetch operations to sign that a certain
        ///// type is not mapped.
        ///// </summary>
        //private interface IDoNotMap
        //{
        //}

        ///// <summary>
        ///// This enum defines the modes the update operation should work internally
        ///// </summary>
        //private enum UpdateMode
        //{
        //    Simple = 0,
        //    GetPrevious,
        //    GetCurrent
        //}

        //#endregion
    }
}