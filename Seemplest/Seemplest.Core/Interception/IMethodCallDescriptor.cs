using System.Reflection;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This interface represents a method args
    /// </summary>
    public interface IMethodCallDescriptor
    {
        /// <summary>
        /// Reflection information on the target method
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// Gets the number of method arguments
        /// </summary>
        int ArgumentCount { get; }

        /// <summary>
        /// Gets the argument specified by the index
        /// </summary>
        /// <param name="index">Zero-based argument index</param>
        /// <returns>Method argument</returns>
        MethodArgument GetArgument(int index);
    }
}