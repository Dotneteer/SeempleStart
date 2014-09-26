namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class represents a method argument
    /// </summary>
    public class MethodArgument
    {
        /// <summary>
        /// Gets the name of the argument
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of the argument
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Creates a new method argument with the specified name and value.
        /// </summary>
        /// <param name="name">Argument name</param>
        /// <param name="value">Argument value</param>
        public MethodArgument(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}