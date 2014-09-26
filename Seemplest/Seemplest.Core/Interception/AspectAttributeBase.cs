using System;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class is intended to be the base of all attribute that define
    /// an aspect that can decorate an operation
    /// </summary>
    public abstract class AspectAttributeBase: Attribute, IMethodAspect
    {
        /// <summary>
        /// Gets the intended order of execution when an operation uses more than one aspects.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets the flag signing whether this aspect instance overrides the same type of 
        /// aspect instances.
        /// </summary>
        /// <remarks>
        /// Aspect attributes can be assigned to assemblies, classes and methods (operations).
        /// If this property is <code>true</code>, an aspect overrides the aspect defined in a higher level.
        /// Class attrbute overrides assembly level attribute, method attribute overrides class
        /// and assembly level attribute. Using <code>false</code>, the aspect does not override
        /// other aspects assigned to a higher level, instead, both aspects are executed.
        /// </remarks>
        public bool Override { get; set; }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        protected AspectAttributeBase()
        {
            Order = 0;
            Override = false;
        }

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
        public virtual IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        /// <summary>
        /// This method is called right after the body of the aspected method has been
        /// executed, independently whether it was successful or failed.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor representing the return values of the call</param>
        public virtual void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
        }

        /// <summary>
        /// This method is called right after <see cref="IMethodAspect.OnExit"/>, when the method body
        /// invocation was successful. Otherwise, the <see cref="IMethodAspect.OnException"/> method is
        /// called.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor representing the return values of the call</param>
        /// <returns>
        /// This method should return the value of <paramref name="result"/> by default, or it
        /// can modify the original result on return that value.
        /// </returns>
        public virtual IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        /// <summary>
        /// This method is called right after <see cref="IMethodAspect.OnExit"/>, when the method body
        /// invocation raised an exception. Otherwise, the <see cref="IMethodAspect.OnSuccess"/> method is
        /// called.
        /// </summary>
        /// <param name="argsMessage">Message representing the method call</param>
        /// <param name="exceptionRaised">Exception raised by the method body</param>
        /// <returns>Exception instance to be raised by the caller of the aspected method</returns>
        public virtual Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            return exceptionRaised;
        }
    }
}