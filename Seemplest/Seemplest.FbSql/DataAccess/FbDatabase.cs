using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        private const int DEFAULT_VARCHAR_LENGTH = 255;
        private const int BLOB_LENGTH = 10900;

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
            new Regex(@"order\s+by\s+""*\w+""*(\s+asc|\s+desc)?([\s,]*""*\w+""*(\s+asc|\s+desc)?)*$",
              RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        //static readonly Regex s_RxOrderBy =
        //    new Regex(@"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
        //      RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

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
        public SqlOperationMode OperationMode { get; }

        /// <summary>
        /// Gets the direct Execute mode of this instance
        /// </summary>
        public SqlDirectExecuteMode DirectExecuteMode { get; }

        /// <summary>
        /// Gets the handling mode of rowversion columns
        /// </summary>
        public SqlRowVersionHandling RowVersionHandling { get; }

        /// <summary>
        /// Gets the connection name used to obtain the connection string to
        /// the SQL server database
        /// </summary>
        public string ConnectionOrName { get; private set; }

        /// <summary>
        /// Gets the connection string to the SQL Server database
        /// </summary>
        public string ConnectionString { get; }

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

        #region Lifecycle management

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
        }

        #endregion

        #region Connection and transaction management

        /// <summary>
        /// Opens a shared connection that allows nesting.
        /// </summary>
        public void OpenSharedConnection()
        {
            var task = OpenSharedConnectionAsync();
            task.WaitAndUnwrapException();
        }

        /// <summary>
        /// Opens a shared connection that allows nesting -- async.
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        public async Task OpenSharedConnectionAsync(CancellationToken token = default(CancellationToken))
        {
            if (SharedConnectionDepth == 0)
            {
                // --- There is no open connection
                Connection = new FbConnection(ConnectionString);
                CloseBrokenConnection(Connection);
                if (Connection.State == ConnectionState.Closed)
                {
                    await Connection.OpenAsync(token);
                }
                Connection = OnConnectionOpened(Connection);
            }
            SharedConnectionDepth++;
        }

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

        /// <summary>
        /// Starts a new transaction with the specified optional isolation level.
        /// </summary>
        /// <param name="isolationLevel">Isolation level to use.</param>
        public void BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            var task = BeginTransactionAsync(isolationLevel);
            task.WaitAndUnwrapException();
        }

        /// <summary>
        /// Starts a new transaction with the specified optional isolation level.
        /// </summary>
        /// <param name="isolationLevel">Isolation level to use.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task BeginTransactionAsync(IsolationLevel? isolationLevel = null, CancellationToken token = default(CancellationToken))
        {
            TransactionDepth++;
            _transactionLogIndexes.Push(_changes.Count);
            if (TransactionDepth != 1) return;
            await OpenSharedConnectionAsync(token);
            Transaction = isolationLevel == null
                ? Connection.BeginTransaction()
                : Connection.BeginTransaction(isolationLevel.Value);
            _transactionCancelled = false;
            OnBeginTransaction();
        }

        /// <summary>
        /// Aborts the current transaction.
        /// </summary>
        public void AbortTransaction()
        {
            var task = AbortTransactionAsync();
            task.WaitAndUnwrapException();
        }

        /// <summary>
        /// Aborts the current transaction.
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        public async Task AbortTransactionAsync(CancellationToken token = default(CancellationToken))
        {
            if (TransactionDepth == 0)
            {
                throw new InvalidOperationException("There is no open transaction to abort.");
            }
            _transactionCancelled = true;

            // --- Cleanup the log for the aborted transaction
            var abortIndex = _transactionLogIndexes.Pop();
            _changes.RemoveRange(abortIndex, _changes.Count - abortIndex);

            // --- Clean up the transaction 
            if ((--TransactionDepth) == 0)
            {
                await CleanupTransactionAsync(token);
            }
        }

        /// <summary>
        /// Completes the current transaction.
        /// </summary>
        public void CompleteTransaction()
        {
            var task = CompleteTransactionAsync();
            task.WaitAndUnwrapException();
        }

        /// <summary>
        /// Completes the current transaction -- async
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        public async Task CompleteTransactionAsync(CancellationToken token = default(CancellationToken))
        {
            if (TransactionDepth == 0)
            {
                throw new InvalidOperationException("There is no open transaction to complete.");
            }

            // --- all changes should be kept...
            var abortIndex = _transactionLogIndexes.Pop();

            // --- ...unless the transaction has already been cancelled
            if (_transactionCancelled)
            {
                _changes.RemoveRange(abortIndex, _changes.Count - abortIndex);
            }

            // --- Clean up the transaction 
            if ((--TransactionDepth) == 0)
            {
                await CleanupTransactionAsync(token);
            }
        }

        #endregion

        #region SQL Command execution

        /// <summary>
        /// Sets the command timeout value to use when executing a command.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Sets a one-time command timeout value that is reset right after executing the next command.
        /// </summary>
        public int OneTimeCommandTimeout { get; set; }

        /// <summary>
        /// The last SQL string exceuted in a command
        /// </summary>
        public string LastSql { get; private set; }

        /// <summary>
        /// The last SQL arguments used in a command
        /// </summary>
        public object[] LastArgs { get; private set; }

        /// <summary>
        /// The last command executed
        /// </summary>
        public string LastCommand => FormatCommand(LastSql, LastArgs);

        /// <summary>
        /// Gets or sets the object used to map CLR instances to <see cref="FbParameter"/> instances.
        /// </summary>
        public ISqlParameterMapper ParameterMapper { get; set; }

        /// <summary>
        /// Executes the specified SQL batch with the provided parameters.
        /// </summary>
        /// <param name="sql">SQL string</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>Number of rows affected</returns>
        public int Execute(string sql, params object[] args)
        {
            return Execute(new SqlExpression(sql, args));
        }

        /// <summary>
        /// Executes the specified SQL batch with the provided parameters -- async
        /// </summary>
        /// <param name="sql">SQL string</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>Number of rows affected</returns>
        public Task<int> ExecuteAsync(string sql, params object[] args)
        {
            return ExecuteAsync(new SqlExpression(sql, args));
        }

        /// <summary>
        /// Executes the specified SQL batch with the provided parameters -- async
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL string</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>Number of rows affected</returns>
        public Task<int> ExecuteAsync(CancellationToken token, string sql, params object[] args)
        {
            return ExecuteAsync(new SqlExpression(sql, args), token);
        }

        /// <summary>
        /// Executes the specified SQL batch.
        /// </summary>
        /// <param name="sqlExpr">SQL statement</param>
        /// <returns>Number of rows affected</returns>
        public int Execute(SqlExpression sqlExpr)
        {
            var task = ExecuteAsync(sqlExpr);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Executes the specified SQL batch -- async
        /// </summary>
        /// <param name="sqlExpr">SQL statement</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int> ExecuteAsync(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            // --- Check if direct execution is enabled or not
            if (DirectExecuteMode != SqlDirectExecuteMode.Enable)
            {
                throw new InvalidOperationException(
                    "Using the Execute family of operation is not allowed.");
            }

            // --- Prepare and execute the command
            var sql = sqlExpr.SqlText;
            var args = sqlExpr.Arguments;
            try
            {
                await OpenSharedConnectionAsync(token);
                try
                {
                    using (var cmd = CreateCommand(Connection, sql, args))
                    {
                        var result = await cmd.ExecuteNonQueryAsync(token);
                        OnExecutedCommand(cmd);
                        return result;
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw;
            }
        }

        /// <summary>
        /// Executes an SQL batch that returns a scalar value.
        /// </summary>
        /// <typeparam name="T">Type of scalar value</typeparam>
        /// <param name="sqlString">SQL string</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The value resulted from executing this SQL batch</returns>
        public T ExecuteScalar<T>(string sqlString, params object[] args)
        {
            return ExecuteScalar<T>(new SqlExpression(sqlString, args));
        }

        /// <summary>
        /// Executes an SQL batch that returns a scalar value -- async
        /// </summary>
        /// <typeparam name="T">Type of scalar value</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sqlString">SQL string</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The value resulted from executing this SQL batch</returns>
        public Task<T> ExecuteScalarAsync<T>(CancellationToken token, string sqlString, params object[] args)
        {
            return ExecuteScalarAsync<T>(new SqlExpression(sqlString, args), token);
        }

        /// <summary>
        /// Executes an SQL batch that returns a scalar value -- async
        /// </summary>
        /// <typeparam name="T">Type of scalar value</typeparam>
        /// <param name="sqlString">SQL string</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The value resulted from executing this SQL batch</returns>
        public Task<T> ExecuteScalarAsync<T>(string sqlString, params object[] args)
        {
            return ExecuteScalarAsync<T>(new SqlExpression(sqlString, args));
        }

        /// <summary>
        /// Executes an SQL batch that returns a scalar value.
        /// </summary>
        /// <typeparam name="T">Type of scalar value</typeparam>
        /// <param name="sqlExpr">SQL string</param>
        /// <returns>The value resulted from executing this SQL batch</returns>
        public T ExecuteScalar<T>(SqlExpression sqlExpr)
        {
            var task = ExecuteScalarAsync<T>(sqlExpr);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Executes an SQL batch that returns a scalar value.
        /// </summary>
        /// <typeparam name="T">Type of scalar value</typeparam>
        /// <param name="sqlExpr">SQL string</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The value resulted from executing this SQL batch</returns>
        public async Task<T> ExecuteScalarAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            // --- Check if direct execution is enabled or not
            if (DirectExecuteMode != SqlDirectExecuteMode.Enable)
            {
                throw new InvalidOperationException(
                    "Using the Execute family of operation is not allowed.");
            }

            // --- Prepare and execute the command
            var sql = sqlExpr.SqlText;
            var args = sqlExpr.Arguments;
            try
            {
                await OpenSharedConnectionAsync(token);
                try
                {
                    using (var cmd = CreateCommand(Connection, sql, args))
                    {
                        var val = await cmd.ExecuteScalarAsync(token) ?? default(T);
                        OnExecutedCommand(cmd);

                        var t = typeof(T);
                        var u = Nullable.GetUnderlyingType(t);
                        if (u == null)
                        {
                            return (T)Convert.ChangeType(val, t);
                        }
                        else
                        {
                            return val == null || val == DBNull.Value
                                       ? default(T)
                                       : (T)Convert.ChangeType(val, u);
                        }
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception ex)
            {
                OnException(ex);
                throw;
            }
        }

        /// <summary>
        /// Formats the specified SQL command for diagnostics purposes
        /// </summary>
        /// <param name="cmd">Command to format</param>
        /// <returns>Formatted SQL command</returns>
        public string FormatCommand(FbCommand cmd)
        {
            return FormatCommand(cmd.CommandText,
                (from IDataParameter parameter in cmd.Parameters
                 select parameter.Value).ToArray());
        }

        /// <summary>
        /// Formats the specified SQL command for diagnostics purposes
        /// </summary>
        /// <param name="sql">SQL command string</param>
        /// <param name="args">Command arguments</param>
        /// <returns>Formatted SQL command</returns>
        public string FormatCommand(string sql, object[] args)
        {
            var sb = new StringBuilder();
            if (sql == null) return string.Empty;
            sb.Append(sql);
            if (args != null && args.Length > 0)
            {
                sb.Append("\n");
                for (var i = 0; i < args.Length; i++)
                {
                    sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", PARAM_PREFIX, i, args[i].GetType().Name, args[i]);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        #endregion

        #region Event methods

        /// <summary>
        /// This method is called when a new <see cref="FbConnection"/> instance is 
        /// about to be open.
        /// </summary>
        /// <param name="conn">Connection instance</param>
        /// <returns>Connection instance to open</returns>
        public virtual FbConnection OnConnectionOpened(FbConnection conn)
        {
            return conn;
        }

        /// <summary>
        /// This method is called when a <see cref="FbConnection"/> instance is about to
        /// be closed.
        /// </summary>
        /// <param name="conn">Connection instance</param>
        public virtual void OnConnectionClosing(FbConnection conn)
        {
        }

        /// <summary>
        /// Override this method to handle additional activites when a new transaction is started.
        /// </summary>
        public virtual void OnBeginTransaction()
        {
        }

        /// <summary>
        /// Override this method to handle additional activites when a new transaction is ended.
        /// </summary>
        public virtual void OnEndTransaction()
        {
        }

        /// <summary>
        /// This method is called when a <see cref="FbCommand"/> is about to be executed.
        /// </summary>
        /// <param name="command">Command to execute</param>
        public virtual void OnExecutingCommand(FbCommand command)
        {
        }

        /// <summary>
        /// This method is called when a <see cref="FbCommand"/> has been executed.
        /// </summary>
        /// <param name="command">Command that has been executed</param>
        public virtual void OnExecutedCommand(FbCommand command)
        {
        }

        /// <summary>
        /// Override this event to handle an exception caught during command execution.
        /// </summary>
        /// <param name="x"></param>
        public virtual void OnException(Exception x)
        {
            Debug.WriteLine(x.ToString());
            Debug.WriteLine(LastCommand);
        }

        #endregion

        #region Single record queries

        /// <summary>
        /// Retrieves the first object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public T First<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).First();
        }

        /// <summary>
        /// Retrieves the first object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public async Task<T> FirstAsync<T>(string sql, params object[] args)
        {
            return (await QueryAsync<T>(sql, args)).First();
        }

        /// <summary>
        /// Retrieves the first object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sqlExpr">SQL expression</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public T First<T>(SqlExpression sqlExpr)
        {
            return Query<T>(sqlExpr).First();
        }

        /// <summary>
        /// Retrieves the first object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sqlExpr">SQL expression</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public async Task<T> FirstAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            return (await QueryAsync<T>(sqlExpr, token)).First();
        }

        /// <summary>
        /// Retrieves the first object and maps it into another object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        public T FirstInto<T>(T instance, string sql, params object[] args)
        {
            var task = FirstIntoAsync(instance, sql, args);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves the first object and maps it into another object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        public async Task<T> FirstIntoAsync<T>(T instance, string sql, params object[] args)
        {
            var records = await DoQueryAsync(new SqlExpression(sql, args), instance, default(CancellationToken), true);
            return records.First();
        }

        /// <summary>
        /// Retrieves the first object and maps it into another object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        public async Task<T> FirstIntoAsync<T>(CancellationToken token, T instance, string sql, params object[] args)
        {
            var records = await DoQueryAsync(new SqlExpression(sql, args), instance, token, true);
            return records.First();
        }

        /// <summary>
        /// Retrieves the first object and maps it into another object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sqlExpr">SQL expression</param>
        /// <returns>The single object retrieved</returns>
        public T FirstInto<T>(T instance, SqlExpression sqlExpr)
        {
            var task = DoQueryAsync(sqlExpr, instance, default(CancellationToken), true);
            task.WaitAndUnwrapException();
            return task.Result.First();
        }

        /// <summary>
        /// Retrieves the first object and maps it into another object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sqlExpr">SQL expression</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single object retrieved</returns>
        public async Task<T> FirstIntoAsync<T>(T instance, SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            var records = await DoQueryAsync(sqlExpr, instance, token, true);
            return records.First();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public T FirstOrDefault<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public async Task<T> FirstOrDefaultAsync<T>(string sql, params object[] args)
        {
            return (await QueryAsync<T>(sql, args)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken token, string sql, params object[] args)
        {
            return (await QueryAsync<T>(token, sql, args)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sqlExpr">SQL expression</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public T FirstOrDefault<T>(SqlExpression sqlExpr)
        {
            return Query<T>(sqlExpr).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sqlExpr">SQL expression</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public async Task<T> FirstOrDefaultAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            return (await QueryAsync<T>(sqlExpr, token)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- and maps it into another object 
        /// using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public T FirstOrDefaultInto<T>(T instance, string sql, params object[] args)
        {
            var task = DoQueryAsync(new SqlExpression(sql, args), instance, default(CancellationToken), true);
            task.WaitAndUnwrapException();
            return task.Result.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- and maps it into another object -- async
        /// using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public async Task<T> FirstOrDefaultIntoAsync<T>(T instance, string sql, params object[] args)
        {
            var records = await DoQueryAsync(new SqlExpression(sql, args), instance, default(CancellationToken), true);
            return records.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- and maps it into another object -- async
        /// using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public async Task<T> FirstOrDefaultIntoAsync<T>(CancellationToken token, T instance, string sql, params object[] args)
        {
            var records = await DoQueryAsync(new SqlExpression(sql, args), instance, token, true);
            return records.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- and maps it into another 
        /// object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sqlExpr">SQL expression</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public T FirstOrDefaultInto<T>(T instance, SqlExpression sqlExpr)
        {
            var task = DoQueryAsync(sqlExpr, instance, default(CancellationToken), true);
            task.WaitAndUnwrapException();
            return task.Result.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first object -- or the default if no objects found -- and maps it into another -- async
        /// object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sqlExpr">SQL expression</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public async Task<T> FirstOrDefaultIntoAsync<T>(T instance, SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            var records = await DoQueryAsync(sqlExpr, instance, token, true);
            return records.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a single object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public T Single<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).Single();
        }

        /// <summary>
        /// Retrieves a single object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public async Task<T> SingleAsync<T>(string sql, params object[] args)
        {
            return (await QueryAsync<T>(sql, args)).Single();
        }

        /// <summary>
        /// Retrieves a single object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved, or the poco's default value, if not found in database.</returns>
        public async Task<T> SingleAsync<T>(CancellationToken token, string sql, params object[] args)
        {
            return (await QueryAsync<T>(token, sql, args)).Single();
        }

        /// <summary>
        /// Retrieves a single object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="sqlExpr">SQL expression</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public T Single<T>(SqlExpression sqlExpr)
        {
            return Query<T>(sqlExpr).Single();
        }

        /// <summary>
        /// Retrieves a single object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="sqlExpr">SQL expression</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public async Task<T> SingleAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            return (await QueryAsync<T>(sqlExpr, token)).Single();
        }

        /// <summary>
        /// Retrieves a single object and maps it into another object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public T SingleInto<T>(T instance, string sql, params object[] args)
        {
            var task = SingleIntoAsync(instance, new SqlExpression(sql, args));
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves a single object and maps it into another object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public async Task<T> SingleIntoAsync<T>(T instance, string sql, params object[] args)
        {
            return (await DoQueryAsync(new SqlExpression(sql, args), instance)).Single();
        }

        /// <summary>
        /// Retrieves a single object and maps it into another object using the specified SQL batch -- async
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public async Task<T> SingleIntoAsync<T>(CancellationToken token, T instance, string sql, params object[] args)
        {
            return (await DoQueryAsync(new SqlExpression(sql, args), instance, token)).Single();
        }

        /// <summary>
        /// Retrieves a single object and maps it into another object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sqlExpr">SQL expression</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public T SingleInto<T>(T instance, SqlExpression sqlExpr)
        {
            var task = SingleIntoAsync(instance, sqlExpr);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves a single object and maps it into another object using the specified SQL batch
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="instance">Poco instance to map the column values to</param>
        /// <param name="sqlExpr">SQL expression</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single object retrieved</returns>
        /// <remarks>Raises an exception, if not a single object found</remarks>
        public async Task<T> SingleIntoAsync<T>(T instance, SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            return (await DoQueryAsync(sqlExpr, instance, token)).Single();
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements.
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public T SingleById<T>(object pkFirst, params object[] pkOthers)
        {
            var task = SingleByIdAsync<T>(pkFirst, pkOthers);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public Task<T> SingleByIdAsync<T>(object pkFirst, params object[] pkOthers)
        {
            return SingleByIdAsync<T>(default(CancellationToken), pkFirst, pkOthers);
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public Task<T> SingleByIdAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        {
            var pkValues = new object[pkOthers.Length + 1];
            pkValues[0] = pkFirst;
            for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
            return SingleByIdAsync<T>(pkValues, token);
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements.
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkValues">Primary key elements</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public T SingleById<T>(IEnumerable<object> pkValues)
        {
            var task = SingleByIdAsync<T>(pkValues);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkValues">Primary key elements</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public async Task<T> SingleByIdAsync<T>(IEnumerable<object> pkValues,
            CancellationToken token = default(CancellationToken))
        {
            var values = pkValues.ToList();
            var records = await FetchByPrimaryKeyAsync<T>(values, token);
            if (records.Count == 1) return records[0];
            if (records.Count == 0) throw new RecordNotFoundException(typeof(T), values.ToArray());
            throw new MultipleRecordFoundException(typeof(T), values.ToArray());
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements.
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public T SingleOrDefaultById<T>(object pkFirst, params object[] pkOthers)
        {
            var task = SingleOrDefaultByIdAsync<T>(pkFirst, pkOthers);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public Task<T> SingleOrDefaultByIdAsync<T>(object pkFirst, params object[] pkOthers)
        {
            return SingleOrDefaultByIdAsync<T>(default(CancellationToken), pkFirst, pkOthers);
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public Task<T> SingleOrDefaultByIdAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        {
            var pkValues = new object[pkOthers.Length + 1];
            pkValues[0] = pkFirst;
            for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
            return SingleOrDefaultByIdAsync<T>(pkValues, token);
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements.
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkValues">Primary key elements</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public T SingleOrDefaultById<T>(IEnumerable<object> pkValues)
        {
            var task = SingleOrDefaultByIdAsync<T>(pkValues);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves a single record by the specified primary key elements -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkValues">Primary key elements</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The single record if that can be found.</returns>
        /// <exception cref="RecordNotFoundException">No record can be found</exception>
        /// <exception cref="MultipleRecordFoundException">Multiple record can be found</exception>
        public async Task<T> SingleOrDefaultByIdAsync<T>(IEnumerable<object> pkValues,
            CancellationToken token = default(CancellationToken))
        {
            var values = pkValues.ToList();
            var records = await FetchByPrimaryKeyAsync<T>(values, token);
            if (records.Count > 1) throw new MultipleRecordFoundException(typeof(T), values.ToArray());
            return (records.Count == 0) ? default(T) : records[0];
        }

        /// <summary>
        /// Checks if a record with the specified primary key values exists in the database
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>True, if the record exists; otherwise, false</returns>
        public bool Exists<T>(object pkFirst, params object[] pkOthers)
        {
            var task = ExistsAsync<T>(pkFirst, pkOthers);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Checks if a record with the specified primary key values exists in the database
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>True, if the record exists; otherwise, false</returns>
        public Task<bool> ExistsAsync<T>(object pkFirst, params object[] pkOthers)
        {
            return ExistsAsync<T>(default(CancellationToken), pkFirst, pkOthers);
        }

        /// <summary>
        /// Checks if a record with the specified primary key values exists in the database
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        /// <returns>True, if the record exists; otherwise, false</returns>
        public Task<bool> ExistsAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        {
            var pkValues = new object[pkOthers.Length + 1];
            pkValues[0] = pkFirst;
            for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
            return ExistsAsync<T>(pkValues, token);
        }

        /// <summary>
        /// Checks if a record with the specified primary key values exists in the database
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkValues">Primary key elements</param>
        /// <returns>True, if the record exists; otherwise, false</returns>
        public bool Exists<T>(IEnumerable<object> pkValues)
        {
            var task = ExistsAsync<T>(pkValues);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Checks if a record with the specified primary key values exists in the database
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkValues">Primary key elements</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>True, if the record exists; otherwise, false</returns>
        public async Task<bool> ExistsAsync<T>(IEnumerable<object> pkValues,
            CancellationToken token = default(CancellationToken))
        {
            return (await FetchByPrimaryKeyAsync<T>(pkValues.ToList(), token)).Count > 0;
        }

        /// <summary>
        /// Fetches all records filtered by the specified primary key values -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="values">Primary key values</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of record filtered by the primary key</returns>
        private Task<List<T>> FetchByPrimaryKeyAsync<T>(IReadOnlyList<object> values,
            CancellationToken token = default(CancellationToken))
        {
            // --- Check if the correct number of key element has been provided
            var metadata = RecordMetadataManager.GetMetadata<T>();
            if (metadata.PrimaryKeyColumns.Count != values.Count)
            {
                throw new InvalidOperationException(
                    $"Type {typeof (T).FullName} has primary key item count {metadata.PrimaryKeyColumns.Count}, but the number of provided item is {values.Count}.");
            }

            // --- Prepare the expression to execute
            var sqlExpr = SqlExpression.New.Select<T>().From<T>();
            for (var i = 0; i < values.Count; i++)
            {
                if (values[i] == null)
                {
                    throw new InvalidOperationException(
                        $"Value of primary key element {metadata.PrimaryKeyColumns[i].ColumnName} cannot be null");
                }
                sqlExpr.Where(
                    $"{EscapeSqlIdentifier(metadata.PrimaryKeyColumns[i].ColumnName)}=@0",
                    values[i]);
            }
            return FetchAsync<T>(sqlExpr, token);
        }

        #endregion

        #region Fetch operations

        /// <summary>
        /// Fetches records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public List<T> Fetch<T>(string sql, params object[] args)
        {
            return Fetch<T>(new SqlExpression(sql, args));
        }

        /// <summary>
        /// Fetches records from the database with the specified SQL fragment -- async
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public async Task<List<T>> FetchAsync<T>(string sql, params object[] args)
        {
            return await FetchAsync<T>(new SqlExpression(sql, args));
        }

        /// <summary>
        /// Fetches records from the database with the specified SQL fragment -- async
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public async Task<List<T>> FetchAsync<T>(CancellationToken token, string sql, params object[] args)
        {
            return await FetchAsync<T>(new SqlExpression(sql, args), token);
        }

        /// <summary>
        /// Fetches records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="sql">SQL fragment</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public List<T> Fetch<T>(SqlExpression sql)
        {
            return Query<T>(sql).ToList();
        }

        /// <summary>
        /// Fetches records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="sql">SQL fragment</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public async Task<List<T>> FetchAsync<T>(SqlExpression sql, CancellationToken token = default(CancellationToken))
        {
            return (await QueryAsync<T>(sql, token)).ToList();
        }

        /// <summary>
        /// Fetches records from the database.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <returns>The list of pocos fetched from the database.</returns>
        public List<T> Fetch<T>()
        {
            return Fetch<T>("");
        }

        /// <summary>
        /// Fetches records from the database -- async
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public async Task<List<T>> FetchAsync<T>(CancellationToken token = default(CancellationToken))
        {
            return (await FetchAsync<T>(token, ""));
        }

        /// <summary>
        /// Retrieves an enumeration mapped to the specified poco type.
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            return Query<T>(new SqlExpression(sql, args));
        }

        /// <summary>
        /// Retrieves an enumeration mapped to the specified poco type - async
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] args)
        {
            return QueryAsync<T>(new SqlExpression(sql, args));
        }

        /// <summary>
        /// Retrieves an enumeration mapped to the specified poco type - async
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public Task<IEnumerable<T>> QueryAsync<T>(CancellationToken token, string sql, params object[] args)
        {
            return QueryAsync<T>(new SqlExpression(sql, args), token);
        }

        /// <summary>
        /// Retrieves an enumeration mapped to the specified poco type, using the specified SQL object.
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="sqlExpr">SQL object</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public IEnumerable<T> Query<T>(SqlExpression sqlExpr)
        {
            var task = QueryAsync<T>(sqlExpr);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Retrieves an enumeration mapped to the specified poco type, using the specified SQL object.
        /// </summary>
        /// <typeparam name="T">Poco type</typeparam>
        /// <param name="sqlExpr">SQL object</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The list of pocos fetched from the database.</returns>
        public Task<IEnumerable<T>> QueryAsync<T>(SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            return DoQueryAsync(sqlExpr, default(T), token);
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Tuple<List<T1>, List<T2>> FetchMultiple<T1, T2>(string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>>> FetchMultipleAsync<T1, T2>(string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>>> FetchMultipleAsync<T1, T2>(CancellationToken token, string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Tuple<List<T1>, List<T2>, List<T3>> FetchMultiple<T1, T2, T3>(string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>, List<T3>>> FetchMultipleAsync<T1, T2, T3>(string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>, List<T3>>> FetchMultipleAsync<T1, T2, T3>(
            CancellationToken token, string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="T4">Data record type #4</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Tuple<List<T1>, List<T2>, List<T3>, List<T4>> FetchMultiple<T1, T2, T3, T4>(string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="T4">Data record type #4</typeparam>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="T4">Data record type #4</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="sql">SQL batch</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(
            CancellationToken token, string sql, params object[] args)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Tuple<List<T1>, List<T2>> FetchMultiple<T1, T2>(SqlExpression sqlExpr)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>>> FetchMultipleAsync<T1, T2>(SqlExpression sqlExpr,
            CancellationToken token = default(CancellationToken))
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Tuple<List<T1>, List<T2>, List<T3>> FetchMultiple<T1, T2, T3>(SqlExpression sqlExpr)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>, List<T3>>> FetchMultipleAsync<T1, T2, T3>(
            SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }


        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="T4">Data record type #4</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Tuple<List<T1>, List<T2>, List<T3>, List<T4>> FetchMultiple<T1, T2, T3, T4>(SqlExpression sqlExpr)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="T4">Data record type #4</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The tuple that holds lists of dat records fetched from the database.</returns>
        public Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(
            SqlExpression sqlExpr, CancellationToken token = default(CancellationToken))
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="TRet">Correlated data record type</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="correlator">Function that correlates the result sets</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public TRet FetchMultiple<T1, T2, TRet>(SqlExpression sqlExpr,
            Func<List<T1>, List<T2>, TRet> correlator)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="TRet">Correlated data record type</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="correlator">Function that correlates the result sets</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, TRet>(SqlExpression sqlExpr,
            Func<List<T1>, List<T2>, TRet> correlator,
            CancellationToken token = default(CancellationToken))
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="TRet">Correlated data record type</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="correlator">Function that correlates the result sets</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public TRet FetchMultiple<T1, T2, T3, TRet>(SqlExpression sqlExpr,
            Func<List<T1>, List<T2>, List<T3>, TRet> correlator)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query -- async
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="TRet">Correlated data record type</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="correlator">Function that correlates the result sets</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, TRet>(SqlExpression sqlExpr,
            Func<List<T1>, List<T2>, List<T3>, TRet> correlator,
            CancellationToken token = default(CancellationToken))
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="T4">Data record type #3</typeparam>
        /// <typeparam name="TRet">Correlated data record type</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="correlator">Function that correlates the result sets</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public TRet FetchMultiple<T1, T2, T3, T4, TRet>(SqlExpression sqlExpr,
            Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> correlator)
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Fetches multiple result sets in one query.
        /// </summary>
        /// <typeparam name="T1">Data record type #1</typeparam>
        /// <typeparam name="T2">Data record type #2</typeparam>
        /// <typeparam name="T3">Data record type #3</typeparam>
        /// <typeparam name="T4">Data record type #3</typeparam>
        /// <typeparam name="TRet">Correlated data record type</typeparam>
        /// <param name="sqlExpr">SQL Expression that defines the query</param>
        /// <param name="correlator">Function that correlates the result sets</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The tuple that holds lists of data records fetched from the database.</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, T4, TRet>(SqlExpression sqlExpr,
            Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> correlator,
            CancellationToken token = default(CancellationToken))
        {
            throw new NotSupportedException("Firebird does not support multipleresult sets.");
        }

        /// <summary>
        /// Executes the query 
        /// </summary>
        /// <typeparam name="T">Entity type of the query result</typeparam>
        /// <param name="sqlExpr">SQL statement</param>
        /// <param name="instance">Instance to fill with the data</param>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="firstOnly">Query only the first record, if found</param>
        /// <returns>Query resultset</returns>
        private async Task<IEnumerable<T>> DoQueryAsync<T>(SqlExpression sqlExpr, T instance = default(T),
            CancellationToken token = default(CancellationToken), bool firstOnly = false)
        {
            sqlExpr = sqlExpr.CompleteSelect<T>();
            var sql = sqlExpr.SqlText;
            var args = sqlExpr.Arguments;

            await OpenSharedConnectionAsync(token);
            try
            {
                using (var cmd = CreateCommand(Connection, sql, args))
                {
                    IDataReader r;
                    try
                    {
                        r = await cmd.ExecuteReaderAsync(token);
                        OnExecutedCommand(cmd);
                    }
                    catch (Exception x)
                    {
                        OnException(x);
                        throw;
                    }

                    var records = new List<T>();
                    using (r)
                    {
                        var recordFactory = DataReaderMappingManager.GetMapperFor(r, instance);
                        while (true)
                        {
                            T dataRecord;
                            try
                            {
                                if (!r.Read()) break;
                                dataRecord = recordFactory(r, instance);
                            }
                            catch (Exception x)
                            {
                                OnException(x);
                                throw;
                            }
                            records.Add(dataRecord);
                            if (firstOnly) break;
                        }
                        return records;
                    }
                }
            }
            finally
            {
                CloseSharedConnection();
            }
        }

        #endregion

        #region Page operations

        /// <summary>
        /// Fetches a page of records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageLength">Number of items per page</param>
        /// <param name="sql">SQL fragment</param>
        /// <returns>The paged result of the query.</returns>
        public Page<T> Page<T>(long pageIndex, long pageLength, SqlExpression sql)
        {
            return Page<T>(pageIndex, pageLength, sql.SqlText, sql.Arguments);
        }

        /// <summary>
        /// Fetches a page of records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageLength">Number of items per page</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The paged result of the query.</returns>
        public async Task<Page<T>> PageAsync<T>(long pageIndex, long pageLength, SqlExpression sql,
            CancellationToken token = default(CancellationToken))
        {
            return await PageAsync<T>(token, pageIndex, pageLength, sql.SqlText, sql.Arguments);
        }

        /// <summary>
        /// Fetches a page of records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageLength">Number of items per page</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The paged result of the query.</returns>
        public Page<T> Page<T>(long pageIndex, long pageLength, string sql, params object[] args)
        {
            var task = PageAsync<T>(default(CancellationToken), pageIndex, pageLength, sql, args);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Fetches a page of records from the database with the specified SQL fragment -- async
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageLength">Number of items per page</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The paged result of the query.</returns>
        public async Task<Page<T>> PageAsync<T>(long pageIndex, long pageLength, string sql, params object[] args)
        {
            return await PageAsync<T>(default(CancellationToken), pageIndex, pageLength, sql, args);
        }

        /// <summary>
        /// Fetches a page of records from the database with the specified SQL fragment -- async
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageLength">Number of items per page</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The paged result of the query.</returns>
        public async Task<Page<T>> PageAsync<T>(CancellationToken token, long pageIndex, long pageLength, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>(pageLength * pageIndex, pageLength, sql, ref args, out sqlCount, out sqlPage);

            // --- Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T>
            {
                CurrentPage = pageIndex,
                ItemsPerPage = pageLength,
                TotalItems = await ExecuteScalarAsync<long>(token, sqlCount, args)
            };
            result.TotalPages = result.TotalItems / pageLength;
            if ((result.TotalItems % pageLength) != 0) result.TotalPages++;
            OneTimeCommandTimeout = saveTimeout;

            // --- Get the records
            result.Items = await FetchAsync<T>(token, sqlPage, args);
            return result;
        }

        /// <summary>
        /// Fetches a chunk of records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="skip">Number of records to skip from the beginning of the record set</param>
        /// <param name="take">Number of records to take</param>
        /// <param name="sql">SQL fragment</param>
        /// <returns>The paged result of the query.</returns>
        public List<T> SkipTake<T>(long skip, long take, SqlExpression sql)
        {
            return SkipTake<T>(skip, take, sql.SqlText, sql.Arguments);
        }

        /// <summary>
        /// Fetches a chunk of records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="skip">Number of records to skip from the beginning of the record set</param>
        /// <param name="take">Number of records to take</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The paged result of the query.</returns>
        public async Task<List<T>> SkipTakeAsync<T>(long skip, long take, SqlExpression sql,
            CancellationToken token = default(CancellationToken))
        {
            return await SkipTakeAsync<T>(token, skip, take, sql.SqlText, sql.Arguments);
        }
        /// <summary>
        /// Fetches a chunk of records from the database with the specified SQL fragment.
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="skip">Number of records to skip from the beginning of the record set</param>
        /// <param name="take">Number of records to take</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The paged result of the query.</returns>
        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            var task = SkipTakeAsync<T>(skip, take, sql, args);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Fetches a chunk of records from the database with the specified SQL fragment -- async
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="skip">Number of records to skip from the beginning of the record set</param>
        /// <param name="take">Number of records to take</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The paged result of the query.</returns>
        public async Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return await FetchAsync<T>(sqlPage, args);
        }

        /// <summary>
        /// Fetches a chunk of records from the database with the specified SQL fragment -- async
        /// </summary>
        /// <typeparam name="T">Type of poco to fetch.</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="skip">Number of records to skip from the beginning of the record set</param>
        /// <param name="take">Number of records to take</param>
        /// <param name="sql">SQL fragment</param>
        /// <param name="args">Array of query parameters</param>
        /// <returns>The paged result of the query.</returns>
        public async Task<List<T>> SkipTakeAsync<T>(CancellationToken token, long skip, long take, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return await FetchAsync<T>(token, sqlPage, args);
        }

        #endregion

        #region Data modification operations

        /// <summary>
        /// Inserts a new record into the database and retrieves the newly inserted record.
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">record instance</param>
        /// <param name="insertIdentity">True shows that identity value is inserted explicitly</param>
        /// <param name="withGet">True indicates that the newly inserted record should be read back</param>
        /// <remarks>The insert and get operations are atomic.</remarks>
        public void Insert<T>(T record, bool insertIdentity = false, bool withGet = true)
        {
            var task = InsertAsync(record, insertIdentity, withGet);
            task.WaitAndUnwrapException();
        }

        /// <summary>
        /// Inserts a new record into the database and retrieves the newly inserted record -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">record instance</param>
        /// <param name="insertIdentity">True shows that identity value is inserted explicitly</param>
        /// <param name="withGet">True indicates that the newly inserted record should be read back</param>
        /// <param name="token">Optional cancellation token</param>
        /// <remarks>The insert and get operations are atomic.</remarks>
        public async Task InsertAsync<T>(T record, bool insertIdentity = false, bool withGet = true,
            CancellationToken token = default(CancellationToken))
        {
            // --- Read-only mode prevents this operation to run
            if (OperationMode == SqlOperationMode.ReadOnly)
            {
                throw new InvalidOperationException(
                    "The database instance is in read-only mode, so INSERT is not allowed.");
            }

            // --- Do the operation
            try
            {
                // ReSharper disable CompareNonConstrainedGenericWithNull
                if (record == null) throw new ArgumentNullException(nameof(record));
                // ReSharper restore CompareNonConstrainedGenericWithNull

                // --- Prepare the SQL expression to execute
                var metadata = RecordMetadataManager.GetMetadata<T>();
                List<string> columnNames;
                List<string> columnValues;
                List<object> rawValues;
                PrepareInsertData(record, out columnNames, out columnValues, out rawValues);

                // --- Start executing it
                await OpenSharedConnectionAsync(token);
                try
                {
                    // --- Create and excute the insert command
                    var shouldGetBack = withGet;
                    using (var cmd = CreateCommand(Connection, ""))
                    {
                        var tableName = EscapeSqlTableName(metadata.TableName);
                        PrepareInsertCommand(metadata, cmd, shouldGetBack, tableName,
                            columnNames, columnValues, rawValues);

                        // --- Read back the inserted record with the RETURNING clause
                        if (shouldGetBack)
                        {
                            IDataReader r = await cmd.ExecuteReaderAsync(token);
                            OnExecutedCommand(cmd);
                            using (r)
                            {
                                var recordFactory = DataReaderMappingManager.GetMapperFor(r, record);
                                r.Read();
                                recordFactory(r, record);
                            }
                        }
                        else
                        {
                            await cmd.ExecuteNonQueryAsync(token);
                            OnExecutedCommand(cmd);
                        }
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// Updates the specified record in the database
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">Record instance</param>
        /// <remarks>The record must have a primary key</remarks>
        public void Update<T>(T record)
        {
            var task = UpdateInternalAsync(record);
            task.WaitAndUnwrapException();
        }

        /// <summary>
        /// Updates the specified record in the database -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">Record instance</param>
        /// <param name="token">Optional cancellation token</param>
        /// <remarks>The record must have a primary key</remarks>
        public async Task UpdateAsync<T>(T record, CancellationToken token = default(CancellationToken))
        {
            await UpdateInternalAsync(record, token);
        }

        /// <summary>
        /// Implements the update operation
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">Record instance</param>
        /// <param name="token">Optional cancellation token</param>
        /// <remarks>The record must have a primary key</remarks>
        private async Task<T> UpdateInternalAsync<T>(T record, CancellationToken token = default(CancellationToken))
        {
            // --- Read-only mode prevents this operation to run
            if (OperationMode == SqlOperationMode.ReadOnly)
            {
                throw new InvalidOperationException(
                    "The database instance is in read-only mode, so UPDATE is not allowed.");
            }

            // --- Do the operation
            var outRecord = default(T);
            try
            {
                // ReSharper disable CompareNonConstrainedGenericWithNull
                if (record == null) throw new ArgumentNullException(nameof(record));
                // ReSharper restore CompareNonConstrainedGenericWithNull

                // --- Prepare the SQL expression to execute
                var metadata = RecordMetadataManager.GetMetadata<T>();

                // --- Check if data record is used
                if (metadata.IsSimplePoco)
                {
                    throw new InvalidOperationException(
                        $"Type {typeof (T)} is not a data record type, it cannot be used in Update<> operation.");
                }

                // --- Check for primary key existence
                if (metadata.PrimaryKeyColumns.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"No primary key values found for {typeof (T)}, so it can be used in UPDATE.");
                }

                // --- Check for update column specification
                var recordAsDataRecord = record as IDataRecord;
                // ReSharper disable PossibleNullReferenceException
                var modifiedColumns = recordAsDataRecord.GetModifiedColumns();
                // ReSharper restore PossibleNullReferenceException

                // --- Prepare UPDATE
                var sql = new StringBuilder();
                var tableName = EscapeSqlTableName(metadata.TableName);
                sql.AppendFormat("update {0} set ", tableName);

                // --- Prepare SET columns
                var columnIndex = 0;
                var rawValues = new List<object>();
                foreach (var column in modifiedColumns)
                {
                    var dataColumn = metadata[column];
                    if (dataColumn.IsVersionColumn) continue;

                    // --- Obtain column value
                    var value = dataColumn.PropertyInfo.GetValue(record);

                    // --- Use a custom target converter if provided
                    if (dataColumn.TargetConverter != null)
                    {
                        value = dataColumn.TargetConverter.ConvertToDataType(value);
                    }
                    value = FixDateTimeValue(value);
                    if (columnIndex > 0) sql.Append(", ");
                    rawValues.Add(value);
                    sql.AppendFormat("{0} = @{1}", EscapeSqlIdentifier(dataColumn.ColumnName), columnIndex++);
                }

                // --- Prepare the WHERE clause
                sql.Append(" where ");
                var pkClauseAdded = false;
                foreach (var pkColumn in metadata.PrimaryKeyColumns)
                {
                    var pkValue = pkColumn.PropertyInfo.GetValue(record);
                    if (pkClauseAdded) sql.Append(" and ");
                    sql.AppendFormat("{0} = @{1}", EscapeSqlIdentifier(pkColumn.ColumnName), columnIndex++);
                    pkClauseAdded = true;
                    rawValues.Add(pkValue);
                }

                // --- Prepare version column where clause
                var versionColumn = metadata.DataColumns.FirstOrDefault(c => c.IsVersionColumn);
                if (RowVersionHandling != SqlRowVersionHandling.DoNotUseVersions && versionColumn != null)
                {
                    sql.AppendFormat(" and {0} = @{1}", EscapeSqlIdentifier(versionColumn.ColumnName), columnIndex);
                    rawValues.Add(versionColumn.PropertyInfo.GetValue(record));
                }

                await OpenSharedConnectionAsync(token);
                try
                {
                    int resultCount;

                    // --- Execute the command
                    using (var cmd = CreateCommand(Connection, sql.ToString(), rawValues.ToArray()))
                    {
                        resultCount = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                    }

                    // --- Check for concurrency issues
                    if (resultCount == 0 && versionColumn != null && RowVersionHandling == SqlRowVersionHandling.RaiseException)
                    {
                        throw new DBConcurrencyException(
                            $"A Concurrency update occurred in table '{tableName}' for primary key " +
                            $"value(s) = '{string.Join(", ", metadata.PrimaryKeyColumns.Select(col => col.PropertyInfo.GetValue(record)).ToArray())}' and version = '{TypeConversionHelper.ByteArrayToString((byte[]) versionColumn.PropertyInfo.GetValue(record))}'");
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
            return outRecord;
        }

        /// <summary>
        /// Deletes the specified record from the database
        /// </summary>
        /// <typeparam name="T">Reocord type</typeparam>
        /// <param name="record">Record to delete</param>
        public bool Delete<T>(T record)
        {
            var task = DeleteInternalAsync(record);
            task.WaitAndUnwrapException();
            return task.Result.Item1;
        }

        /// <summary>
        /// Deletes the specified record from the database -- async
        /// </summary>
        /// <typeparam name="T">Reocord type</typeparam>
        /// <param name="record">Record to delete</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<bool> DeleteAsync<T>(T record, CancellationToken token = default(CancellationToken))
        {
            return (await DeleteInternalAsync(record, token)).Item1;
        }

        /// <summary>
        /// Deletes the specified record from the database and retrieves
        /// the record before the deletion.
        /// </summary>
        /// <typeparam name="T">Reocrd type</typeparam>
        /// <param name="record">Record to delete</param>
        public T DeleteAndGetPrevious<T>(T record)
        {
            var task = DeleteInternalAsync(record);
            task.WaitAndUnwrapException();
            return task.Result.Item2;
        }

        /// <summary>
        /// Deletes the specified record from the database and retrieves -- async
        /// the record before the deletion.
        /// </summary>
        /// <typeparam name="T">Reocrd type</typeparam>
        /// <param name="record">Record to delete</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<T> DeleteAndGetPreviousAsync<T>(T record, CancellationToken token = default(CancellationToken))
        {
            var result = await DeleteInternalAsync(record, token);
            return result.Item2;
        }

        /// <summary>
        /// Deletes the specified record from the database
        /// </summary>
        /// <typeparam name="T">Reocrd type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        public bool DeleteById<T>(object pkFirst, params object[] pkOthers)
        {
            var task = DeleteByIdAsync<T>(pkFirst, pkOthers);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Deletes the specified record from the database -- async
        /// </summary>
        /// <typeparam name="T">Reocrd type</typeparam>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        public async Task<bool> DeleteByIdAsync<T>(object pkFirst, params object[] pkOthers)
        {
            var pkValues = new object[pkOthers.Length + 1];
            pkValues[0] = pkFirst;
            for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
            return await DeleteByIdAsync<T>(pkValues);
        }

        /// <summary>
        /// Deletes the specified record from the database -- async
        /// </summary>
        /// <typeparam name="T">Reocrd type</typeparam>
        /// <param name="token">Optional cancellation token</param>
        /// <param name="pkFirst">First element of the primary key</param>
        /// <param name="pkOthers">Other elements of the primary key</param>
        public async Task<bool> DeleteByIdAsync<T>(CancellationToken token, object pkFirst, params object[] pkOthers)
        {
            var pkValues = new object[pkOthers.Length + 1];
            pkValues[0] = pkFirst;
            for (var i = 0; i < pkOthers.Length; i++) pkValues[i + 1] = pkOthers[i];
            return await DeleteByIdAsync<T>(pkValues, token);
        }

        /// <summary>
        /// Deletes a record from the database using its primary key values
        /// </summary>
        /// <typeparam name="T">Reocrd type</typeparam>
        /// <param name="pkValues">Collection of primary key values</param>
        public bool DeleteById<T>(IEnumerable<object> pkValues)
        {
            var task = DeleteByIdAsync<T>(pkValues);
            task.WaitAndUnwrapException();
            return task.Result;
        }

        /// <summary>
        /// Deletes a record from the database using its primary key values
        /// </summary>
        /// <typeparam name="T">Reocrd type</typeparam>
        /// <param name="pkValues">Collection of primary key values</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<bool> DeleteByIdAsync<T>(IEnumerable<object> pkValues,
            CancellationToken token = default(CancellationToken))
        {
            return (await DeleteInternalAsync<T>(pkValues, token)).Item1;
        }

        /// <summary>
        /// Implements the delete operation -- async
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="pkValues">Collection of primary key values</param>
        /// <param name="token">Optional cancellation token</param>
        /// <remarks>The record must have a primary key</remarks>
        private async Task<Tuple<bool, T>> DeleteInternalAsync<T>(IEnumerable<object> pkValues,
            CancellationToken token = default(CancellationToken))
        {
            // --- Read-only mode prevents this operation to run
            if (OperationMode == SqlOperationMode.ReadOnly)
            {
                throw new InvalidOperationException(
                    "The database instance is in read-only mode, so DELETE is not allowed.");
            }

            // --- Do the operation
            if (pkValues == null) throw new ArgumentNullException(nameof(pkValues));
            var values = pkValues.ToList();
            try
            {
                // --- Prepare the SQL expression to execute
                var metadata = RecordMetadataManager.GetMetadata<T>();

                // --- Check if data record is used
                if (metadata.IsSimplePoco)
                {
                    throw new InvalidOperationException(
                        $"Type {typeof (T)} is not a data record type, it cannot be used in a Delete<> operation.");
                }

                // --- Check for primary key existence
                if (metadata.PrimaryKeyColumns.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"No primary key values found for {typeof (T)}, so it can be used in DELETE.");
                }

                // --- Prepare DELETE
                var sql = new StringBuilder();
                var tableName = EscapeSqlTableName(metadata.TableName);
                sql.AppendFormat("delete from {0}", tableName);

                // --- Prepare the WHERE clause
                var columnIndex = 0;
                var rawValues = new List<object>();
                sql.Append(" where ");
                var pkClauseAdded = false;
                foreach (var pkColumn in metadata.PrimaryKeyColumns)
                {
                    var pkValue = values[columnIndex];
                    if (pkClauseAdded) sql.Append(" and ");
                    sql.AppendFormat("{0} = @{1}", EscapeSqlIdentifier(pkColumn.ColumnName), columnIndex++);
                    pkClauseAdded = true;
                    rawValues.Add(pkValue);
                }

                await OpenSharedConnectionAsync(token);
                try
                {
                    // --- Execute the command
                    using (var cmd = CreateCommand(Connection, sql.ToString(), rawValues.ToArray()))
                    {
                        var rows = await cmd.ExecuteNonQueryAsync(token);
                        OnExecutedCommand(cmd);
                        return new Tuple<bool, T>(rows > 0, default(T));
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// Implements the delete operation
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">Record instance</param>
        /// <param name="token">Optional cancellation token</param>
        /// <remarks>The record must have a primary key</remarks>
        private async Task<Tuple<bool, T>> DeleteInternalAsync<T>(T record,
            CancellationToken token = default(CancellationToken))
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            if (record == null)
            // ReSharper restore CompareNonConstrainedGenericWithNull
            {
                throw new ArgumentNullException(nameof(record));
            }
            return await DeleteInternalAsync<T>(GetPrimaryKeyValue(record), token);
        }

        #endregion

        #region Miscellaneous methods

        /// <summary>
        /// Gets the escaped column name of the specified identifier
        /// </summary>
        /// <param name="identifier">SQL identifier</param>
        /// <returns>Escaped name</returns>
        public static string EscapeSqlIdentifier(string identifier)
        {
            return $"\"{identifier}\"";
        }

        /// <summary>
        /// Gets the escaped SQL name of the specified table
        /// </summary>
        /// <param name="tableName">SQL table name</param>
        /// <returns>Escaped table name</returns>
        public static string EscapeSqlTableName(string tableName)
        {
            return $"\"{tableName}\"";
        }

        /// <summary>
        /// Gets the escaped SQL name of the table specified by record type
        /// </summary>
        /// <returns>Escaped table name</returns>
        public static string EscapeSqlTableName<T>()
        {
            var metadata = RecordMetadataManager.GetMetadata<T>();
            return EscapeSqlTableName(metadata.TableName);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Prepares the values for the INSERT statement
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">Record instance</param>
        /// <param name="columnNames">Column names list</param>
        /// <param name="columnValues">Column parameters list</param>
        /// <param name="rawValues">Raw column values</param>
        private static void PrepareInsertData<T>(T record,
            out List<string> columnNames, out List<string> columnValues, out List<object> rawValues)
        {
            var metadata = RecordMetadataManager.GetMetadata<T>();
            columnNames = new List<string>();
            columnValues = new List<string>();
            rawValues = new List<object>();
            var columnIndex = 0;
            foreach (var column in metadata.DataColumns)
            {
                // --- Don't insert calculated columns and version columns
                if (column.IsCalculated || column.IsVersionColumn) continue;

                // --- Obtain column value
                var val = column.PropertyInfo.GetValue(record);

                // --- Use a custom target converter if provided
                if (column.TargetConverter != null)
                {
                    val = column.TargetConverter.ConvertToDataType(val);
                }
                val = FixDateTimeValue(val);

                // --- Add column info
                columnNames.Add(EscapeSqlIdentifier(column.ColumnName));
                columnValues.Add($"{PARAM_PREFIX}{columnIndex++}");
                rawValues.Add(val);
            }
        }

        /// <summary>
        /// Prepares the specified SQL command for insert operation
        /// </summary>
        /// <param name="metadata">Record metadata</param>
        /// <param name="cmd">SQL command instance</param>
        /// <param name="useOutput">Should the output of the INSERT statement used?</param>
        /// <param name="tableName">Name of the table to insert into</param>
        /// <param name="columnNames">Column names</param>
        /// <param name="columnValues">Column parameter names</param>
        /// <param name="rawValues">Column values</param>
        private void PrepareInsertCommand(DataRecordDescriptor metadata, FbCommand cmd,
            bool useOutput, string tableName, List<string> columnNames, List<string> columnValues,
            List<object> rawValues)
        {
            // --- Create the insert statement with paramters
            var sql = new StringBuilder();
            sql.Append(columnNames.Count > 0
                ? $"insert into {tableName} ({string.Join(", ", columnNames.ToArray())}) values ({string.Join(", ", columnValues.ToArray())})"
                : $"insert into {tableName} default values");
            if (!useOutput)
            {
                cmd.CommandText = sql.ToString();
                rawValues.ForEach(par => AddParam(cmd, par));
                return;
            }

            // --- Create a block with INSERT..RETURNING statement
            sql = new StringBuilder("execute block returns (");
            var needComma = false;
            foreach (var column in metadata.DataColumns.Where(c => !c.IsCalculated))
            {
                if (needComma)
                {
                    sql.Append(", ");
                }
                sql.Append($"{EscapeSqlIdentifier(column.Name)} {GetFbTypeNameForColumn(column)}");
                needComma = true;
            }

            // --- INSERT
            sql.Append($") as begin insert into {tableName} ");
            if (columnNames.Count > 0)
            {
                sql.Append($"({string.Join(", ", columnNames.ToArray())}) values(");
                needComma = false;
                foreach (var value in rawValues)
                {
                    if (needComma)
                    {
                        sql.Append(", ");
                    }
                    sql.Append(ToFbLiteral(value));
                    needComma = true;
                }
                sql.Append(")");
            }
            else
            {
                sql.Append("default values");
            }

            // --- RETURNING..INTO
            sql.Append(" returning ");
            needComma = false;
            foreach (var column in metadata.DataColumns.Where(c => !c.IsCalculated))
            {
                if (needComma)
                {
                    sql.Append(", ");
                }
                sql.Append(EscapeSqlIdentifier(column.Name));
                needComma = true;
            }
            sql.Append(" into ");
            needComma = false;
            foreach (var column in metadata.DataColumns.Where(c => !c.IsCalculated))
            {
                if (needComma)
                {
                    sql.Append(", ");
                }
                sql.AppendFormat(":{0}", EscapeSqlIdentifier(column.Name));
                needComma = true;
            }
            sql.Append("; suspend; end");
            cmd.CommandText = sql.ToString();
        }

        /// <summary>
        /// Converts CLR types to Firebird data type names
        /// </summary>
        /// <param name="column">Data column descriptor</param>
        /// <returns>Firebird data type name</returns>
        private static string GetFbTypeNameForColumn(DataColumnDescriptor column)
        {
            var clrType = column.ClrType;
            var underlying = Nullable.GetUnderlyingType(clrType);
            if (underlying != null)
            {
                clrType = underlying;
            }

            if (clrType == typeof (bool)
                || clrType == typeof (byte)
                || clrType == typeof (sbyte)
                || clrType == typeof (short)
                || clrType == typeof (ushort)
                || clrType == typeof (int)
                )
            {
                return "int";
            }
            if (clrType == typeof(uint)
                || clrType == typeof(long)
                || clrType == typeof(ulong)
                )
            {
                return "bigint";
            }

            if (clrType == typeof(decimal))
            {
                return "decimal(18, 18)";
            }

            if (clrType == typeof(float)
                || clrType == typeof(double)
                )
            {
                return "double precision";
            }

            if (clrType == typeof(char))
            {
                return "char(1)";
            }

            if (clrType == typeof(DateTime)
                || clrType == typeof(DateTimeOffset)
                )
            {
                return "timestamp";
            }

            if (column.MaxLength != null)
            {
                return $"varchar ({column.MaxLength})";
            }
            return column.IsBlob 
                ? $"varchar ({BLOB_LENGTH})" 
                : $"varchar({DEFAULT_VARCHAR_LENGTH})";
        }

        /// <summary>
        /// Converts an object to a Firebase value literal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ToFbLiteral(object value)
        {
            if (value == null)
            {
                return "null";
            }
            var clrType = value.GetType();
            if (clrType == typeof(bool)
                || clrType == typeof(byte)
                || clrType == typeof(sbyte)
                || clrType == typeof(short)
                || clrType == typeof(ushort)
                || clrType == typeof(int)
                || clrType == typeof(uint)
                || clrType == typeof(long)
                || clrType == typeof(ulong)
                || clrType == typeof(decimal)
                || clrType == typeof(float)
                || clrType == typeof(double)
                )
            {
                return value.ToString();
            }
            if (clrType == typeof(char)
                || clrType == typeof(string))
            {
                return $"'{value}'";
            }
            if (clrType == typeof(DateTime))
            {
                return $"'{((DateTime) value).ToString("MM/dd/yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture)}'";
            }
            if (clrType == typeof(DateTimeOffset))
            {
                return $"'{((DateTimeOffset)value).ToString("MM/dd/yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture)}'";
            }
            return ""; 
        }

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

        /// <summary>
        /// Creates a new <see cref="FbCommand"/> instance
        /// </summary>
        /// <param name="connection">Connection to use</param>
        /// <param name="sql">SQL command text</param>
        /// <param name="args">SQL command arguments</param>
        /// <returns></returns>
        private FbCommand CreateCommand(FbConnection connection, string sql, params object[] args)
        {
            // --- Perform parameter prefix replacements
            sql = sql.Replace("@@", "@");

            // --- Create the command and add parameters
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = sql;
            command.Transaction = Transaction;
            foreach (var item in args) AddParam(command, item);
            if (!string.IsNullOrEmpty(sql)) DoPreExecute(command);
            return command;
        }

        /// <summary>
        /// Adds a parameter to a database command
        /// </summary>
        /// <param name="cmd">Command to add the parameter for</param>
        /// <param name="item">Parameter object</param>
        private void AddParam(FbCommand cmd, object item)
        {
            var mapper = ParameterMapper ?? new DefaultSqlParameterMapper();
            var parameterName = $"{PARAM_PREFIX}{cmd.Parameters.Count}";
            var parameter = mapper.MapParameterValue(parameterName, item);
            if (parameter == null)
            {
                throw new InvalidOperationException(
                    $"The specified item with type '{item.GetType().FullName}' cannot be mapped to an SqlParameter");
            }
            cmd.Parameters.Add(parameter);
        }

        /// <summary>
        /// Prepares a command to execute
        /// </summary>
        /// <param name="command">Command to execute</param>
        private void DoPreExecute(FbCommand command)
        {
            // --- Setup command timeout
            if (OneTimeCommandTimeout != 0)
            {
                command.CommandTimeout = OneTimeCommandTimeout;
                OneTimeCommandTimeout = 0;
            }
            else if (CommandTimeout != 0)
            {
                command.CommandTimeout = CommandTimeout;
            }

            // --- Call hook
            OnExecutingCommand(command);

            // --- Save it
            LastSql = command.CommandText;
            LastArgs = (from FbParameter parameter in command.Parameters select parameter.Value).ToArray();
        }

        /// <summary>
        /// Fixes an SQL DateTime value to the acceptable minimum value
        /// </summary>
        /// <param name="value">value to fix</param>
        /// <returns>Fixed DateTime value</returns>
        public static object FixDateTimeValue(object value)
        {
            // --- Manage DateTime issues with SQL Server
            if (!(value is DateTime)) return value;
            var dateTimeValue = (DateTime)value;
            if (dateTimeValue.Year <= 1754)
            {
                value = new DateTime(1754, 1, 1);
            }
            return value;
        }

        /// <summary>
        /// Builds page queries with the specified parameters.
        /// </summary>
        /// <typeparam name="T">Entity type retrieved from the query</typeparam>
        /// <param name="skip">Number of items to skip</param>
        /// <param name="take">Number of items to take</param>
        /// <param name="sql">SQL string</param>
        /// <param name="args">SQL arguments</param>
        /// <param name="sqlCount">SQL query for obtaing the record count</param>
        /// <param name="sqlPage">SQL query obtaining the page.</param>
        private static void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args,
            out string sqlCount, out string sqlPage)
        {
            // --- Add auto select clause
            sql = AddSelectClause<T>(sql);

            // --- Split the SQL into the bits we need
            string sqlSelectRemoved;
            string sqlOrderBy;
            if (!SplitSqlForPaging(sql, out sqlCount, out sqlSelectRemoved, out sqlOrderBy))
            {
                throw new InvalidOperationException("Unable to parse SQL statement for paged query");
            }

            // --- Build the SQL for the actual final result
            sqlSelectRemoved = s_RxOrderBy.Replace(sqlSelectRemoved, "");
            sqlPage =
                $"select first @{args.Length} skip @{args.Length + 1} {sqlSelectRemoved}";
            args = args.Concat(new object[] { take, skip }).ToArray();
        }

        /// <summary>
        /// Splits an SQL string for paging.
        /// </summary>
        /// <param name="sql">Original SQL statement</param>
        /// <param name="sqlCount">SQL statement for obtaining the record count</param>
        /// <param name="sqlSelectRemoved">SQL statement withou the select clause</param>
        /// <param name="sqlOrderBy">SQL ORDER BY clause</param>
        /// <returns>True, if successfully cut; otherwise, false.</returns>
        private static bool SplitSqlForPaging(string sql, out string sqlCount,
            out string sqlSelectRemoved, out string sqlOrderBy)
        {
            sqlSelectRemoved = null;
            sqlCount = null;
            sqlOrderBy = null;

            // --- Extract the columns from "SELECT <whatever> FROM"
            var m = s_RxColumns.Match(sql);
            if (!m.Success) return false;

            // --- Save column list and replace with COUNT(*)
            var g = m.Groups[1];
            sqlSelectRemoved = sql.Substring(g.Index);

            if (s_RxDistinct.IsMatch(sqlSelectRemoved))
                sqlCount = sql.Substring(0, g.Index) + "count(" + m.Groups[1].ToString().Trim() + ") " + sql.Substring(g.Index + g.Length);
            else
                sqlCount = sql.Substring(0, g.Index) + "count(*) " + sql.Substring(g.Index + g.Length);

            // --- Look for an "ORDER BY <whatever>" clause
            m = s_RxOrderBy.Match(sqlCount);
            if (!m.Success) return true;
            g = m.Groups[0];
            sqlOrderBy = g.ToString();
            sqlCount = sqlCount.Substring(0, g.Index) + sqlCount.Substring(g.Index + g.Length);
            return true;
        }

        /// <summary>
        /// Adds a SELECT clause to the specified SQL string
        /// </summary>
        /// <typeparam name="T">Type representing the result of a query</typeparam>
        /// <param name="sql">SQL string</param>
        /// <returns></returns>
        static string AddSelectClause<T>(string sql)
        {
            if (s_RxSelect.IsMatch(sql)) return sql;
            var pd = RecordMetadataManager.GetMetadata<T>();
            var tableName = EscapeSqlTableName(pd.TableName);
            var cols = string.Join(", ", pd.DataColumns.Select(dc => EscapeSqlIdentifier(dc.ColumnName)).ToArray());
            sql = !s_RxFrom.IsMatch(sql)
                ? $"select {cols} from {tableName} {sql}"
                : $"select {cols} {sql}";
            return sql;
        }

        /// <summary>
        /// Gets the primary key values of the specified record.
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        /// <param name="record">record instance</param>
        /// <returns></returns>
        private static IEnumerable<object> GetPrimaryKeyValue<T>(T record)
        {
            return RecordMetadataManager
                .GetMetadata<T>()
                .PrimaryKeyColumns.Select(c => c.PropertyInfo.GetValue(record));
        }

        [ExcludeFromCodeCoverage]
        private static void CloseBrokenConnection(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Broken) connection.Close();
        }

        #endregion
    }
}