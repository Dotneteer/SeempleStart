using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when an already registered service is registered again.
    /// </summary>
    public class ServiceAlreadyRegisteredException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of service already registered.</param>
        public ServiceAlreadyRegisteredException(Type serviceType):
            base(string.Format("Service type '{0}' has already been registered", serviceType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The SerializationInfo that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The StreamingContext that contains contextual information about the source or destination.
        /// </param>
        [ExcludeFromCodeCoverage]
        protected ServiceAlreadyRegisteredException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}