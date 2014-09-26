using System;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This interface represents the result returned from a method call
    /// </summary>
    public interface IMethodResultDescriptor
    {
        /// <summary>
        /// Gets the number of output arguments
        /// </summary>
        int OutputArgumentCount { get; }

        /// <summary>
        /// Gets the output argument specified by the index.
        /// </summary>
        /// <param name="index">Output argument index</param>
        /// <returns>The specified output argument</returns>
        MethodArgument GetOutputArgument(int index);

        /// <summary>
        /// Indicates if there is a return value
        /// </summary>
        bool HasReturnValue { get; }

        /// <summary>
        /// Gets the return value of the method call;
        /// </summary>
        object ReturnValue { get; }

        /// <summary>
        /// Gets the exception of the method call.
        /// </summary>
        Exception Exception { get; }
    }
}