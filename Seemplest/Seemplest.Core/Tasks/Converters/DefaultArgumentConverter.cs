using System;
using System.Globalization;

namespace Seemplest.Core.Tasks.Converters
{
    /// <summary>
    /// This argument converter uses the Convert class to convert the argument from a string.
    /// </summary>
    /// <typeparam name="TArgument">Type of argument</typeparam>
    public class DefaultArgumentConverter<TArgument> : ITaskArgumentConverter<TArgument>
    {
        /// <summary>
        /// Converts the string message into an argument understandable by the task.
        /// </summary>
        /// <param name="message">Message representing the task argument.</param>
        /// <returns>Task argument</returns>
        public TArgument ConvertToArgument(string message)
        {
            return (TArgument)Convert.ChangeType(message, typeof(TArgument), CultureInfo.InvariantCulture);
        }
    }
}