using System;
using System.Collections.Generic;
using System.Security.Principal;
using Newtonsoft.Json;

namespace SeemplestCloud.Services.Infrastructure
{
    /// <summary>
    /// This class implements a principal that can be used within this app
    /// </summary>
    public class AppPrincipal : IAppPrincipal
    {
        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.
        /// </returns>
        public IIdentity Identity { get; private set; }

        /// <summary>
        /// The ID of the current user
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// The user is a service user?
        /// </summary>
        public bool IsServiceUser { get; private set; }

        /// <summary>
        /// Ths ID of the subscription the user belongs to
        /// </summary>
        /// <remarks>
        /// Service users do not have a subscription id
        /// </remarks>
        public int? SubscriptionId { get; private set; }

        /// <summary>
        /// Is the user an owner of the subscription?
        /// </summary>
        public bool IsSubscriptionOwner { get; private set; }

        /// <summary>
        /// Description of service roles available for the current principal
        /// </summary>
        public ServiceRoles ServiceRoles { get; private set; }

        /// <summary>
        /// Initializes this object with the specified properties
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userName">User name</param>
        /// <param name="isServiceUser">Is this user a service user?</param>
        /// <param name="subscriptionId">Optional subscription ID</param>
        /// <param name="isSubscriptionOwner">Is this user a subscription owner?</param>
        /// <param name="roles">Available service roles</param>
        public AppPrincipal(Guid userId, string userName, bool isServiceUser, int? subscriptionId, bool isSubscriptionOwner,
            ServiceRoles roles)
        {
            Identity = new GenericIdentity(userName);
            UserId = userId;
            IsServiceUser = isServiceUser;
            SubscriptionId = subscriptionId;
            IsSubscriptionOwner = isSubscriptionOwner;
            ServiceRoles = roles ?? new ServiceRoles(new List<ServiceRoleDescription>());
        }

        /// <summary>
        /// Initializes this object with the specified properties
        /// </summary>
        /// <param name="model">Serialization model</param>
        /// <param name="roles">Available service roles</param>
        public AppPrincipal(SerializationModel model, ServiceRoles roles)
        {
            Identity = new GenericIdentity(model.UserName);
            UserId = model.UserId;
            IsServiceUser = model.IsServiceUser;
            SubscriptionId = model.SubscriptionId;
            IsSubscriptionOwner = model.IsSubscriptionOwner;
            ServiceRoles = roles ?? new ServiceRoles(new List<ServiceRoleDescription>());
        }
        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        /// <param name="role">The name of the role for which to check membership. </param>
        public bool IsInRole(string role)
        {
            role = role.Trim();
            if (role == "$" && IsServiceUser || role == "#" && IsSubscriptionOwner)
            {
                return true;
            }
            return ServiceRoles != null && ServiceRoles.HasRole(role);
        }

        /// <summary>
        /// Serializes this object into JSON
        /// </summary>
        /// <remarks>ServiceRoles are not serialized</remarks>
        /// <returns>JSON representation of this object</returns>
        public string Serialize()
        {
            var model = new SerializationModel
            {
                UserId = UserId,
                UserName = Identity.Name,
                SubscriptionId = SubscriptionId,
                IsServiceUser = IsServiceUser,
                IsSubscriptionOwner = IsSubscriptionOwner
            };
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// This class provides a model for serialization
        /// </summary>
        public class SerializationModel
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
        }
    }
}