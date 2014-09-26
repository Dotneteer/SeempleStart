using System;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This interface defines the responsibilities of an aspect assigned to an operation. 
    /// </summary>
    public interface IMethodAspect
    {
        /// <summary>
        /// This method is called before the body of the aspected method is about to be
        /// invoked.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor coming from the previous aspect.</param>
        /// <returns>
        /// This method should return null value indicating that the aspected method's body should
        /// be called. If the method body invocation should be omitted, this method returns the
        /// result descriptor substituting the result coming from the invocation of the method body.
        /// </returns>
        IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result);

        /// <summary>
        /// This method is called right after the body of the aspected method has been
        /// executed, independently whether it was successful or failed.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor representing the return values of the call</param>
        void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result);

        /// <summary>
        /// This method is called right after <see cref="OnExit"/>, when the method body
        /// invocation was successful. Otherwise, the <see cref="OnException"/> method is
        /// called.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor representing the return values of the call</param>
        /// <returns>
        /// This method should return the value of <paramref name="result"/> by default, or it
        /// can modify the original result on return that value.
        /// </returns>
        IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result);

        /// <summary>
        /// This method is called right after <see cref="OnExit"/>, when the method body
        /// invocation raised an exception. Otherwise, the <see cref="OnSuccess"/> method is
        /// called.
        /// </summary>
        /// <param name="argsMessage">Message representing the method call</param>
        /// <param name="exceptionRaised">Exception raised by the method body</param>
        /// <returns>Exception instance to be raised by the caller of the aspected method</returns>
        Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised);
    }
}