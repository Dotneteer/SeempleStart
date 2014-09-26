using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when multiple constructors are found in the definition
    /// of the service object type.
    /// </summary>
    public class AmbigousConstructorException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <param name="signature">Constructor signature</param>
        public AmbigousConstructorException(Type type, string signature) :
            base(string.Format("Multiple constructors of {0} found with matching signature {1} prefix",
                type, signature))
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
        protected AmbigousConstructorException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}