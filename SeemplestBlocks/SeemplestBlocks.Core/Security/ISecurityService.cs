using System.Collections.Generic;
using System.Threading.Tasks;
using Seemplest.Core.ServiceObjects;
using SeemplestBlocks.Dto.Security;

namespace SeemplestBlocks.Core.Security
{
    /// <summary>
    /// This interface defines the service operations in regard to security
    /// </summary>
    public interface ISecurityService: IServiceObject
    {
        /// <summary>
        /// Gets the user with the specified ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserInfoDto> GetUserByIdAsync(string userId);

        /// <summary>
        /// Gets the user with the specified name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserInfoDto> GetUserByNameAsync(string userName);

        /// <summary>
        /// Gets the user with the email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserInfoDto> GetUserByEmailAsync(string email);

        /// <summary>
        /// Inserts the specified user into the user database
        /// </summary>
        /// <param name="user">User information</param>
        Task InsertUserAsync(UserInfoDto user);

        /// <summary>
        /// Updates the specified user in the user database
        /// </summary>
        /// <param name="user">User information</param>
        Task UpdateUserAsync(UserInfoDto user);

        /// <summary>
        /// Gets the user information by its provider data
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserInfoDto> GetUserByProviderData(string provider, string providerData);

        /// <summary>
        /// Inserts a new user account into the database
        /// </summary>
        /// <param name="userAccount">User account information</param>
        Task InsertUserAccount(UserAccountDto userAccount);

        /// <summary>
        /// Gets the login account that belong to the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of users</returns>
        Task<List<UserAccountDto>> GetUserLogins(string userId);

        /// <summary>
        /// Removes the specified login from the user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="providerId">ProviderId</param>
        Task RemoveLogin(string userId, string providerId);
    }
}