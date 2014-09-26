using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when an invalid container name is used.
    /// </summary>
    public class InvalidContainerNameException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        public InvalidContainerNameException() : base("Container name must not be null or empty")
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
        protected InvalidContainerNameException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}