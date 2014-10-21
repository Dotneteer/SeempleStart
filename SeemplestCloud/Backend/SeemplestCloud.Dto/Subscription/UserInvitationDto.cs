using System;

namespace SeemplestCloud.Dto.Subscription
{
    public class UserInvitationCoreDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int? SubscriptionId { get; set; }
        public string InvitedEmail { get; set; }
        public string InvitedUserName { get; set; }
        public DateTimeOffset? ExpirationDateUtc { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
    }

    public class UserInvitationDto : UserInvitationCoreDto
    {
        public string InvitationCode { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset? LastModifiedUtc { get; set; }
    }
}