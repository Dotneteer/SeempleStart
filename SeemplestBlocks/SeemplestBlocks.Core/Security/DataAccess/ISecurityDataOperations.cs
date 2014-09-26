using System.Collections.Generic;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;

namespace SeemplestBlocks.Core.Security.DataAccess
{
    /// <summary>
    /// This interface defines the data access operations related to security
    /// </summary>
    public interface ISecurityDataOperations: IDataAccessOperation
    {
        /// <summary>
        /// Gets the user by its name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>User record if found; otherwise, null</returns>
        Task<UserRecord> GetUserByNameAsync(string userName);

        /// <summary>
        /// Gets the user by its email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User record if found; otherwise, null</returns>
        Task<UserRecord> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets the user by its ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User record if found; otherwise, null</returns>
        Task<UserRecord> GetUserByIdAsync(string userId);

        /// <summary>
        /// Inserts a new user into the database
        /// </summary>
        /// <param name="user">User record</param>
        Task InsertUserAsync(UserRecord user);

        /// <summary>
        /// Updates the user in the database
        /// </summary>
        /// <param name="user">User record</param>
        Task UpdateUserAsync(UserRecord user);

        /// <summary>
        /// Gets the user account record by its provider info
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        Task<UserAccountRecord> GetUserAccountByProvider(string provider, string providerData);

        /// <summary>
        /// Inserts a new user account into the database
        /// </summary>
        /// <param name="userAccount"></param>
        Task InsertUserAccountAsync(UserAccountRecord userAccount);

        /// <summary>
        /// Gets all logins belonging to a specifiec user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>The list of user accounts belonging to the user</returns>
        Task<List<UserAccountRecord>> GetUserAccounts(string userId);

        /// <summary>
        /// Removes the specified login from the user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="providerId">ProviderId</param>
        Task RemoveLogin(string userId, string providerId);
    }
}