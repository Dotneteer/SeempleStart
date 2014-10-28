using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when an unknown user ID is used.
    /// </summary>
    public class UnknownUserIdException: InvalidBusinessRuleException
    {
        public UnknownUserIdException(string userId) : base(ScErrorCodes.UNKNOWN_EMAIL, userId)
        {
        }
    }
}