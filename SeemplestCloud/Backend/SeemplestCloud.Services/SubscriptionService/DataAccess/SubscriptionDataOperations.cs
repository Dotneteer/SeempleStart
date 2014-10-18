using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeemplestBlocks.Core.ServiceInfrastructure;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    /// <summary>
    /// This class implements all data access operations used by subscription management
    /// </summary>
    public class SubscriptionDataOperations : CoreSqlDataAccessOperationBase, ISubscriptionDataOperations
    {
        /// <summary>
        /// Initializes the object with the specified connection information
        /// </summary>
        /// <param name="connectionOrName">Connection information</param>
        public SubscriptionDataOperations(string connectionOrName)
            : base(connectionOrName)
        {
        }

        /// <summary>
        /// Gets all User records from the database
        /// </summary>
        /// <returns>
        /// List of User records
        /// </returns>
        public async Task<List<UserRecord>> GetAllUserAsync()
        {
            return await OperationAsync(ctx => ctx.FetchAsync<UserRecord>());
        }

        /// <summary>
        /// Gets a User record by its primary key values
        /// </summary>
        /// <param name="id">Id key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        public async Task<UserRecord> GetUserByIdAsync(Guid id)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserRecord>(
                "where [Id]=@0",
                id));
        }

        /// <summary>
        /// Gets a User record by its "AK_Email" alternate key values
        /// </summary>
        public async Task<UserRecord> GetUserByEmailAsync(string email)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserRecord>(
                "where [Email]=@0",
                email));
        }

        /// <summary>
        /// Gets a User record by its "AK_UserName" alternate key values
        /// </summary>
        public async Task<UserRecord> GetUserByUserNameAsync(string userName)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserRecord>(
                "where [UserName]=@0",
                userName));
        }

        /// <summary>
        /// Inserts a User record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertUserAsync(UserRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }

        /// <summary>
        /// Updates a User record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        public async Task UpdateUserAsync(UserRecord record)
        {
            await OperationAsync(async ctx =>
            {
                var userInDb = await ctx.FirstOrDefaultAsync<UserRecord>(
                    "where [Id] = @0", record.Id);
                if (userInDb == null) return;
                userInDb.MergeChangesFrom(record);
                userInDb.LastModifiedUtc = DateTimeOffset.UtcNow;
                await ctx.UpdateAsync(userInDb);
            });
        }

        /// <summary>
        /// Deletes a User the specidfied record
        /// </summary>
        /// <param name="id">Id key value</param>
        public async Task DeleteUserAsync(Guid id)
        {
            await OperationAsync(ctx => ctx.DeleteByIdAsync<UserRecord>(
                id));
        }

        /// <summary>
        /// Gets a Subscription record by its primary key values
        /// </summary>
        /// <param name="id">Id key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        public async Task<SubscriptionRecord> GetSubscriptionByIdAsync(int id)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<SubscriptionRecord>(
                "where [Id]=@0",
                id));
        }

        /// <summary>
        /// Inserts a Subscription record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertSubscriptionAsync(SubscriptionRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }

        /// <summary>
        /// Updates a Subscription record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        public async Task UpdateSubscriptionAsync(SubscriptionRecord record)
        {
            await OperationAsync(async ctx =>
            {
                var recordInDb = await ctx.FirstOrDefaultAsync<SubscriptionRecord>(
                    "where [Id] = @0", record.Id);
                if (recordInDb == null) return;
                recordInDb.MergeChangesFrom(record);
                recordInDb.LastModifiedUtc = DateTimeOffset.UtcNow;
                await ctx.UpdateAsync(recordInDb);
            });
        }

        /// <summary>
        /// Gets a SubscriptionOwner record by its primary key values
        /// </summary>
        /// <param name="subscriptionId">SubscriptionId key value</param>
        /// <param name="userId">UserId key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        public async Task<SubscriptionOwnerRecord> GetSubscriptionOwnerByIdAsync(int subscriptionId, Guid userId)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<SubscriptionOwnerRecord>(
                "where [SubscriptionId]=@0 and [UserId]=@1",
                subscriptionId, userId));
        }

        /// <summary>
        /// Inserts a SubscriptionOwner record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertSubscriptionOwnerAsync(SubscriptionOwnerRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }

        /// <summary>
        /// Deletes a SubscriptionOwner the specidfied record
        /// </summary>
        /// <param name="subscriptionId">SubscriptionId key value</param>
        /// <param name="userId">UserId key value</param>
        public async Task DeleteSubscriptionOwnerAsync(int subscriptionId, Guid userId)
        {
            await OperationAsync(ctx => ctx.DeleteByIdAsync<SubscriptionOwnerRecord>(
                subscriptionId, userId));
        }

        /// <summary>
        /// Gets a UserAccount record by its "AK_UserAccountOnProviderData" alternate key values
        /// </summary>
        public async Task<UserAccountRecord> GetUserAccountByProviderAsync(string provider, string providerData)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserAccountRecord>(
                "where [Provider]=@0 and [ProviderData]=@1",
                provider, providerData));
        }

        /// <summary>
        /// Inserts a UserAccount record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertUserAccountAsync(UserAccountRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }

        /// <summary>
        /// Deletes a UserAccount the specidfied record
        /// </summary>
        /// <param name="userId">UserId key value</param>
        /// <param name="provider">Provider key value</param>
        public async Task DeleteUserAccountAsync(Guid userId, string provider)
        {
            await OperationAsync(ctx => ctx.DeleteByIdAsync<UserAccountRecord>(
                userId, provider));
        }

        /// <summary>
        /// Gets a UserAccount records by user ID
        /// </summary>
        public async Task<List<UserAccountRecord>> GetUserAccountsByUserIdAsync(Guid userId)
        {
            return await OperationAsync(ctx => ctx.FetchAsync<UserAccountRecord>(
                "where [UserId]=@0",
                userId));
        }

        /// <summary>
        /// Gets a UserInvitation record by its primary key values
        /// </summary>
        /// <param name="id">Id key value</param>
        /// <returns>The record if found; otherwise, null</returns>
        public async Task<UserInvitationRecord> GetUserInvitationByIdAsync(int id)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<UserInvitationRecord>(
                "where [Id]=@0",
                id));
        }

        /// <summary>
        /// Gets UserInvitation records by its "FK_SubscriptionOfInvitation" foreign key values
        /// </summary>
        public async Task<List<UserInvitationRecord>> GetUserInvitationBySubscriptionAsync(int? subscriptionId)
        {
            return await OperationAsync(ctx => ctx.FetchAsync<UserInvitationRecord>(
                "where [SubscriptionId]=@0",
                subscriptionId));
        }

        /// <summary>
        /// Gets UserInvitation records by its "FK_UserOfInvitation" foreign key values
        /// </summary>
        public async Task<List<UserInvitationRecord>> GetUserInvitationByUserAsync(Guid userId)
        {
            return await OperationAsync(ctx => ctx.FetchAsync<UserInvitationRecord>(
                "where [UserId]=@0",
                userId));
        }

        /// <summary>
        /// Inserts a UserInvitation record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertUserInvitationAsync(UserInvitationRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }

        /// <summary>
        /// Updates a UserInvitation record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        public async Task UpdateUserInvitationAsync(UserInvitationRecord record)
        {
            await OperationAsync(ctx => ctx.UpdateAsync(record));
        }

        /// <summary>
        /// Deletes a UserInvitation the specidfied record
        /// </summary>
        /// <param name="id">Id key value</param>
        public async Task DeleteUserInvitationAsync(int id)
        {
            await OperationAsync(ctx => ctx.DeleteByIdAsync<UserInvitationRecord>(
                id));
        }
    }
}