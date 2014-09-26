using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Seemplest.Core.DataAccess.Exceptions
{
    /// <summary>
    /// This class represents the data base operation exception when a check 
    /// constriant is violated.
    /// </summary>
    public class CheckConstraintViolationException : DataAccessExceptionBase
    {
        /// <summary>
        /// Gets the name of the table affected by the operation.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the name of the unique key.
        /// </summary>
        public string CheckName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error tableName 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="tableName">The tableName that describes the error. </param>
        /// <param name="checkName">The constraint that has been violated.</param>
        /// <param name="innerException">Original exception</param>
        public CheckConstraintViolationException(string tableName, string checkName, Exception innerException) :
            base(String.Format("Check constraint ({0}) violation on table {1}.", checkName, tableName),
            innerException)
        {
            TableName = tableName;
            CheckName = checkName;
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
        protected CheckConstraintViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}