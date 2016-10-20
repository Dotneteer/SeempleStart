using System;
using System.Data;
using System.Data.SqlClient;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.DataServices;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace Seemplest.FbSql.DataAccess
{
    /// <summary>
    /// This class is intended to be the base class of all SQL Server based
    /// data access operations
    /// </summary>
    public abstract class FbDataAccessOperationBase : IDataAccessOperation
    {
        private EventedFbDatabase _context;
        private bool _modeSet;
        private SqlOperationMode _mode;
        private readonly string _connectionParam;

        /// <summary>
        /// Initializes a new instance of the class with the specified connection
        /// and operation mode
        /// </summary>
        /// <param name="connectionOrName">Connection information</param>
        protected FbDataAccessOperationBase(string connectionOrName)
        {
            _connectionParam = connectionOrName;
            _modeSet = false;
        }

        /// <summary>
        /// Gets the operation mode set
        /// </summary>
        protected SqlOperationMode OperationMode => _mode;

        /// <summary>
        /// Gets the data access context
        /// </summary>
        protected FbDatabase Context
        {
            get
            {
                if (_context != null) return _context;
                _context = new EventedFbDatabase(_connectionParam, _mode);
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
        protected void Operation(Action<FbDatabase> action)
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
        protected TResult Operation<TResult>(Func<FbDatabase, TResult> function)
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
        protected async Task OperationAsync(Func<FbDatabase, Task> action)
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
        protected async Task<TResult> OperationAsync<TResult>(Func<FbDatabase, Task<TResult>> function)
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
            _context?.Dispose();
        }

        /// <summary>
        /// Handles the event when a command is being executed.
        /// </summary>
        private void OnCommandExecuting(object sender, FbCommandEventArgs e)
        {
            OnCommandExecuting(e.FbCommand);
        }

        /// <summary>
        /// Handles the event when a command is being executed.
        /// </summary>
        private void OnCommandExecuted(object sender, FbCommandEventArgs e)
        {
            OnCommandExecuted(e.FbCommand);
        }

        /// <summary>
        /// Override this method to catch the event when a command is about to be executed.
        /// </summary>
        /// <param name="command">Command being executed</param>
        protected virtual void OnCommandExecuting(FbCommand command)
        {
        }

        /// <summary>
        /// Override this method to catch the event when a command has been executed.
        /// </summary>
        /// <param name="command">Command being executed</param>
        protected virtual void OnCommandExecuted(FbCommand command)
        {
        }

        /// <summary>
        /// This class provides an SqlDatabase object that can be bound to events.
        /// </summary>
        private class EventedFbDatabase : FbDatabase
        {
            public EventedFbDatabase(string connectionOrName, SqlOperationMode operationMode = SqlOperationMode.ReadWrite,
                SqlDirectExecuteMode executeMode = SqlDirectExecuteMode.Enable,
                SqlRowVersionHandling rowVersionHandling = SqlRowVersionHandling.RaiseException)
                : base(connectionOrName, operationMode, executeMode, rowVersionHandling)
            {
            }

            public EventHandler<FbCommandEventArgs> OnCommandExecuting;
            public EventHandler<FbCommandEventArgs> OnCommandExecuted;

            /// <summary>
            /// This method is called when a <see cref="SqlCommand"/> is about to be executed.
            /// </summary>
            /// <param name="command">Command to execute</param>
            public override void OnExecutingCommand(FbCommand command)
            {
                var handler = OnCommandExecuting;
                handler?.Invoke(this, new FbCommandEventArgs(command));
            }

            /// <summary>
            /// This method is called when a <see cref="SqlCommand"/> is about to be executed.
            /// </summary>
            /// <param name="command">Command to execute</param>
            public override void OnExecutedCommand(FbCommand command)
            {
                var handler = OnCommandExecuted;
                handler?.Invoke(this, new FbCommandEventArgs(command));
            }
        }
    }
}