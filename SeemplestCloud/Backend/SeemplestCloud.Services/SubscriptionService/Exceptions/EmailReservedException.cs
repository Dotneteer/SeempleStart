using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when an already used email address is to be assigned to a user
    /// </summary>
    public class EmailReservedException: InvalidBusinessRuleException
    {
        public EmailReservedException(string email) : base(ScErrorCodes.EMAIL_RESERVED, email)
        {
        }
    }
}