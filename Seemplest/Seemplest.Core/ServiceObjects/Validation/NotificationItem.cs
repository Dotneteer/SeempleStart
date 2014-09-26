using System;
using System.Collections.Generic;

namespace Seemplest.Core.ServiceObjects.Validation
{
    /// <summary>
    /// This class represents a notification retrieved from a business operation.
    /// </summary>
    public class NotificationItem
    {
        /// <summary>
        /// Gets the type of the notification.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Gets the target of the notification.
        /// </summary>
        public string Target { get; private set; }

        /// <summary>
        /// Gets the type of the target the notification belongs to.
        /// </summary>
        public NotificationTargetType TargetType { get; set; }

        /// <summary>
        /// Code of the notification
        /// </summary>
        /// <remarks>
        /// This property should contain predefined codes so that the consumer of
        /// the business operation would understand the same issue that the operation
        /// that raised this notification.
        /// </remarks>
        public string Code { get; set; }

        /// <summary>
        /// Optional attributes of the notification
        /// </summary>
        public IList<object> Attributes { get; set; }

        /// <summary>
        /// Optional exception of the notification
        /// </summary>
        public string ExceptionText { get; set; }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="type">Type fo the notification</param>
        /// <param name="code">The predefined code of the notification</param>
        /// <param name="target">The target of the notification</param>
        /// <param name="targetType">The type of the target the notification belongs to</param>
        /// <param name="attributes">A collection describing the attributes of the notification</param>
        /// <param name="exception">Optional exception instance</param>
        public NotificationItem(NotificationType type, string code, string target,
            NotificationTargetType targetType = NotificationTargetType.Operation,
            IList<object> attributes = null,
            Exception exception = null)
        {
            Type = type;
            Target = target;
            TargetType = targetType;
            Code = code;
            Attributes = attributes ?? new List<object>();
            ExceptionText = exception == null ? null : exception.Message;
        }
        /// <summary>
        /// String representation of this notification item.
        /// </summary>        
        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                Code,
                ExceptionText == null ? "" : ": ",
                ExceptionText ?? "");
        }
    }
}