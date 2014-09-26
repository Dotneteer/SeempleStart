using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.Exceptions
{
    /// <summary>
    /// This exception is raised when an immutable record is about o be changed.
    /// </summary>
    public class ImmutableRecordChangedException: Exception
    {
        /// <summary>
        /// Creates a new instance with the specified column name
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        public ImmutableRecordChangedException(string columnName) :
            base(string.Format("You tried to modify the value of column '{0}', but it is immutable.", columnName))
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
        protected ImmutableRecordChangedException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}