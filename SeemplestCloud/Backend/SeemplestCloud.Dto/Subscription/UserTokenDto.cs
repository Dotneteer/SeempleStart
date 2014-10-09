using System;
using System.Collections.Generic;

namespace SeemplestCloud.Dto.Subscription
{
    /// <summary>
    /// This class provides a model for serialization
    /// </summary>
    public class UserTokenDto
    {
        /// <summary>
        /// The ID of the current user
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user is a service user?
        /// </summary>
        public bool IsServiceUser { get; set; }

        /// <summary>
        /// Ths ID of the subscription the user belongs to
        /// </summary>
        /// <remarks>
        /// Service users do not have a subscription id
        /// </remarks>
        public int? SubscriptionId { get; set; }

        /// <summary>
        /// Is the user an owner of the subscription?
        /// </summary>
        public bool IsSubscriptionOwner { get; set; }

        /// <summary>
        /// Service roles held by the user
        /// </summary>
        public List<ServiceRolesDto> ServiceRoles { get; set; }
    }

    public class ServiceRolesDto
    {
        /// <summary>
        /// Code of the service
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Available service roles
        /// </summary>
        public List<string> AvailableRoles { get; set; }
    }
}