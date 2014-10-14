using System;

namespace Seemplest.Core.ServiceObjects.Validation
{
    /// <summary>
    /// Base exception class for all specific argument validation exceptions.
    /// </summary>
    public class ArgumentValidationException : BusinessOperationException
    {
        public const string ARGUMENT_REASON = "ArgumentValidation";
        /// <summary>
        /// Initializes a new instance of the exception class.
        /// </summary>
        public ArgumentValidationException()
            : base("Argument validation failed.")
        {
            ReasonCode = ARGUMENT_REASON;
        }

        /// <summary>
        /// Initializes a new instance of the exception class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public ArgumentValidationException(string message)
            : base(message)
        {
            ReasonCode = ARGUMENT_REASON;
        }

        /// <summary>
        /// Initializes a new instance of the exception class with a specified error message and 
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified. 
        /// </param>
        public ArgumentValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ReasonCode = ARGUMENT_REASON;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var baseMessage = base.ToString();
            if (Notifications == null || Notifications.Items.Count == 0) return baseMessage;
            var notificationMessage = Notifications.ToString();
            return notificationMessage + Environment.NewLine + baseMessage;
        }
    }
}