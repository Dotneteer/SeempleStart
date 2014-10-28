using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when a user with the specified email is already invited
    /// </summary>
    public class EmailAlreadyInvitedException: InvalidBusinessRuleException
    {
        public EmailAlreadyInvitedException(string email) : base(ScErrorCodes.EMAIL_ALREADY_INVITED, email)
        {
        }
    }
}