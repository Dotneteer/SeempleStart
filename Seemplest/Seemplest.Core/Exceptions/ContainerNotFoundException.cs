using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when a container is not found in the registry
    /// </summary>
    public class ContainerNotFoundException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="name">Container name</param>
        public ContainerNotFoundException(string name) : 
            base(string.Format("Container '{0}' not found ion the registry", name))
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
        protected ContainerNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}