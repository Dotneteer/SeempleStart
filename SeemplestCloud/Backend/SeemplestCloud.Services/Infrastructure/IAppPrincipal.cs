using System;
using System.Security.Principal;

namespace SeemplestCloud.Services.Infrastructure
{
    /// <summary>
    /// This interface defines a principal that can be used within this app
    /// </summary>
    public interface IAppPrincipal: IPrincipal
    {
        /// <summary>
        /// The ID of the current user
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// The user is a service user?
        /// </summary>
        bool IsServiceUser { get; }

        /// <summary>
        /// Ths ID of the subscription the user belongs to
        /// </summary>
        /// <remarks>
        /// Service users do not have a subscription id
        /// </remarks>
        int? SubscriptionId { get; }

        /// <summary>
        /// Is the user an owner of the subscription?
        /// </summary>
        bool IsSubscriptionOwner { get; }

        /// <summary>
        /// Description of service roles available for the current principal
        /// </summary>
        ServiceRoles ServiceRoles { get; }
    }
}