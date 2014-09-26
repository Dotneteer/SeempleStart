using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when a circular service reference found.
    /// </summary>
    public class CircularServiceReferenceException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of service already registered.</param>
        public CircularServiceReferenceException(Type serviceType):
            base(string.Format("Circular service reference found while resolving type '{0}'", serviceType))
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
        protected CircularServiceReferenceException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}