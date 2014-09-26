using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.ServiceObjects;
using SeemplestBlocks.Core.Security.DataAccess;
using SeemplestBlocks.Dto.Security;

namespace SeemplestBlocks.Core.Security
{
    public class SecurityService : ServiceObjectBase, ISecurityService
    {
        /// <summary>
        /// Gets the user with the specified ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserInfoDto> GetUserByIdAsync(string userId)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISecurityDataOperations>())
            {
                var user = await ctx.GetUserByIdAsync(userId);
                return user == null
                    ? await Task.FromResult<UserInfoDto>(null)
                    : MapUser(user);
            }
        }

        /// <summary>
        /// Gets the user with the specified name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserInfoDto> GetUserByNameAsync(string userName)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISecurityDataOperations>())
            {
                var user = await ctx.GetUserByNameAsync(userName);
                return user == null 
                    ? await Task.FromResult<UserInfoDto>(null) 
                    : MapUser(user);
            }
        }

        /// <summary>
        /// Gets the user with the email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserInfoDto> GetUserByEmailAsync(string email)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISecurityDataOperations>())
            {
                var user = await ctx.GetUserByEmailAsync(email);
                return user == null
                    ? await Task.FromResult<UserInfoDto>(null)
                    : MapUser(user);
            }
        }

        /// <summary>
        /// Inserts the specified user into the user database
        /// </summary>
        /// <param name="user">User information</param>
        /// <returns></returns>
        public async Task InsertUserAsync(UserInfoDto user)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISecurityDataOperations>())
            {
                await ctx.InsertUserAsync(MapUser(user));
            }
        }

        /// <summary>
        /// Updates the specified user in the user database
        /// </summary>
        /// <param name="user">User information</param>
        public async Task UpdateUserAsync(UserInfoDto user)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISecurityDataOperations>())
            {
                await ctx.UpdateUserAsync(MapUser(user));
            }
        }

        /// <summary>
        /// Gets the user information by its provider data
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserInfoDto> GetUserByProviderData(string provider, string providerData)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISecurityDataOperations>())
            {
                var account = await ctx.GetUserAccountByProvider(provider, providerData);
                if (account == null) return null;
                var user = await ctx.GetUserByIdAsync(account.UserId);
                return user == null ? null : MapUser(user);
            }
        }

        /// <summary>
        /// Inserts a new user account into the database
        /// </summary>
        /// <param name="userAccount">User account information</param>
        public async Task InsertUserAccount(UserAccountDto userAccount)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISecurityDataOperations>())
            {
                await ctx.InsertUserAccountAsync(new UserAccountRecord
                {
                    UserId = userAccount.UserId,
                    Provider = userAccount.Provider,
                    ProviderData = userAccount.ProviderData
                });
            }
        }

        /// <summary>
        /// Gets the login account that belong to the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of users</returns>
        public async Task<List<UserAccountDto>> GetUserLogins(string userId)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISecurityDataOperations>())
            {
                return (await ctx.GetUserAccounts(userId))
                    .Select(u => new UserAccountDto
                    {
                        UserId = userId,
                        Provider = u.Provider,
                        ProviderData = u.ProviderData
                    }).ToList();
            }
        }

        /// <summary>
        /// Removes the specified login from the user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="providerId">ProviderId</param>
        public async Task RemoveLogin(string userId, string providerId)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISecurityDataOperations>())
            {
                await ctx.RemoveLogin(userId, providerId);
            }
        }

        /// <summary>
        /// Maps the specified UserRecord to a UserInfoDto instance
        /// </summary>
        /// <param name="user">UserRecord to map</param>
        /// <returns>UserInfoDto instance</returns>
        public static UserInfoDto MapUser(UserRecord user)
        {
            return new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                SecurityStamp = user.SecurityStamp,
                EmailConfirmed = user.EmailConfirmed,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                LockoutEndDateUtc = user.LockOutEndDateUtc,
                AccessFailedCount = user.AccessFailedCount,
                Active = user.Active,
                Created = user.Created,
                LastModified = user.LastModified,
            };
        }

        /// <summary>
        /// Maps the specified UserInfoDto to a UserRecord instance
        /// </summary>
        /// <param name="user">UserInfoDto to map</param>
        /// <returns>UserRecord instance</returns>
        public static UserRecord MapUser(UserInfoDto user)
        {
            return new UserRecord
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                SecurityStamp = user.SecurityStamp,
                EmailConfirmed = user.EmailConfirmed,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                LockOutEndDateUtc = user.LockoutEndDateUtc,
                AccessFailedCount = user.AccessFailedCount,
                Active = user.Active,
                Created = user.Created,
                LastModified = user.LastModified,
            };
        }
    }
}