using System.Collections.Generic;
using System.Reflection;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class represents a method args
    /// </summary>
    public class MethodCallDescriptor : IMethodCallDescriptor
    {
        private readonly List<MethodArgument> _arguments;

        /// <summary>
        /// The target object the method is called on
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// Reflection information on the target method
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Gets the number of method arguments
        /// </summary>
        public int ArgumentCount
        {
            get { return _arguments.Count; }
        }

        /// <summary>
        /// Gets the argument specified by the index
        /// </summary>
        /// <param name="index">Zero-based argument index</param>
        /// <returns>Method argument</returns>
        public MethodArgument GetArgument(int index)
        {
            return _arguments[index];
        }

        /// <summary>
        /// Creates a new descriptor using the specified targewt and method
        /// </summary>
        /// <param name="target">Target object of the method args</param>
        /// <param name="method">Method to args with the target object</param>
        /// <param name="args">Method arguments</param>
        public MethodCallDescriptor(object target, MethodInfo method, List<MethodArgument> args)
        {
            Target = target;
            Method = method;
            _arguments = args ?? new List<MethodArgument>();
        }
    }
}