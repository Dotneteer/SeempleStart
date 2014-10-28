using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto;

namespace SeemplestCloud.Services.SubscriptionService.Exceptions
{
    /// <summary>
    /// This exceptions is raised when an unknown provider data is used.
    /// </summary>
    public class UnknownProviderDataException: InvalidBusinessRuleException
    {
        public UnknownProviderDataException(string provider, string providerData) : 
            base(ScErrorCodes.UNKNOWN_EMAIL, string.Format("{0}:{1}", provider, providerData))
        {
        }
    }
}