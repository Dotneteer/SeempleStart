using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when an unknown invitation ID is used.
    /// </summary>
    public class UnknownInvitationIdException: InvalidBusinessRuleException
    {
        public UnknownInvitationIdException(int invitationId) : base(ScErrorCodes.UNKNOWN_INVITATION_ID, invitationId)
        {
        }
    }
}