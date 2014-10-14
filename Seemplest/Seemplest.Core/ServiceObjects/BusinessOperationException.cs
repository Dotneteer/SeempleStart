using System;
using System.Collections.Generic;
using System.Text;
using Seemplest.Core.ServiceObjects.Validation;

namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class represents an exception that says a business operation explicitly raised an
    /// exception because the operation failed by non-comforting business rules.
    /// </summary>
    /// <remarks>
    /// Do not use this kind of exception to describe any operation failures that are results of
    /// infrastructure or resource issues.
    /// </remarks>
    public abstract class BusinessOperationException : ApplicationException
    {
        /// <summary>
        /// The default reason code of all business exceptions
        /// </summary>
        public const string DEFAULT_REASON = "NoExplanation";

        /// <summary>
        /// Gets the notifications belonging to this exception.
        /// </summary>
        public NotificationList Notifications { get; private set; }

        public string ReasonCode { get; protected set; }

        /// <summary>
        /// The object that contains the error information to send to the client side
        /// </summary>
        public virtual object ErrorObject
        {
            get { return Notifications.Items; }
        }

        /// <summary>
        /// Initializes a new instance of the exception class.
        /// </summary>
        protected BusinessOperationException()
        {
            Notifications = new NotificationList();
            ReasonCode = DEFAULT_REASON;
        }

        /// <summary>
        /// Initializes a new instance of the exception class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        protected BusinessOperationException(string message)
            : base(message)
        {
            Notifications = new NotificationList();
            ReasonCode = DEFAULT_REASON;
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
        protected BusinessOperationException(
            string message, Exception innerException)
            : base(message, innerException)
        {
            Notifications = new NotificationList();
            ReasonCode = DEFAULT_REASON;
        }

        /// <summary>
        /// This method sets the collection of the business operation notifications according to the
        /// specified parameter, and retrieves this exception object.
        /// </summary>
        /// <param name="notifications">Business operation notifications</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement:
        /// throw MyBusinessOperationException().SetNotifications(myNotifications);
        /// </remarks>
        public BusinessOperationException SetNotifications(NotificationList notifications)
        {
            Notifications = notifications;
            return this;
        }

        /// <summary>
        /// This method adds a new notification to this exception instance according to the
        /// specified parameter, and retrieves this exception object.
        /// </summary>
        /// <param name="notification">Business operation notification</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement.
        /// </remarks>
        public BusinessOperationException AddNotification(NotificationItem notification)
        {
            EnsureNotifications();
            Notifications.Add(notification);
            return this;
        }

        /// <summary>
        /// Adds a new operation error notification.
        /// </summary>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement.
        /// </remarks>
        public BusinessOperationException AddOperationError(string code,
            IList<object> attributes = null, Exception exception = null)
        {
            EnsureNotifications();
            Notifications.AddOperationError(code, attributes, exception);
            return this;
        }

        /// <summary>
        /// Adds a new operation warning notification.
        /// </summary>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement.
        /// </remarks>
        public BusinessOperationException AddOperationWarning(string code,
            IList<object> attributes = null, Exception exception = null)
        {
            EnsureNotifications();
            Notifications.AddOperationWarning(code, attributes, exception);
            return this;
        }

        /// <summary>
        /// Adds a new entity error notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement.
        /// </remarks>
        public BusinessOperationException AddEntityError(string target, string code,
            IList<object> attributes = null, Exception exception = null)
        {
            EnsureNotifications();
            Notifications.AddEntityError(target, code, attributes, exception);
            return this;
        }

        /// <summary>
        /// Adds a new entity warning notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement.
        /// </remarks>
        public BusinessOperationException AddEntityWarning(string target, string code,
            IList<object> attributes = null, Exception exception = null)
        {
            EnsureNotifications();
            Notifications.AddEntityWarning(target, code, attributes, exception);
            return this;
        }

        /// <summary>
        /// Adds a new property error notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement.
        /// </remarks>
        public BusinessOperationException AddPropertyError(string target, string code,
            IList<object> attributes = null, Exception exception = null)
        {
            EnsureNotifications();
            Notifications.AddPropertyError(target, code, attributes, exception);
            return this;
        }

        /// <summary>
        /// Adds a new property warning notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <returns>This exception object</returns>
        /// <remarks>
        /// This method can be used directly in throw statement.
        /// </remarks>
        public BusinessOperationException AddPropertyWarning(string target, string code,
            IList<object> attributes = null, Exception exception = null)
        {
            EnsureNotifications();
            Notifications.AddPropertyWarning(target, code, attributes, exception);
            return this;
        }

        /// <summary>
        /// Ensures that the list of notifications is initialized
        /// </summary>
        private void EnsureNotifications()
        {
            if (Notifications == null)
            {
                Notifications = new NotificationList();
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>
        /// The error message that explains the reason for the exception, or an empty string("").
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override string Message
        {
            get
            {
                return new StringBuilder(base.Message)
                    .AppendLine()
                    .AppendLine(Notifications.ToString())
                    .ToString();
            }
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                new StringBuilder(Notifications.ToString())
                    .AppendLine()
                    .Append(base.ToString())
                    .ToString();
        }

        /// <summary>
        /// Appends the passed BusinessOperationException's notifications to this instance's 
        /// notifications and returns this instance.
        /// </summary>
        /// <param name="exception">BusinessOperationException whose notifications will be taken.</param>
        /// <returns>This instance with the parameter's notifications appended.</returns>
        public BusinessOperationException Append(BusinessOperationException exception)
        {
            foreach (var notification in exception.Notifications.Items)
            {
                Notifications.Add(notification);
            }
            return this;
        }
    }
}