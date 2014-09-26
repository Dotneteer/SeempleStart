using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when no matching constructor is found in the definition
    /// of the service object type.
    /// </summary>
    public class NoMatchingConstructorException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <param name="signature">Constructor signature</param>
        public NoMatchingConstructorException(Type type, string signature) :
            base(string.Format("No constructors of {0} found with signature {1}",
                                type, signature))
        {
        }

        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <param name="signature">Constructor signature</param>
        /// <param name="index">Parameter index</param>
        public NoMatchingConstructorException(Type type, string signature, int index) :
            base(string.Format("Injected constructor parameter cannot be resolved for type " +
                            "{0} in constructor with signature {1}. Parameter index is {2}",
                                type, signature, index))
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
        protected NoMatchingConstructorException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}