using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    /// <summary>
    /// This interface defines all data access operations used by subscription management
    /// </summary>
    public interface ISubscriptionDataOperations: IDataAccessOperation
    {
        /// <summary>
        /// Gets all User records from the database
        /// </summary>
        /// <returns>
        /// List of User records
        /// </returns>
        Task<List<UserRecord>> GetAllUserAsync();

        /// <summary>
        /// Gets a User record by its primary key values
        /// </summary>
        /// <param name="id">Id key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        Task<UserRecord> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Gets a User record by its "AK_Email" alternate key values
        /// </summary>
        Task<UserRecord> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets a User record by its "AK_UserName" alternate key values
        /// </summary>
        Task<UserRecord> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// Inserts a User record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertUserAsync(UserRecord record);

        /// <summary>
        /// Updates a User record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        Task UpdateUserAsync(UserRecord record);

        /// <summary>
        /// Deletes a User the specidfied record
        /// </summary>
        /// <param name="id">Id key value</param>
        Task DeleteUserAsync(Guid id);

        /// <summary>
        /// Gets a Subscription record by its primary key values
        /// </summary>
        /// <param name="id">Id key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        Task<SubscriptionRecord> GetSubscriptionByIdAsync(int id);

        /// <summary>
        /// Inserts a Subscription record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertSubscriptionAsync(SubscriptionRecord record);

        /// <summary>
        /// Updates a Subscription record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        Task UpdateSubscriptionAsync(SubscriptionRecord record);

        /// <summary>
        /// Gets a SubscriptionOwner record by its primary key values
        /// </summary>
        /// <param name="subscriptionId">SubscriptionId key value</param>
        /// <param name="userId">UserId key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        Task<SubscriptionOwnerRecord> GetSubscriptionOwnerByIdAsync(int subscriptionId, Guid userId);

        /// <summary>
        /// Inserts a SubscriptionOwner record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertSubscriptionOwnerAsync(SubscriptionOwnerRecord record);

        /// <summary>
        /// Deletes a SubscriptionOwner the specidfied record
        /// </summary>
        /// <param name="subscriptionId">SubscriptionId key value</param>
        /// <param name="userId">UserId key value</param>
        Task DeleteSubscriptionOwnerAsync(int subscriptionId, Guid userId);

        /// <summary>
        /// Gets a UserAccount record by its "AK_UserAccountOnProviderData" alternate key values
        /// </summary>
        Task<UserAccountRecord> GetUserAccountByProviderAsync(string provider, string providerData);

        /// <summary>
        /// Inserts a UserAccount record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertUserAccountAsync(UserAccountRecord record);

        /// <summary>
        /// Deletes a UserAccount the specidfied record
        /// </summary>
        /// <param name="userId">UserId key value</param>
        /// <param name="provider">Provider key value</param>
        Task DeleteUserAccountAsync(Guid userId, string provider);

        /// <summary>
        /// Gets a UserAccount records by user ID
        /// </summary>
        Task<List<UserAccountRecord>> GetUserAccountsByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets a UserInvitation record by its primary key values
        /// </summary>
        /// <param name="id">Id key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        Task<UserInvitationRecord> GetUserInvitationByIdAsync(int id);

        /// <summary>
        /// Gets a UserInvitation record by its "FK_SubscriptionOfInvitation" foreign key values
        /// </summary>
        Task<UserInvitationRecord> GetUserInvitationBySubscriptionAsync(int? subscriptionId);

        /// <summary>
        /// Gets a UserInvitation record by its "FK_UserOfInvitation" foreign key values
        /// </summary>
        Task<UserInvitationRecord> GetUserInvitationByUserAsync(Guid userId);

        /// <summary>
        /// Inserts a UserInvitation record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertUserInvitationAsync(UserInvitationRecord record);

        /// <summary>
        /// Updates a UserInvitation record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        Task UpdateUserInvitationAsync(UserInvitationRecord record);

        /// <summary>
        /// Deletes a UserInvitation the specidfied record
        /// </summary>
        /// <param name="id">Id key value</param>
        Task DeleteUserInvitationAsync(int id);
    }
}