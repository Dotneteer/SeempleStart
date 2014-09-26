using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Seemplest.Core.ServiceObjects.Validation
{
    /// <summary>
    /// This class maintains a list of related notifications.
    /// </summary>
    /// <remarks>
    /// Notifications in this list belong to the same logical entity, e.g. operation, data entity, 
    /// property, etc.</remarks>
    [Serializable]
    public class NotificationList
    {
        private readonly List<NotificationItem> _items = new List<NotificationItem>();
        /// <summary>
        /// Gets the notification items of this list.
        /// </summary>
        public IReadOnlyList<NotificationItem> Items 
        {
            get { return new ReadOnlyCollection<NotificationItem>(_items); } 
        }

        /// <summary>
        /// Gets the count of items in this list;
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Adds the specified notification to the list.
        /// </summary>
        /// <param name="notificationItem">Notification to add</param>
        public void Add(NotificationItem notificationItem)
        {
            if (notificationItem == null) throw new ArgumentNullException("notificationItem");
            _items.Add(notificationItem);
        }

        /// <summary>
        /// Adds a new operation error notification.
        /// </summary>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <remarks>
        /// The notifiaction is added to the list of already collected notifications.
        /// </remarks>
        public void AddOperationError(string code, IList<object> attributes = null,
            Exception exception = null)
        {
            _items.Add(new NotificationItem(
                NotificationType.Error,
                code, String.Empty,
                NotificationTargetType.Operation,
                attributes, exception));
        }

        /// <summary>
        /// Adds a new operation warning notification.
        /// </summary>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <remarks>
        /// The notifiaction is added to the list of already collected notifications.
        /// </remarks>
        public void AddOperationWarning(string code, IList<object> attributes = null,
            Exception exception = null)
        {
            _items.Add(new NotificationItem(
                NotificationType.Warning,
                code, String.Empty,
                NotificationTargetType.Operation,
                attributes, exception));
        }

        /// <summary>
        /// Adds a new entity error notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <remarks>
        /// The notifiaction is added to the list of already collected notifications.
        /// </remarks>
        public void AddEntityError(string target, string code, IList<object> attributes = null,
            Exception exception = null)
        {
            _items.Add(new NotificationItem(
                NotificationType.Error,
                code, target,
                NotificationTargetType.Entity,
                attributes, exception));
        }

        /// <summary>
        /// Adds a new entity warning notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <remarks>
        /// The notifiaction is added to the list of already collected notifications.
        /// </remarks>
        public void AddEntityWarning(string target, string code, IList<object> attributes = null,
            Exception exception = null)
        {
            _items.Add(new NotificationItem(
                NotificationType.Warning,
                code, target,
                NotificationTargetType.Entity,
                attributes, exception));
        }

        /// <summary>
        /// Adds a new property error notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <remarks>
        /// The notifiaction is added to the list of already collected notifications.
        /// </remarks>
        public void AddPropertyError(string target, string code, IList<object> attributes = null,
            Exception exception = null)
        {
            _items.Add(new NotificationItem(
                NotificationType.Error,
                code, target,
                NotificationTargetType.Property,
                attributes, exception));
        }

        /// <summary>
        /// Adds a new property warning notification.
        /// </summary>
        /// <param name="target">Target name</param>
        /// <param name="code">Notification code</param>
        /// <param name="attributes">Optional notification attributes</param>
        /// <param name="exception">Optional notification exception</param>
        /// <remarks>
        /// The notifiaction is added to the list of already collected notifications.
        /// </remarks>
        public void AddPropertyWarning(string target, string code, IList<object> attributes = null,
            Exception exception = null)
        {
            _items.Add(new NotificationItem(
                NotificationType.Warning,
                code, target,
                NotificationTargetType.Property,
                attributes, exception));
        }

        /// <summary>
        /// Gets the flag indicating if the list contains an item with error
        /// </summary>
        public bool HasError
        {
            get { return _items.Any(i => i.Type == NotificationType.Error); }
        }

        /// <summary>
        /// Gets the flag indicating if the list contains an item with error or
        /// warning
        /// </summary>
        public bool HasErrorOrWarning
        {
            get { return _items.Any(i => i.Type == NotificationType.Error ||
                    i.Type == NotificationType.Warning); }
        }

        /// <summary>
        /// String representation. 
        /// </summary>        
        public override string ToString()
        {
            return _items == null 
                ? string.Empty 
                : string.Join(", ", Items.Select(i => i.ToString()));
        }
    }
}