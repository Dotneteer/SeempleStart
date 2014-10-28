using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when an unknown email address is used.
    /// </summary>
    public class UnknownEmailException: InvalidBusinessRuleException
    {
        public UnknownEmailException(string email) : base(ScErrorCodes.UNKNOWN_EMAIL, email)
        {
        }
    }
}