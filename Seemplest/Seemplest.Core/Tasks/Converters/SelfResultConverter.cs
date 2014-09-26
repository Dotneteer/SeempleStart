namespace Seemplest.Core.Tasks.Converters
{
    /// <summary>
    /// This converter does not change a string when converting it to a result message.
    /// </summary>
    public sealed class SelfResultConverter : ITaskResultConverter<string>
    {
        /// <summary>
        /// Converts the result of the task to a string message.
        /// </summary>
        /// <param name="result">Task result instance</param>
        /// <returns>String representation of the task result.</returns>
        public string ConvertToResult(string result)
        {
            return result;
        }
    }
}