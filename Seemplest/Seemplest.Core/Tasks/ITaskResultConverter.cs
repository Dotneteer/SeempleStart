namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the behaviour of a task result converter.
    /// </summary>
    /// <typeparam name="TResult">Task result type</typeparam>
    public interface ITaskResultConverter<in TResult>
    {
        /// <summary>
        /// Converts the result of the task to a string message.
        /// </summary>
        /// <param name="result">Task result instance</param>
        /// <returns>String representation of the task result.</returns>
        string ConvertToResult(TResult result);
    }
}