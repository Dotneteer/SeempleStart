using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when multiple records can be found by a single record operation
    /// </summary>
    public class MultipleRecordFoundException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="recordType">Type of record</param>
        /// <param name="keys">Elements of the primary key</param>
        public MultipleRecordFoundException(Type recordType, object[] keys) : 
            base(string.Format("Multiple record of type {0} can be found by its primary key '{1}'",
                               recordType.FullName,
                               string.Join(", ", keys)))
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
        protected MultipleRecordFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}