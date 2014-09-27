using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Younderwater.Webclient.Models.UserManagement
{
    /// <summary>
    /// This class represents an application user with its properties.
    /// </summary>
    public class AppUser: IUser
    {
        /// <summary>
        /// Unique key for the user
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Unique username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Eamil of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// True if the email is confirmed, default is false
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// The salted/hashed form of the user password
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// PhoneNumber for the user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// True if the phone number is confirmed, default is false
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Is two factor enabled for the user
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTimeOffset? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// Is lockout enabled for this user
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        public bool Active { get; set; }

        public DateTime Created { get; set; }

        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Creates an empty user
        /// </summary>
        public AppUser()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Creates a new user with the specified ID
        /// </summary>
        /// <param name="id">User Id</param>
        public AppUser(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Creates a ClaimsIdentity for the user with the specified manager object
        /// </summary>
        /// <param name="manager">UserManager object</param>
        /// <returns>The claims that identify the user</returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}