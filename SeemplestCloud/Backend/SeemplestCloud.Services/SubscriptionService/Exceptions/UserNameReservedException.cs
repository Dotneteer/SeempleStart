using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when an already used name is to be assigned to a user
    /// </summary>
    public class UserNameReservedException: InvalidBusinessRuleException
    {
        public UserNameReservedException(string email) : base(ScErrorCodes.USER_NAME_RESERVED, email)
        {
        }
    }
}