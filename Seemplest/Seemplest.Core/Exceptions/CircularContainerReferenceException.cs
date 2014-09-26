using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when a circular container reference found.
    /// </summary>
    public class CircularContainerReferenceException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="containerName">Type of container already registered.</param>
        public CircularContainerReferenceException(string containerName):
            base(string.Format("Circular reference found while registering container '{0}'", containerName))
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
        protected CircularContainerReferenceException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}