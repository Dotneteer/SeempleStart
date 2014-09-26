namespace Seemplest.Core.Tasks.Converters
{
    /// <summary>
    /// This converter does not change the message string when converting it to an argument.
    /// </summary>
    public sealed class SelfArgumentConverter : ITaskArgumentConverter<string>
    {
        /// <summary>
        /// Converts the string message into an argument understandable by the task.
        /// </summary>
        /// <param name="message">Message representing the task argument.</param>
        /// <returns>Task argument</returns>
        public string ConvertToArgument(string message)
        {
            return message;
        }
    }
}