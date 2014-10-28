using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when an invalid invitation code is used
    /// </summary>
    public class InvalidInvitationCodeException: InvalidBusinessRuleException
    {
        public InvalidInvitationCodeException() : base(ScErrorCodes.INVALID_INVITATION_CODE)
        {
        }
    }
}