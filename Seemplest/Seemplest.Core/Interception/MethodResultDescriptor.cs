using System;
using System.Collections.Generic;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class represents the result returned from a method call
    /// </summary>
    public class MethodResultDescriptor : IMethodResultDescriptor
    {
        private readonly List<MethodArgument> _outputArguments = new List<MethodArgument>();

        /// <summary>
        /// Gets the number of output arguments
        /// </summary>
        public int OutputArgumentCount 
        {
            get { return _outputArguments.Count; }
        }

        /// <summary>
        /// Gets the output argument specified by the index.
        /// </summary>
        /// <param name="index">Output argument index</param>
        /// <returns>The specified output argument</returns>
        public MethodArgument GetOutputArgument(int index)
        {
            return _outputArguments[index];
        }

        /// <summary>
        /// Gets the flag indicating whether this method has a return value.
        /// </summary>
        public bool HasReturnValue { get; private set; }

        /// <summary>
        /// Gets the return value of the method call;
        /// </summary>
        public object ReturnValue { get; private set; }

        /// <summary>
        /// Gets the exception of the method call.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Creates a new instance with the specified return values and output arguments
        /// </summary>
        /// <param name="hasReturnValue">Indicates if the method has return value</param>
        /// <param name="returnValue">Method return value</param>
        /// <param name="outputArgs">Output arguments</param>
        public MethodResultDescriptor(bool hasReturnValue, object returnValue, 
            IEnumerable<MethodArgument> outputArgs)
        {
            HasReturnValue = hasReturnValue;
            _outputArguments = outputArgs == null 
                ? new List<MethodArgument>() 
                : new List<MethodArgument>(outputArgs);
            ReturnValue = returnValue;
        }

        /// <summary>
        /// Creates a new instance with the specified exception instance.
        /// </summary>
        /// <param name="exception">Exception instance</param>
        public MethodResultDescriptor(Exception exception)
        {
            Exception = exception;
        }
    }
}