using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Seemplest.Core.DataAccess.Attributes;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when a record does not has a <see cref="TableNameAttribute"/>.
    /// </summary>
    public class MissingTableNameException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified service type.
        /// </summary>
        /// <param name="type">Record type</param>
        public MissingTableNameException(Type type) :
            base(string.Format("TableNameAttribute is required on type {0}", type))
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
        protected MissingTableNameException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}