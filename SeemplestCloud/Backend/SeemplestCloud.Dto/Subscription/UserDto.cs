using System;

namespace SeemplestCloud.Dto.Subscription
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.08. 10:04:07
    public class UserDto
    {
        public Guid Id { get; set; }
        public int? SubscriptionId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string SecurityStamp { get; set; }
        public string PasswordHash { get; set; }
        public DateTimeOffset? LastFailedAuthUtc { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockedOut { get; set; }
        public bool OwnerSuspend { get; set; }
        public bool PasswordResetSuspend { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset? LastModifiedUtc { get; set; }
    }
}
