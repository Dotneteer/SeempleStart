namespace Seemplest.Core.ServiceObjects.Validation
{
    /// <summary>
    /// This enumeration describes the type of the target the notification belongs to.
    /// </summary>
    public enum NotificationTargetType
    {
        /// <summary>
        /// This notification is related to an operation
        /// </summary>
        Operation = 1,

        /// <summary>
        /// This notification is related to an entity
        /// </summary>
        Entity = 2,

        /// <summary>
        /// This notification is related to a property
        /// </summary>
        Property = 3
    }
}