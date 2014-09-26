namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This interface defines the behaviour of a task argument converter.
    /// </summary>
    /// <typeparam name="TArgument">Task argument type</typeparam>
    public interface ITaskArgumentConverter<out TArgument>
    {
        /// <summary>
        /// Converts the string message into an argument understandable by the task.
        /// </summary>
        /// <param name="message">Message representing the task argument.</param>
        /// <returns>Task argument</returns>
        TArgument ConvertToArgument(string message);
    }
}