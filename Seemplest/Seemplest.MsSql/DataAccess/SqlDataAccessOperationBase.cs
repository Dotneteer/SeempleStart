using System;
using System.Data;
using System.Data.SqlClient;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.DataServices;
using System.Threading.Tasks;

namespace Seemplest.MsSql.DataAccess
{
    /// <summary>
    /// This class is intended to be the base class of all SQL Server based
    /// data access operations
    /// </summary>
    public abstract class SqlDataAccessOperationBase : IDataAccessOperation
    {
        private EventedSqlDatabase _context;
        private bool _modeSet;
        private SqlOperationMode _mode;
        private readonly string _connectionParam;

                /// <summary>
        /// Initializes a new instance of the class with the specified connection
        /// and operation mode
        /// </summary>
        /// <param name="connectionOrName">Connection information</param>
        protected SqlDataAccessOperationBase(string connectionOrName)
        {
            _connectionParam = connectionOrName;
            _modeSet = false;
        }

        /// <summary>
        /// Gets the operation mode set
        /// </summary>
        protected SqlOperationMode OperationMode
        {
            get { return _mode; }
        }

        /// <summary>
        /// Gets the data access context
        /// </summary>
        protected SqlDatabase Context
        {
            get 
            {
                if (_context != null) return _context;
                _context = new EventedSqlDatabase(_connectionParam, _mode);
                _context.OnCommandExecuting += OnCommandExecuting;
                _context.OnCommandExecuted += OnCommandExecuted;
                return _context;
            }
        }

        /// <summary>
        /// Sets the operation mode of the service object
        /// </summary>
        /// <param name="mode"></param>
        public void SetOperationMode(SqlOperationMode mode)
        {
            if (_modeSet) return;
            _modeSet = true;
            _mode = mode;
        }

        /// <summary>
        /// Starts a new transaction
        /// </summary>
        /// <param name="level">Transaction isolation level</param>
        public void BeginTransaction(IsolationLevel? level = null)
        {
            Context.BeginTransaction(level);
        }

        /// <summary>
        /// Aborts the current transaction
        /// </summary>
        public void AbortTransaction()
        {
            Context.AbortTransaction();
        }

        /// <summary>
        /// Sign the operation as ready to commit
        /// </summary>
        public void Complete()
        {
            Context.CompleteTransaction();
        }

        /// <summary>
        /// Starts a new transaction -- async
        /// </summary>
        /// <param name="level">Transaction isolation level</param>
        public async Task BeginTransactionAsync(IsolationLevel? level = null)
        {
            await Context.BeginTransactionAsync(level);
        }

        /// <summary>
        /// Aborts the current transaction -- async
        /// </summary>
        public async Task AbortTransactionAsync()
        {
            await Context.AbortTransactionAsync();
        }

        /// <summary>
        /// Sign the operation as ready to commit
        /// </summary>
        public async Task CompleteAsync()
        {
            await Context.CompleteTransactionAsync();
        }

        /// <summary>
        /// Executes the operation on the private context
        /// </summary>
        /// <param name="action">Operation to execute on the context</param>
        protected void Operation(Action<SqlDatabase> action)
        {
            Operation(ctx =>
            {
                action(ctx);
                return 0;
            });
        }

        /// <summary>
        /// Executes the operation on the private context
        /// </summary>
        /// <typeparam name="TResult">Type of the operation's result</typeparam>
        /// <param name="function">Operation function</param>
        /// <returns></returns>
        protected TResult Operation<TResult>(Func<SqlDatabase, TResult> function)
        {
            try
            {
                return function(Context);
            }
            catch (SqlException ex)
            {
                throw DatabaseExceptionHelper.TransformSqlException(ex);
            }
        }

        /// <summary>
        /// Executes the operation on the private context
        /// </summary>
        /// <param name="action">Operation to execute on the context</param>
        protected async Task OperationAsync(Func<SqlDatabase, Task> action)
        {
            await OperationAsync(async ctx =>
            {
                await action(ctx);
                return 0;
            });
        }

        /// <summary>
        /// Executes the operation in async way on the private context
        /// </summary>
        /// <typeparam name="TResult">Type of the operation's result</typeparam>
        /// <param name="function">Operation function</param>
        /// <returns></returns>
        protected async Task<TResult> OperationAsync<TResult>(Func<SqlDatabase, Task<TResult>> function)
        {
            try
            {
                return await function(Context);
            }
            catch (SqlException ex)
            {
                throw DatabaseExceptionHelper.TransformSqlException(ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_context != null) _context.Dispose();
        }

        /// <summary>
        /// Handles the event when a command is being executed.
        /// </summary>
        private void OnCommandExecuting(object sender, SqlCommandEventArgs e)
        {
            OnCommandExecuting(e.SqlCommand);
        }

        /// <summary>
        /// Handles the event when a command is being executed.
        /// </summary>
        private void OnCommandExecuted(object sender, SqlCommandEventArgs e)
        {
            OnCommandExecuted(e.SqlCommand);
        }

        /// <summary>
        /// Override this method to catch the event when a command is about to be executed.
        /// </summary>
        /// <param name="command">Command being executed</param>
        protected virtual void OnCommandExecuting(SqlCommand command)
        {
        }

        /// <summary>
        /// Override this method to catch the event when a command has been executed.
        /// </summary>
        /// <param name="command">Command being executed</param>
        protected virtual void OnCommandExecuted(SqlCommand command)
        {
        }

        /// <summary>
        /// This class provides an SqlDatabase object that can be bound to events.
        /// </summary>
        private class EventedSqlDatabase : SqlDatabase
        {
            public EventedSqlDatabase(string connectionOrName, SqlOperationMode operationMode = SqlOperationMode.ReadWrite, 
                SqlDirectExecuteMode executeMode = SqlDirectExecuteMode.Enable, 
                SqlRowVersionHandling rowVersionHandling = SqlRowVersionHandling.RaiseException) 
                : base(connectionOrName, operationMode, executeMode, rowVersionHandling)
            {
            }

            public EventHandler<SqlCommandEventArgs> OnCommandExecuting;
            public EventHandler<SqlCommandEventArgs> OnCommandExecuted;

            /// <summary>
            /// This method is called when a <see cref="SqlCommand"/> is about to be executed.
            /// </summary>
            /// <param name="command">Command to execute</param>
            public override void OnExecutingCommand(SqlCommand command)
            {
                var handler = OnCommandExecuting;
                if (handler != null)
                {
                    handler(this, new SqlCommandEventArgs(command));
                }
            }

            /// <summary>
            /// This method is called when a <see cref="SqlCommand"/> is about to be executed.
            /// </summary>
            /// <param name="command">Command to execute</param>
            public override void OnExecutedCommand(SqlCommand command)
            {
                var handler = OnCommandExecuted;
                if (handler != null)
                {
                    handler(this, new SqlCommandEventArgs(command));
                }
            }
        }
    }
}