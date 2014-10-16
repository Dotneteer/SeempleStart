using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;
using SeemplestCloud.Dto.Subscription;
using SeemplestCloud.Services.Infrastructure;
using SeemplestCloud.Services.SubscriptionService.DataAccess;
using SeemplestCloud.Services.SubscriptionService.Exceptions;
using UserRecord = SeemplestCloud.Services.SubscriptionService.DataAccess.UserRecord;

namespace SeemplestCloud.Services.SubscriptionService
{
    /// <summary>
    /// This class implements the operations related to subscriptions
    /// </summary>
    public class SubscriptionService : ServiceWithAppPrincipalBase, ISubscriptionService
    {
        /// <summary>
        /// Gets the user with the specified ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                var user = await ctx.GetUserByIdAsync(userId);
                return user == null
                    ? await Task.FromResult<UserDto>(null)
                    : MapUser(user);
            }
        }

        /// <summary>
        /// Gets the user with the specified name
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserDto> GetUserByNameAsync(string userName)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                var user = await ctx.GetUserByUserNameAsync(userName);
                return user == null
                    ? await Task.FromResult<UserDto>(null)
                    : MapUser(user);
            }
        }

        /// <summary>
        /// Gets the user with the email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                var user = await ctx.GetUserByEmailAsync(email);
                return user == null
                    ? await Task.FromResult<UserDto>(null)
                    : MapUser(user);
            }
        }

        /// <summary>
        /// Inserts the specified user into the user database
        /// </summary>
        /// <param name="user">User information</param>
        public async Task InsertUserAsync(UserDto user)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISubscriptionDataOperations>())
            {
                await ctx.InsertUserAsync(MapUser(user));
            }
        }

        /// <summary>
        /// Updates the specified user in the user database
        /// </summary>
        /// <param name="user">User information</param>
        public async Task UpdateUserAsync(UserDto user)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISubscriptionDataOperations>())
            {
                await ctx.UpdateUserAsync(MapUser(user));
            }
        }

        /// <summary>
        /// Create a new subscription with the specified data
        /// </summary>
        /// <param name="subscription">Subscription object</param>
        /// <param name="userId">User owning the subscription</param>
        /// <returns>The ID of the new subscription</returns>
        public async Task<int> CreateSubscriptionAsync(SubscriptionDto subscription, string userId)
        {
            Verify.NotNull(subscription, "subscription");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateContext<ISubscriptionDataOperations>())
            {
                var userIdGuid = new Guid(userId);
                var userRecord = await ctx.GetUserByIdAsync(userIdGuid);
                if (userRecord == null)
                {
                    throw new InvalidOperationException(
                        string.Format("User with ID {0} have not been found in the database",userId));
                }
                await ctx.BeginTransactionAsync();
                var subscriptionRecord = MapSubscription(subscription);
                await ctx.InsertSubscriptionAsync(subscriptionRecord);
                await ctx.InsertSubscriptionOwnerAsync(new SubscriptionOwnerRecord
                {
                    SubscriptionId = subscriptionRecord.Id,
                    UserId = userIdGuid,
                });
                userRecord.SubscriptionId = subscriptionRecord.Id;
                userRecord.LastModifiedUtc = DateTimeOffset.UtcNow;
                await ctx.UpdateUserAsync(userRecord);
                await ctx.CompleteAsync();
                return subscriptionRecord.Id;
            }
        }

        /// <summary>
        /// Gets user information by the email address provided
        /// </summary>
        /// <param name="userEmail">Email of the user (used as unique ID)</param>
        /// <returns>User token</returns>
        public async Task<UserTokenDto> GetUserTokenAsync(string userEmail)
        {
            Verify.NotNullOrEmpty(userEmail, "userEmail");
            Verify.RaiseWhenFailed();
            
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                var userRecord = await ctx.GetUserByEmailAsync(userEmail);
                if (userRecord == null)
                {
                    throw new InvalidOperationException(
                        string.Format("User with email '{0}' have not been found in the database", userEmail));
                }
                var owner = await ctx.GetSubscriptionOwnerByIdAsync(userRecord.SubscriptionId ?? -1, userRecord.Id);

                // TODO: collect role information
                return new UserTokenDto
                {
                    UserId = userRecord.Id,
                    UserName = userRecord.UserName,
                    SubscriptionId = userRecord.SubscriptionId,
                    IsSubscriptionOwner = owner != null,
                    ServiceRoles = new List<ServiceRolesDto>()
                };
            }
        }

        /// <summary>
        /// Gets user information by the user ID provided
        /// </summary>
        /// <param name="userId">User ID within the system</param>
        /// <returns>User token</returns>
        public async Task<UserTokenDto> GetUserTokenByIdAsync(Guid userId)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                var userRecord = await ctx.GetUserByIdAsync(userId);
                if (userRecord == null)
                {
                    throw new InvalidOperationException(
                        string.Format("User with ID '{0}' have not been found in the database", userId));
                }
                var owner = await ctx.GetSubscriptionOwnerByIdAsync(userRecord.SubscriptionId ?? -1, userRecord.Id);

                // TODO: collect role information
                return new UserTokenDto
                {
                    UserId = userRecord.Id,
                    UserName = userRecord.UserName,
                    SubscriptionId = userRecord.SubscriptionId,
                    IsSubscriptionOwner = owner != null,
                    ServiceRoles = new List<ServiceRolesDto>()
                };
            }
        }

        /// <summary>
        /// Gets user information by its provider data
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        /// <returns>User token</returns>
        public async Task<UserTokenDto> GetUserTokenByProviderDataAsync(string provider, string providerData)
        {
            Verify.NotNullOrEmpty(provider, "provider");
            Verify.NotNullOrEmpty(providerData, "providerData");
            Verify.RaiseWhenFailed();

            var user = await GetUserByProviderDataAsync(provider, providerData);
            if (user == null)
            {
                throw new InvalidOperationException(
                    string.Format("User with provider data '{0}, {1}' have not been found in the database", 
                    provider, providerData));
            }
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                var owner = await ctx.GetSubscriptionOwnerByIdAsync(user.SubscriptionId ?? -1, user.Id);

                // TODO: collect role information
                return new UserTokenDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    SubscriptionId = user.SubscriptionId,
                    IsSubscriptionOwner = owner != null,
                    ServiceRoles = new List<ServiceRolesDto>()
                };
            }
        }

        /// <summary>
        /// Gets the user information by its provider data
        /// </summary>
        /// <param name="provider">Provider ID</param>
        /// <param name="providerData">Provider data</param>
        /// <returns>The user information, if found; otherwise, null</returns>
        public async Task<UserDto> GetUserByProviderDataAsync(string provider, string providerData)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                var account = await ctx.GetUserAccountByProviderAsync(provider, providerData);
                if (account == null) return null;
                var user = await ctx.GetUserByIdAsync(account.UserId);
                return user == null ? null : MapUser(user);
            }
        }

        /// <summary>
        /// Inserts a new user account into the database
        /// </summary>
        /// <param name="account">Account information</param>
        /// <returns></returns>
        public async Task InsertUserAccountAsync(UserAccountDto account)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISubscriptionDataOperations>())
            {
                await ctx.InsertUserAccountAsync(new UserAccountRecord
                {
                    UserId = account.UserId,
                    Provider = account.Provider,
                    ProviderData = account.ProviderData
                });
            }
        }

        /// <summary>
        /// Removes the specified user account from the database
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="provider">Account provider</param>
        public async Task RemoveUserAccountAsync(Guid userId, string provider)
        {
            using (var ctx = DataAccessFactory.CreateContext<ISubscriptionDataOperations>())
            {
                await ctx.DeleteUserAccountAsync(userId, provider);
            }
        }

        /// <summary>
        /// Gets the login account belonging to a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Login accounts</returns>
        public async Task<List<UserAccountDto>> GetUserAccountsByUserId(Guid userId)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<ISubscriptionDataOperations>())
            {
                return (await ctx.GetUserAccountsByUserIdAsync(userId))
                    .Select(MapUserAccount).ToList();
            }
        }

        /// <summary>
        /// Sends an invitation to the specified user
        /// </summary>
        /// <param name="userInfo">Information about the invited user</param>
        public async Task InviteUserAsync(InviteUserDto userInfo)
        {
            Verify
                .NotNull(userInfo, "userInfo")
                .NotNullOrEmpty(userInfo.InvitedEmail, "InvitedEmail")
                .IsEmail(userInfo.InvitedEmail, "InvitedEmail")
                .NotNullOrEmpty(userInfo.InvitedUserName, "InvitedUserName")
                .RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateContext<ISubscriptionDataOperations>())
            {
                var email = await ctx.GetUserByEmailAsync(userInfo.InvitedEmail);
                if (email != null)
                {
                    throw new EmailReservedException(userInfo.InvitedEmail);
                }

                var invitationCode = String.Format("{0:N}{1:N}{2:N}", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
                await ctx.InsertUserInvitationAsync(new UserInvitationRecord
                {
                    InvitedUserName = userInfo.InvitedUserName,
                    InvitedEmail = userInfo.InvitedEmail,
                    InvitationCode = invitationCode,
                    CreatedUtc = DateTimeOffset.UtcNow,
                    ExpirationDateUtc = DateTimeOffset.UtcNow.AddHours(72),
                    SubscriptionId = Principal.SubscriptionId,
                    UserId = Principal.UserId,
                    LastModifiedUtc = null,
                    State = UserInvitationState.SENT,
                    Type = UserInvitationType.USER
                });
            }
        }

        /// <summary>
        /// Maps the specified UserRecord to a UserDto instance
        /// </summary>
        /// <param name="record">UserRecord to map</param>
        /// <returns>UserDto instance</returns>
        public static UserDto MapUser(UserRecord record)
        {
            return new UserDto
            {
                Id = record.Id,
                SubscriptionId = record.SubscriptionId,
                UserName = record.UserName,
                Email = record.Email,
                SecurityStamp = record.SecurityStamp,
                PasswordHash = record.PasswordHash,
                LastFailedAuthUtc = record.LastFailedAuthUtc,
                AccessFailedCount = record.AccessFailedCount,
                LockedOut = record.LockedOut,
                OwnerSuspend = record.OwnerSuspend,
                PasswordResetSuspend = record.PasswordResetSuspend,
                CreatedUtc = record.CreatedUtc,
                LastModifiedUtc = record.LastModifiedUtc
            };
        }

        /// <summary>
        /// Maps the specified UserDto to a UserRecord instance
        /// </summary>
        /// <param name="dto">UserDto to map</param>
        /// <returns>UserRecord instance</returns>
        public static UserRecord MapUser(UserDto dto)
        {
            return new UserRecord
            {
                Id = dto.Id,
                SubscriptionId = dto.SubscriptionId,
                UserName = dto.UserName,
                Email = dto.Email,
                SecurityStamp = dto.SecurityStamp,
                PasswordHash = dto.PasswordHash,
                LastFailedAuthUtc = dto.LastFailedAuthUtc,
                AccessFailedCount = dto.AccessFailedCount,
                LockedOut = dto.LockedOut,
                OwnerSuspend = dto.OwnerSuspend,
                PasswordResetSuspend = dto.PasswordResetSuspend,
                CreatedUtc = dto.CreatedUtc,
                LastModifiedUtc = dto.LastModifiedUtc
            };
        }

        /// <summary>
        /// Maps the specified SubscriptionRecord to a SubscriptionDto instance
        /// </summary>
        /// <param name="record">SubscriptionRecord to map</param>
        /// <returns>SubscriptionDto instance</returns>
        public static SubscriptionDto MapSubscription(SubscriptionRecord record)
        {
            return new SubscriptionDto
            {
                Id = record.Id,
                SubscriberName = record.SubscriberName,
                PrimaryEmail = record.PrimaryEmail,
                PrimaryPhone = record.PrimaryPhone,
                AddrCountry = record.AddrCountry,
                AddrZip = record.AddrZip,
                AddrTown = record.AddrTown,
                AddrLine1 = record.AddrLine1,
                AddrLine2 = record.AddrLine2,
                AddrState = record.AddrState,
                TaxId = record.TaxId,
                BankAccountNo = record.BankAccountNo,
                CreatedUtc = record.CreatedUtc,
                LastModifiedUtc = record.LastModifiedUtc
            };
        }

        /// <summary>
        /// Maps the specified SubscriptionDto to a SubscriptionRecord instance
        /// </summary>
        /// <param name="dto">SubscriptionDto to map</param>
        /// <returns>SubscriptionRecord instance</returns>
        public static SubscriptionRecord MapSubscription(SubscriptionDto dto)
        {
            return new SubscriptionRecord
            {
                Id = dto.Id,
                SubscriberName = dto.SubscriberName,
                PrimaryEmail = dto.PrimaryEmail,
                PrimaryPhone = dto.PrimaryPhone,
                AddrCountry = dto.AddrCountry,
                AddrZip = dto.AddrZip,
                AddrTown = dto.AddrTown,
                AddrLine1 = dto.AddrLine1,
                AddrLine2 = dto.AddrLine2,
                AddrState = dto.AddrState,
                TaxId = dto.TaxId,
                BankAccountNo = dto.BankAccountNo,
                CreatedUtc = dto.CreatedUtc,
                LastModifiedUtc = dto.LastModifiedUtc
            };
        }

        /// <summary>
        /// Maps the specified UserAccountRecord to a UserAccountDto instance
        /// </summary>
        /// <param name="record">UserAccountRecord to map</param>
        /// <returns>UserAccountDto instance</returns>
        public static UserAccountDto MapUserAccount(UserAccountRecord record)
        {
            return new UserAccountDto
            {
                UserId = record.UserId,
                Provider = record.Provider,
                ProviderData = record.ProviderData
            };
        }

        /// <summary>
        /// Maps the specified UserAccountDto to a UserAccountRecord instance
        /// </summary>
        /// <param name="dto">UserAccountDto to map</param>
        /// <returns>UserAccountRecord instance</returns>
        public static UserAccountRecord MapUserAccount(UserAccountDto dto)
        {
            return new UserAccountRecord
            {
                UserId = dto.UserId,
                Provider = dto.Provider,
                ProviderData = dto.ProviderData
            };
        }
    }
}