using System;
using System.Globalization;

namespace Seemplest.Core.Tasks.Converters
{
    /// <summary>
    /// This argument converter uses the Convert class to convert the result to a string.
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    public class DefaultResultConverter<TResult> : ITaskResultConverter<TResult>
    {
        /// <summary>
        /// Converts the result of the task to a string message.
        /// </summary>
        /// <param name="result">Task result instance</param>
        /// <returns>String representation of the task result.</returns>
        public string ConvertToResult(TResult result)
        {
            return (string)Convert.ChangeType(result, typeof(string), CultureInfo.InvariantCulture);
        }
    }
}