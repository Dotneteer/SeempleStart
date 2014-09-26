using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeemplestBlocks.Core.ServiceInfrastructure;

namespace SeemplestBlocks.Core.Security.DataAccess
{
    /// <summary>
    /// This interface defines the data access operations related to security
    /// </summary>
    public class SecurityDataOperations : CoreSqlDataAccessOperationBase, ISecurityDataOperations
    {
        /// <summary>
        /// Initializes this object with the specified connection information
        /// </summary>
        /// <param name="connectionOrName">Connection information</param>
        public SecurityDataOperations(string connectionOrName) : base(connectionOrName)
        {
        }

        /// <summary>
        /// Gets the user by its name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>User record if found; otherwise, null</returns>
        public async Task<UserRecord> GetUserByNameAsync(string userName)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserRecord>(
                "where [UserName] = @0", userName));
        }

        /// <summary>
        /// Gets the user by its email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User record if found; otherwise, null</returns>
        public async Task<UserRecord> GetUserByEmailAsync(string email)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserRecord>(
                "where [Email] = @0", email));
        }

        /// <summary>
        /// Gets the user by its ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User record if found; otherwise, null</returns>
        public async Task<UserRecord> GetUserByIdAsync(string userId)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserRecord>(
                "where [Id] = @0", userId));
        }

        /// <summary>
        /// Inserts a new user into the database
        /// </summary>
        /// <param name="user">User record</param>
        public async Task InsertUserAsync(UserRecord user)
        {
            await OperationAsync(ctx => ctx.InsertAsync(user));
        }

        /// <summary>
        /// Updates the user in the database
        /// </summary>
        /// <param name="user">User record</param>
        public async Task UpdateUserAsync(UserRecord user)
        {
            await OperationAsync(async ctx =>
            {
                var userInDb = await ctx.FirstOrDefaultAsync<UserRecord>(
                    "where [Id] = @0", user.Id);
                if (userInDb == null) return;
                userInDb.MergeChangesFrom(user);
                userInDb.LastModified = DateTime.UtcNow;
                await ctx.UpdateAsync(userInDb);
            });
        }

        /// <summary>
        /// Gets the user account record by its provider info
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        /// <returns></returns>
        public async Task<UserAccountRecord> GetUserAccountByProvider(string provider, string providerData)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserAccountRecord>(
               "where [Provider]=@0 and [ProviderData]=@1", provider, providerData));
        }

        /// <summary>
        /// Inserts a new user account into the database
        /// </summary>
        /// <param name="userAccount"></param>
        public async Task InsertUserAccountAsync(UserAccountRecord userAccount)
        {
            await OperationAsync(ctx => ctx.InsertAsync(userAccount));
        }

        /// <summary>
        /// Gets all logins belonging to a specifiec user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>The list of user accounts belonging to the user</returns>
        public async Task<List<UserAccountRecord>> GetUserAccounts(string userId)
        {
            return await OperationAsync(ctx => ctx.FetchAsync<UserAccountRecord>(
                "where [UserId]=@0", userId));
        }

        /// <summary>
        /// Removes the specified login from the user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="providerId">ProviderId</param>
        public async Task RemoveLogin(string userId, string providerId)
        {
            await OperationAsync(ctx => ctx.DeleteByIdAsync<UserAccountRecord>(userId, providerId));
        }
    }
}