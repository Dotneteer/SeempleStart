using Seemplest.Core.Tasks.Converters;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class implements the <see cref="ITask"/> interface and is intended to be
    /// the base class for all tasks.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        public ITaskExecutionContext Context { get; private set; }

        /// <summary>
        /// Sets up the task that will be run in the specified context.
        /// </summary>
        /// <param name="context">Task execution context</param>
        public virtual void Setup(ITaskExecutionContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Runs the specific task.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }
    }

    /// <summary>
    /// This class implements the <see cref="ITask{TArgument}"/> interface and is intended to be
    /// the base class for all tasks that have input arguments.
    /// </summary>
    public abstract class TaskBase<TArgument> :
        TaskBase,
        ITask<TArgument>
    {
        /// <summary>
        /// This property can be used to pass the argument to the task. 
        /// </summary>
        public TArgument Argument { get; set; }

        /// <summary>
        /// Gets the argument converter of this task
        /// </summary>
        public virtual ITaskArgumentConverter<TArgument> ArgumentConverter
        {
            get { return new DefaultArgumentConverter<TArgument>(); }
        }
    }

    /// <summary>
    /// This class implements the <see cref="ITask{TArgument, TResult}"/> interface and is intended to be
    /// the base class for all tasks that have input arguments and results.
    /// </summary>
    public abstract class TaskBase<TArgument, TResult> :
        TaskBase<TArgument>,
        ITask<TArgument, TResult>
    {
        /// <summary>
        /// This property can be used to query the result of the task. 
        /// </summary>
        public TResult Result { get; protected set; }

        /// <summary>
        /// Gets the result converter of this task
        /// </summary>
        public virtual ITaskResultConverter<TResult> ResultConverter
        {
            get { return new DefaultResultConverter<TResult>(); }
        }
    }
}