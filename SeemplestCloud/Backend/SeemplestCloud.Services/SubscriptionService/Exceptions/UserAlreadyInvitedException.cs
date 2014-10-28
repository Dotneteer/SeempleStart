using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when a user is already invited
    /// </summary>
    public class UserAlreadyInvitedException: InvalidBusinessRuleException
    {
        public UserAlreadyInvitedException(string email) : base(ScErrorCodes.USER_NAME_ALREADY_INVITED, email)
        {
        }
    }
}