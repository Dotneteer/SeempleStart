using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.DataAccess.Exceptions
{
    /// <summary>
    /// This class represents the data base operation exception when a foreign key 
    /// constriant is violated.
    /// </summary>
    public class ForeignKeyViolationException : DataAccessExceptionBase
    {
        /// <summary>
        /// Gets the name of the foreign key.
        /// </summary>
        public string KeyName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error keyName and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="keyName">The error keyName that explains the reason for the exception. </param><param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public ForeignKeyViolationException(string keyName, Exception innerException) :
            base(String.Format("Foreign key violation on constraint {0}.", keyName), innerException)
        {
            KeyName = keyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds 
        /// the serialized object data about the exception being thrown. </param><param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or 
        /// <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        [ExcludeFromCodeCoverage]
        protected ForeignKeyViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}