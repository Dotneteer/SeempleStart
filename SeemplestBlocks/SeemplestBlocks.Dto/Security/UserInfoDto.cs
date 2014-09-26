using System;

namespace SeemplestBlocks.Dto.Security
{
    /// <summary>
    /// This class defines a DTO that describes user information
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// Unique key for the user
        /// </summary>
        public string Id { get; set; }

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
        /// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public DateTimeOffset? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// </summary>
        public int AccessFailedCount { get; set; }

        public bool Active { get; set; }

        public DateTime Created { get; set; }

        public DateTime? LastModified { get; set; }
    }
}