namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the behavior of a task that receives an input parameter that
    /// the task should process.
    /// </summary>
    /// <typeparam name="TArgument">Task argument type</typeparam>
    public interface ITask<TArgument> : ITask
    {
        /// <summary>
        /// This property can be used to pass the argument to the task. 
        /// </summary>
        TArgument Argument { set; }

        /// <summary>
        /// Gets the argument converter of this task
        /// </summary>
        ITaskArgumentConverter<TArgument> ArgumentConverter { get; }
    }

    /// <summary>
    /// This interface defines the behavior of a task that receives an input parameter that
    /// the task should process, and provides a result.
    /// </summary>
    /// <typeparam name="TArgument">Task argument type</typeparam>
    /// <typeparam name="TResult">Task Result type</typeparam>
    public interface ITask<TArgument, TResult> : ITask<TArgument>
    {
        /// <summary>
        /// This property can be used to query the result of the task. 
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Gets the result converter of this task
        /// </summary>
        ITaskResultConverter<TResult> ResultConverter { get; }
    }
}