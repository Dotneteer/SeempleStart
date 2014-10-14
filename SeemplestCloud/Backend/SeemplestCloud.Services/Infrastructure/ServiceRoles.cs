using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SeemplestCloud.Services.Infrastructure
{
    /// <summary>
    /// This class manages a collection of service roles
    /// </summary>
    public class ServiceRoles: ReadOnlyCollection<ServiceRoleDescription>
    {
        /// <summary>
        /// Initializes this instance with the specified list of service roles
        /// </summary>
        /// <param name="list"></param>
        public ServiceRoles(IList<ServiceRoleDescription> list) : base(list)
        {
        }

        /// <summary>
        /// Checks whether the specified role is defined in this object or not
        /// </summary>
        /// <param name="roleDescription"></param>
        /// <returns></returns>
        public bool HasRole(string roleDescription)
        {
            if (string.IsNullOrWhiteSpace(roleDescription))
            {
                return false;
            }

            var parts = roleDescription.Split(new char[':'], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return false;
            }

            var service = this.FirstOrDefault(i => String.Compare(i.ServiceCode, parts[0], 
                StringComparison.InvariantCultureIgnoreCase) == 0);
            return service != null && service.AvailableRoles.Contains(parts[1], StringComparer.InvariantCultureIgnoreCase);
        }
    }
}