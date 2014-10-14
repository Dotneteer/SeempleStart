using System;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This abstract class is intended to be the base class for all business rule exceptions
    /// </summary>
    public abstract class InvalidBusinessRuleException : BusinessOperationException
    {
        protected readonly object ErrorInfo;

        /// <summary>
        /// The object that contains the error information to send to the client side
        /// </summary>
        public override object ErrorObject
        {
            get { return ErrorInfo; }
        }

        protected InvalidBusinessRuleException(string code, object errorInfo = null)
        {
            ReasonCode = code;
            ErrorInfo = errorInfo;
        }

        protected InvalidBusinessRuleException(string code, Exception innerException, object errorInfo = null)
            : base(code, innerException)
        {
            ReasonCode = code;
            ErrorInfo = errorInfo;
        }
    }
}