﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Seemplest.Core.ServiceObjects;
using SeemplestCloud.Dto.Subscription;

namespace SeemplestCloud.Services.SubscriptionService
{
    /// <summary>
    /// This interface defines the operations related to subscriptions
    /// </summary>
    public interface ISubscriptionService: IServiceObject
    {
        /// <summary>
        /// Gets the user with the specified ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserDto> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Gets the user with the specified name
        /// </summary>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <param name="userName">User name</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserDto> GetUserByNameAsync(int? subscriptionId, string userName);

        /// <summary>
        /// Gets the user with the email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserDto> GetUserByEmailAsync(string email);

        /// <summary>
        /// Inserts the specified user into the user database
        /// </summary>
        /// <param name="user">User information</param>
        Task InsertUserAsync(UserDto user);

        /// <summary>
        /// Updates the specified user in the user database
        /// </summary>
        /// <param name="user">User information</param>
        Task UpdateUserAsync(UserDto user);

        /// <summary>
        /// Create a new subscription with the specified data
        /// </summary>
        /// <param name="subscription">Subscription object</param>
        /// <param name="userId">User owning the subscription</param>
        /// <returns>The ID of the new subscription</returns>
        Task<int> CreateSubscriptionAsync(SubscriptionDto subscription, string userId);

        /// <summary>
        /// Gets user information by the email address provided
        /// </summary>
        /// <param name="userEmail">Email of the user (used as unique ID)</param>
        /// <returns>User token</returns>
        Task<UserTokenDto> GetUserTokenByEmailAsync(string userEmail);

        /// <summary>
        /// Gets user information by the user ID provided
        /// </summary>
        /// <param name="userId">User ID within the system</param>
        /// <returns>User token</returns>
        Task<UserTokenDto> GetUserTokenByIdAsync(Guid userId);

        /// <summary>
        /// Gets user information by its provider data
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        /// <returns>User token</returns>
        Task<UserTokenDto> GetUserTokenByProviderDataAsync(string provider, string providerData);

        /// <summary>
        /// Gets the user information by its provider data
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        Task<UserDto> GetUserByProviderDataAsync(string provider, string providerData);

        /// <summary>
        /// Inserts a new user account into the database
        /// </summary>
        /// <param name="account">Account information</param>
        /// <returns></returns>
        Task InsertUserAccountAsync(UserAccountDto account);

        /// <summary>
        /// Removes the specified user account from the database
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="provider">Account provider</param>
        Task RemoveUserAccountAsync(Guid userId, string provider);

        /// <summary>
        /// Gets the login account belonging to a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Login accounts</returns>
        Task<List<UserAccountDto>> GetUserAccountsByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets the users invited to join to the subscription of the current user.
        /// </summary>
        /// <returns>List of user invitations</returns>
        Task<List<UserInvitationCoreDto>> GetInvitedUsersAsync();

        /// <summary>
        /// Sends an invitation to the specified user
        /// </summary>
        /// <param name="userInfo">Information about the invited user</param>
        Task InviteUserAsync(InviteUserDto userInfo);

        /// <summary>
        /// Confirms the specified invitation code, and creates the appropriate user.
        /// </summary>
        /// <param name="invitationCode">Invitation code</param>
        Task<UserInvitationCoreDto> GetUserInvitationByCodeAsync(string invitationCode);

        /// <summary>
        /// Assigns the specified user to the given subscription
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="subscriptionId">Subscription ID</param>
        Task AssignUserToSubscription(Guid userId, int subscriptionId);

        /// <summary>
        /// Sets the state of the invitation with the specified id
        /// </summary>
        /// <param name="invitationId">Invitation ID</param>
        /// <param name="state">New invitation state</param>
        Task SetInvitationState(int invitationId, string state);
    }
}