using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This class represents an exception occurring within an aspect
    /// </summary>
    public class AspectInfrastructureException: Exception 
    {
        /// <summary>
        /// Initializes a new instance of this class with the specified message and inner exception.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public AspectInfrastructureException(string message, Exception innerException) : base(message, innerException)
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
        protected AspectInfrastructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}