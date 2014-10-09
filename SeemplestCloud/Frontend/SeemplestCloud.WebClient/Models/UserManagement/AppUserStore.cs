using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Seemplest.Core.DependencyInjection;
using SeemplestCloud.Dto.Subscription;
using SeemplestCloud.Services.SubscriptionService;

namespace SeemplestCloud.WebClient.Models.UserManagement
{
    /// <summary>
    /// This class is responsible for managing user persistence for this application
    /// </summary>
    public class AppUserStore :
        IUserStore<AppUser>,
        IUserPasswordStore<AppUser>,
        IUserEmailStore<AppUser>,
        IUserLockoutStore<AppUser, string>,
        IUserTwoFactorStore<AppUser, string>,
        IUserLoginStore<AppUser>,
        IUserPhoneNumberStore<AppUser>
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Insert a new user
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public async Task CreateAsync(AppUser user)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            user.SecurityStamp = Guid.NewGuid().ToString("N");
            user.CreatedUtc = DateTime.UtcNow;
            await secSrv.InsertUserAsync(MapUser(user));
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public async Task UpdateAsync(AppUser user)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            await secSrv.UpdateUserAsync(MapUser(user));
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task DeleteAsync(AppUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds a user
        /// </summary>
        /// <param name="userId"/>
        /// <returns/>
        public async Task<AppUser> FindByIdAsync(string userId)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            var user = await secSrv.GetUserByIdAsync(new Guid(userId));
            return user == null
                ? await Task.FromResult<AppUser>(null)
                : MapUser(user);
        }

        /// <summary>
        /// Find a user by its user name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <remarks>
        /// For authentication purposes we use the email address as the
        /// user name!
        /// </remarks>
        /// <returns>AppUser information</returns>
        public async Task<AppUser> FindByNameAsync(string userName)
        {
            return await FindByEmailAsync(userName);
        }

        /// <summary>
        /// Set the user password hash
        /// </summary>
        /// <param name="user"/><param name="passwordHash"/>
        /// <returns/>
        public Task SetPasswordHashAsync(AppUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get the user password hash
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<string> GetPasswordHashAsync(AppUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Returns true if a user has a password set
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<bool> HasPasswordAsync(AppUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        /// <summary>
        /// Set the user email
        /// </summary>
        /// <param name="user"/><param name="email"/>
        /// <returns/>
        public Task SetEmailAsync(AppUser user, string email)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get the user email
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<string> GetEmailAsync(AppUser user)
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Returns true if the user email is confirmed
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<bool> GetEmailConfirmedAsync(AppUser user)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Sets whether the user email is confirmed
        /// </summary>
        /// <param name="user"/><param name="confirmed"/>
        /// <returns/>
        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns the user associated with this email
        /// </summary>
        /// <param name="email"/>
        /// <returns/>
        public async Task<AppUser> FindByEmailAsync(string email)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            var user = await secSrv.GetUserByEmailAsync(email);
            return user == null
                ? await Task.FromResult<AppUser>(null)
                : MapUser(user);
        }

        /// <summary>
        /// Returns the DateTimeOffset that represents the end of a user's lockout, any time in the past should be considered
        /// not locked out.
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(AppUser user)
        {
            return Task.FromResult(user.LockedOut ? DateTimeOffset.MaxValue : DateTimeOffset.MinValue);
        }

        /// <summary>
        /// Locks a user out until the specified end date (set to a past date, to unlock a user)
        /// </summary>
        /// <param name="user"/><param name="lockoutEnd"/>
        /// <returns/>
        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset lockoutEnd)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Used to record when an attempt to access the user has failed
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<int> IncrementAccessFailedCountAsync(AppUser user)
        {
            //var failCount = ++user.AccessFailedCount;
            //if (failCount >= 3)
            //{
            //    user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(5);
            //}
            //return Task.FromResult(failCount);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Used to reset the access failed count, typically after the account is successfully accessed
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task ResetAccessFailedCountAsync(AppUser user)
        {
            user.AccessFailedCount = 0;
            //user.LockoutEndDateUtc = null;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns the current number of failed access attempts.  This number usually will be reset whenever the password is
        ///                 verified or the account is locked out.
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<int> GetAccessFailedCountAsync(AppUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Returns whether the user can be locked out.
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<bool> GetLockoutEnabledAsync(AppUser user)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Sets whether the user can be locked out.
        /// </summary>
        /// <param name="user"/><param name="enabled"/>
        /// <returns/>
        public Task SetLockoutEnabledAsync(AppUser user, bool enabled)
        {
            //user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"/><param name="enabled"/>
        /// <returns/>
        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled)
        {
            //user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<bool> GetTwoFactorEnabledAsync(AppUser user)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Adds a user login with the specified provider and key
        /// </summary>
        /// <param name="user"/><param name="login"/>
        /// <returns/>
        public async Task AddLoginAsync(AppUser user, UserLoginInfo login)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            await secSrv.InsertUserAccountAsync(new UserAccountDto
            {
                UserId = new Guid(user.Id),
                Provider = login.LoginProvider,
                ProviderData = login.ProviderKey
            });
        }

        /// <summary>
        /// Removes the user login with the specified combination if it exists
        /// </summary>
        /// <param name="user"/><param name="login"/>
        /// <returns/>
        public async Task RemoveLoginAsync(AppUser user, UserLoginInfo login)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            await secSrv.RemoveUserAccountAsync(new Guid(user.Id), login.LoginProvider);
        }

        /// <summary>
        /// Returns the linked accounts for this user
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            var logins = await secSrv.GetUserAccountsByUserId(new Guid(user.Id));
            return logins.Select(u => new UserLoginInfo(u.Provider, u.ProviderData)).ToList();
        }

        /// <summary>
        /// Returns the user associated with this login
        /// </summary>
        /// <returns/>
        public async Task<AppUser> FindAsync(UserLoginInfo login)
        {
            var secSrv = ServiceManager.GetService<ISubscriptionService>();
            var user = await secSrv.GetUserByProviderDataAsync(login.LoginProvider, login.ProviderKey);
            return user == null ? null : MapUser(user);
        }

        /// <summary>
        /// Set the user's phone number
        /// </summary>
        /// <param name="user"/><param name="phoneNumber"/>
        /// <returns/>
        public Task SetPhoneNumberAsync(AppUser user, string phoneNumber)
        {
            //user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get the user phone number
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<string> GetPhoneNumberAsync(AppUser user)
        {
            return Task.FromResult<string>(null);
        }

        /// <summary>
        /// Returns true if the user phone number is confirmed
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        public Task<bool> GetPhoneNumberConfirmedAsync(AppUser user)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Sets whether the user phone number is confirmed
        /// </summary>
        /// <param name="user"/><param name="confirmed"/>
        /// <returns/>
        public Task SetPhoneNumberConfirmedAsync(AppUser user, bool confirmed)
        {
            //user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Maps the specified UserInfoDto to an AppUser instance
        /// </summary>
        /// <param name="user">UserInfoDto to map</param>
        /// <returns>AppUser instance</returns>
        private static AppUser MapUser(UserDto user)
        {
            return new AppUser(user.Id.ToString())
            {
                SubscriptionId = user.SubscriptionId,
                UserName = user.UserName,
                Email = user.Email,
                SecurityStamp = user.SecurityStamp,
                PasswordHash = user.PasswordHash,
                AccessFailedCount = user.AccessFailedCount,
                CreatedUtc = user.CreatedUtc,
                LastFailedAuthUtc = user.LastFailedAuthUtc,
                LastModifiedUtc = user.LastModifiedUtc,
                LockedOut = user.LockedOut,
                OwnerSuspend = user.OwnerSuspend,
                PasswordResetSuspend = user.PasswordResetSuspend
            };
        }

        /// <summary>
        /// Maps the specified UserInfoDto to an AppUser instance
        /// </summary>
        /// <param name="user">UserInfoDto to map</param>
        /// <returns>AppUser instance</returns>
        private static UserDto MapUser(AppUser user)
        {
            return new UserDto
            {
                Id = new Guid(user.Id),
                SubscriptionId = user.SubscriptionId,
                UserName = user.UserName,
                Email = user.Email,
                SecurityStamp = user.SecurityStamp,
                PasswordHash = user.PasswordHash,
                AccessFailedCount = user.AccessFailedCount,
                CreatedUtc = user.CreatedUtc,
                LastFailedAuthUtc = user.LastFailedAuthUtc,
                LastModifiedUtc = user.LastModifiedUtc,
                LockedOut = user.LockedOut,
                OwnerSuspend = user.OwnerSuspend,
                PasswordResetSuspend = user.PasswordResetSuspend
            };
        }
    }
}