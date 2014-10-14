using System.Collections.Generic;

namespace SeemplestCloud.WebClient.Models.UserManagement
{
    /// <summary>
    /// This class describes a service with its available roles
    /// </summary>
    public class ServiceRoleDescription
    {
        /// <summary>
        /// Code of the service
        /// </summary>
        public string ServiceCode { get; private set; }

        /// <summary>
        /// Available service roles
        /// </summary>
        public List<string> AvailableRoles { get; private set; }

        /// <summary>
        /// Initializes this object with the specified properties
        /// </summary>
        /// <param name="serviceCode">Service code</param>
        /// <param name="availableRoles">Available roles</param>
        public ServiceRoleDescription(string serviceCode, List<string> availableRoles)
        {
            ServiceCode = serviceCode;
            AvailableRoles = availableRoles;
        }
    }
}