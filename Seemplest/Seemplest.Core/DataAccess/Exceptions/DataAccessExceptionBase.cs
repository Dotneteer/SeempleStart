using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.DataAccess.Exceptions
{
    /// <summary>
    /// This class is an class for all data access exceptions
    /// </summary>
    public abstract class DataAccessExceptionBase: ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected DataAccessExceptionBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with a specified error message.
        /// </summary>
        /// <param name="message">Error message</param>
        [ExcludeFromCodeCoverage]
        protected DataAccessExceptionBase(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified. 
        /// </param>
        protected DataAccessExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
        /// object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that 
        /// contains contextual information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null 
        /// or <see cref="P:System.Exception.HResult"/> is zero (0).</exception>
        [ExcludeFromCodeCoverage]
        protected DataAccessExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}