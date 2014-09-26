using System;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.Interception;
using Seemplest.Core.ServiceObjects.Validation;
using Seemplest.Core.Tracing;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class is intended to be the base class of all service objects
    /// </summary>
    [Intercepted]
    public abstract class ServiceObjectBase: 
        IServiceObject,
        IMethodAspect
    {
        /// <summary>
        /// Validator object
        /// </summary>
        public Validator Verify { get; private set; }

        /// <summary>
        /// Initializes an instance of this class
        /// </summary>
        protected ServiceObjectBase()
        {
            Verify = new Validator();
        }

        /// <summary>
        /// Gets the call context associated with the service object
        /// </summary>
        public IServiceCallContext CallContext { get; private set; }

        /// <summary>
        /// Gets the service object with the specified type
        /// </summary>
        /// <param name="serviceType">Service object type</param>
        /// <returns>Service object with the specifie type, if found; otherwise, null</returns>
        [NoArgumentTrace]
        public object GetService(Type serviceType)
        {
            return ServiceLocator.GetService(serviceType);
        }

        /// <summary>
        /// Gest the service object specified with the specified type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>The requested service object</returns>
        [NoArgumentTrace]
        public TService GetService<TService>()
        {
            return ServiceLocator.GetService<TService>();
        }

        /// <summary>
        /// Gets the registry that can be used to obtain and manage services
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Sets up the call context of a service.
        /// </summary>
        /// <param name="context">Context instance</param>
        [NoArgumentTrace]
        void IServiceObject.SetCallContext(IServiceCallContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            CallContext = context;
        }

        /// <summary>
        /// Sets up the locator used by the service object
        /// </summary>
        /// <param name="locator">Service locator object</param>
        [NoArgumentTrace]
        void IServiceObject.SetServiceLocator(IServiceLocator locator)
        {
            ServiceLocator = locator;
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
        [NoArgumentTrace]
        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return null;
        }

        /// <summary>
        /// This method is called right after the body of the aspected method has been
        /// executed, independently whether it was successful or failed.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor representing the return values of the call</param>
        [NoArgumentTrace]
        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
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
        [NoArgumentTrace]
        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
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
        [NoArgumentTrace]
        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            // --- Check for business exceptions
            var businessEx = exceptionRaised as BusinessOperationException;
            return businessEx != null 
                ? OnBusinessException(businessEx) 
                : OnInfrastructureException(exceptionRaised);
        }

        /// <summary>
        /// Override this method to manage a business exception raised by a service operation.
        /// </summary>
        /// <param name="businessEx">Business exception raised</param>
        /// <returns>Exception to be reported</returns>
        [NoArgumentTrace]
        protected virtual Exception OnBusinessException(BusinessOperationException businessEx)
        {
            return businessEx;
        }

        /// <summary>
        /// Override this method to manage an ifrastructure exception raised by a service operation.
        /// </summary>
        /// <param name="exceptionRaised">Infrastructure exception raised</param>
        /// <returns>Exception to be reported</returns>
        [NoArgumentTrace]
        protected virtual Exception OnInfrastructureException(Exception exceptionRaised)
        {
            return exceptionRaised;
        }
    }
}