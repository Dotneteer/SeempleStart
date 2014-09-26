using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This class represents an exception occurring when the tracking of a data contex fails
    /// </summary>
    public class TrackingAbortedException: Exception 
    {
        /// <summary>
        /// Initializes a new instance of this class with the specified message and inner exception.
        /// </summary>
        /// <param name="innerException">Inner exception</param>
        public TrackingAbortedException(Exception innerException) : base("Tracking aborted", innerException)
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
        protected TrackingAbortedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}