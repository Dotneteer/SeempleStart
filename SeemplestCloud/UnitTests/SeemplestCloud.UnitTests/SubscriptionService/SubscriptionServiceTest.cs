using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects.Validation;
using SeemplestBlocks.Core.AppConfig;
using SeemplestBlocks.Core.Email;
using SeemplestBlocks.Dto.Email;
using SeemplestCloud.Dto.Subscription;
using SeemplestCloud.Services.Infrastructure;
using SeemplestCloud.Services.SubscriptionService;
using SeemplestCloud.Services.SubscriptionService.DataAccess;
using SeemplestCloud.Services.SubscriptionService.Exceptions;
using SeemplestCloud.UnitTests.Helpers;
using SoftwareApproach.TestingExtensions;

namespace SeemplestCloud.UnitTests.SubscriptionService
{
    [TestClass]
    public class SubscriptionServiceTest
    {
        private const string DB_CONN = "connStr=Sc";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<ISubscriptionDataOperations, SubscriptionDataOperations>(DB_CONN);
            ServiceManager.Register<ISubscriptionService, Services.SubscriptionService.SubscriptionService>();
            ServiceManager.Register<IAppPrincipalProvider, TestAppPrincipalProvider>();
            ServiceManager.Register<IEmailSender, TestEmailSender>();
            ServiceManager.Register<IConfigurationReader, FakeConfigurationReader>();
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            SqlScriptHelper.RunScript("InitSubscriptions.sql");
        }

        #region GetUserByIdAsyncTests

        [TestMethod]
        public async Task GetUserByIdAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            // --- Act
            var user1 = await srv.GetUserByIdAsync(newUser.Id);
            var user2 = await srv.GetUserByIdAsync(Guid.NewGuid());

            // --- Assert
            user1.ShouldNotBeNull();
            user2.ShouldBeNull();
        }

        #endregion

        #region GetUserByNameAsync test cases

        [TestMethod]
        public async Task GetUserByNameAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            // --- Act
            var user1 = await srv.GetUserByNameAsync(null, newUser.UserName);
            var user2 = await srv.GetUserByNameAsync(null, "dummy");

            // --- Assert
            user1.ShouldNotBeNull();
            user2.ShouldBeNull();
        }

        [TestMethod]
        public async Task GetUserByNameAsyncFailsWithNullUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserByNameAsync(null, null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task GetUserByNameAsyncFailsWithEmptyUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserByNameAsync(null, "  ");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region GetUserByEmailAsync test cases

        [TestMethod]
        public async Task GetUserByEmailAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            // --- Act
            var user1 = await srv.GetUserByEmailAsync(newUser.Email);
            var user2 = await srv.GetUserByEmailAsync("dummy");

            // --- Assert
            user1.ShouldNotBeNull();
            user2.ShouldBeNull();
        }

        [TestMethod]
        public async Task GetUserByEmailAsyncFailsWithNullUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserByEmailAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("email");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task GetUserByEmailAsyncFailsWithEmptyUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserByEmailAsync("  ");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("email");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region InsertUserAsync test cases

        [TestMethod]
        public async Task InsertUserAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            await srv.InsertUserAsync(newUser);

            // --- Assert
            var user = await srv.GetUserByIdAsync(newUser.Id);
            user.ShouldNotBeNull();
            user.SubscriptionId.ShouldBeNull();
            user.UserName.ShouldEqual(newUser.UserName);
            user.Email.ShouldEqual(newUser.Email);
            user.SecurityStamp.ShouldEqual(newUser.SecurityStamp);
        }

        [TestMethod]
        public async Task InsertUserAsyncFailsWithNullRequest()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.InsertUserAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAsyncFailsWithNullEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = null,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.InsertUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.Email");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAsyncFailsWithInvalidEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "email",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.InsertUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.INVALID_EMAIL);
                ex.Notifications.Items[0].Target.ShouldEqual("user.Email");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAsyncFailsWithNullUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = null,
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.InsertUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.UserName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAsyncFailsWithEmptyUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "   ",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.InsertUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.UserName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAsyncFailsWithNullSecurityStamp()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = null,
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.InsertUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.SecurityStamp");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAsyncFailsWithEmptySecurityStamp()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = "   ",
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.InsertUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.SecurityStamp");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region UpdateUserAsync test cases

        [TestMethod]
        public async Task UpdateUserAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);
            newUser.UserName = "dotneteer1";

            // --- Act
            await srv.UpdateUserAsync(newUser);

            // --- Assert
            var user = await srv.GetUserByIdAsync(newUser.Id);
            user.ShouldNotBeNull();
            user.SubscriptionId.ShouldBeNull();
            user.UserName.ShouldEqual(newUser.UserName);
            user.Email.ShouldEqual(newUser.Email);
            user.SecurityStamp.ShouldEqual(newUser.SecurityStamp);
        }

        [TestMethod]
        public async Task UpdateUserAsyncFailsWithNullRequest()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.UpdateUserAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task UpdateUserAsyncFailsWithNullEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = null,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.UpdateUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.Email");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task UpdateUserAsyncFailsWithInvalidEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "email",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.UpdateUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.INVALID_EMAIL);
                ex.Notifications.Items[0].Target.ShouldEqual("user.Email");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task UpdateUserAsyncFailsWithNullUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = null,
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.UpdateUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.UserName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task UpdateUserAsyncFailsWithEmptyUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "   ",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.UpdateUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.UserName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task UpdateUserAsyncFailsWithNullSecurityStamp()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = null,
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.UpdateUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.SecurityStamp");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task UpdateUserAsyncFailsWithEmptySecurityStamp()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = "   ",
                CreatedUtc = DateTimeOffset.UtcNow
            };

            // --- Act
            try
            {
                await srv.UpdateUserAsync(newUser);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("user.SecurityStamp");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region CreateSubscriptionAsync test cases

        [TestMethod]
        public async Task CreateSubscriptionAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };

            // --- Act
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));

            // --- Assert
            var user = await srv.GetUserByIdAsync(newUser.Id);
            user.ShouldNotBeNull();
            user.SubscriptionId.ShouldEqual(subscrId);
            user.UserName.ShouldEqual(newUser.UserName);
            user.Email.ShouldEqual(newUser.Email);
            user.SecurityStamp.ShouldEqual(newUser.SecurityStamp);
        }

        [TestMethod]
        public async Task CreateSubscriptionAsyncFailsWithNullRequest()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.CreateSubscriptionAsync(null, "userId");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("subscription");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task CreateSubscriptionAsyncFailsWithNullUserId()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };

            // --- Act
            try
            {
                await srv.CreateSubscriptionAsync(newSubscr, null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userId");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task CreateSubscriptionAsyncFailsWithEmptyUserId()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };

            // --- Act
            try
            {
                await srv.CreateSubscriptionAsync(newSubscr, "   ");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userId");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task CreateSubscriptionAsyncFailsWithNullSubscriberName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newSubscr = new SubscriptionDto
            {
                SubscriberName = null,
                PrimaryEmail = "vsxguy@gmail.com"
            };

            // --- Act
            try
            {
                await srv.CreateSubscriptionAsync(newSubscr, "id");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("subscription.SubscriberName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task CreateSubscriptionAsyncFailsWithEmptySubscriberName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "   ",
                PrimaryEmail = "vsxguy@gmail.com"
            };

            // --- Act
            try
            {
                await srv.CreateSubscriptionAsync(newSubscr, "id");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("subscription.SubscriberName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task CreateSubscriptionAsyncFailsWithNullEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = null
            };

            // --- Act
            try
            {
                await srv.CreateSubscriptionAsync(newSubscr, "id");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("subscription.PrimaryEmail");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task CreateSubscriptionAsyncFailsWithInvalidEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "email"
            };

            // --- Act
            try
            {
                await srv.CreateSubscriptionAsync(newSubscr, "id");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.INVALID_EMAIL);
                ex.Notifications.Items[0].Target.ShouldEqual("subscription.PrimaryEmail");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region GetUserTokenByEmailAsync test cases

        [TestMethod]
        public async Task GetUserTokenByEmailAsyncWorkAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));

            // --- Act
            var token = await srv.GetUserTokenByEmailAsync("dotneteer@hotmail.com");

            // --- Assert
            token.ShouldNotBeNull();
            token.UserId.ShouldEqual(newUser.Id);
            token.UserName.ShouldEqual("dotneteer");
            token.IsServiceUser.ShouldBeFalse();
            token.IsSubscriptionOwner.ShouldBeTrue();
            token.SubscriptionId.ShouldEqual(subscrId);
        }

        [TestMethod]
        public async Task GetUserTokenByEmailAsyncFailsWithNullEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserTokenByEmailAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userEmail");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task GetUserTokenByEmailAsyncFailsWithEmptyEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserTokenByEmailAsync("  ");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userEmail");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownEmailException))]
        public async Task GetUserTokenByEmailAsyncFailsWithUnknownEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            await srv.GetUserTokenByEmailAsync("email");
        }

        #endregion

        #region GetUserTokenById test cases

        [TestMethod]
        public async Task GetUserTokenByIdAsyncWorkAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));

            // --- Act
            var token = await srv.GetUserTokenByIdAsync(newUser.Id);

            // --- Assert
            token.ShouldNotBeNull();
            token.UserId.ShouldEqual(newUser.Id);
            token.UserName.ShouldEqual("dotneteer");
            token.IsServiceUser.ShouldBeFalse();
            token.IsSubscriptionOwner.ShouldBeTrue();
            token.SubscriptionId.ShouldEqual(subscrId);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownUserIdException))]
        public async Task GetUserTokenByEmailAsyncFailsWithUnknownUserId()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            await srv.GetUserTokenByIdAsync(Guid.NewGuid());
        }

        #endregion

        #region GetUserTokenByProviderDataAsync test cases

        [TestMethod]
        public async Task GetUserTokenByProviderDataAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);
            var newAccount = new UserAccountDto
            {
                UserId = newUser.Id,
                Provider = "Facebook",
                ProviderData = "1111"
            };
            await srv.InsertUserAccountAsync(newAccount);

            // --- Act
            var token1 = await srv.GetUserTokenByProviderDataAsync("Facebook", "1111");

            // --- Assert
            token1.ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownProviderDataException))]
        public async Task GetUserTokenByProviderDataAsyncFailsWithUnknownAccount()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            await srv.GetUserTokenByProviderDataAsync("Facebook", "dummy");
        }

        [TestMethod]
        public async Task GetUserTokenByProviderDataAsyncFailsWithNullProvider()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserTokenByProviderDataAsync(null, "dummy");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("provider");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task GetUserTokenByProviderDataAsyncFailsWithEmptyProvider()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserTokenByProviderDataAsync("   ", "dummy");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("provider");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task GetUserTokenByProviderDataAsyncFailsWithNullProviderData()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserTokenByProviderDataAsync("Facebook", null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("providerData");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task GetUserTokenByProviderDataAsyncFailsWithEmptyProviderData()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.GetUserTokenByProviderDataAsync("Facebook", "   ");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("providerData");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region GetUserByProviderDataAsync test cases

        #endregion

        #region InsertUserAccountAsync test cases

        [TestMethod]
        public async Task InsertUserAccountAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);
            var newAccount = new UserAccountDto
            {
                UserId = newUser.Id,
                Provider = "Facebook",
                ProviderData = "1111"
            };

            // --- Act
            await srv.InsertUserAccountAsync(newAccount);

            // --- Assert
            var accounts = await srv.GetUserAccountsByUserIdAsync(newUser.Id);
            accounts.ShouldHaveCountOf(1);
            accounts[0].Provider.ShouldEqual("Facebook");
            accounts[0].ProviderData.ShouldEqual("1111");
        }

        [TestMethod]
        public async Task InsertUserAccountAsyncFailsWithNullAccount()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.InsertUserAccountAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("account");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAccountAsyncFailsWithNullProvider()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newAccount = new UserAccountDto
            {
                UserId = Guid.NewGuid(),
                Provider = null,
                ProviderData = "1111"
            };


            // --- Act
            try
            {
                await srv.InsertUserAccountAsync(newAccount);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("account.Provider");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAccountAsyncFailsWithEmptyProvider()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newAccount = new UserAccountDto
            {
                UserId = Guid.NewGuid(),
                Provider = "   ",
                ProviderData = "1111"
            };


            // --- Act
            try
            {
                await srv.InsertUserAccountAsync(newAccount);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("account.Provider");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAccountAsyncFailsWithNullProviderData()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newAccount = new UserAccountDto
            {
                UserId = Guid.NewGuid(),
                Provider = "Facebook",
                ProviderData = null
            };


            // --- Act
            try
            {
                await srv.InsertUserAccountAsync(newAccount);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("account.ProviderData");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InsertUserAccountAsyncFailsWithEmptyProviderData()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newAccount = new UserAccountDto
            {
                UserId = Guid.NewGuid(),
                Provider = "Facebook",
                ProviderData = "   "
            };


            // --- Act
            try
            {
                await srv.InsertUserAccountAsync(newAccount);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("account.ProviderData");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region RemoveUserAccountAsync test cases

        [TestMethod]
        public async Task RemoveUserAccountAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);
            var newAccount = new UserAccountDto
            {
                UserId = newUser.Id,
                Provider = "Facebook",
                ProviderData = "1111"
            };
            await srv.InsertUserAccountAsync(newAccount);
            var accounts1 = await srv.GetUserAccountsByUserIdAsync(newUser.Id);

            // --- Act
            await srv.RemoveUserAccountAsync(newUser.Id, "Facebook");
            
            // --- Assert
            var accounts2 = await srv.GetUserAccountsByUserIdAsync(newUser.Id);
            accounts1.ShouldHaveCountOf(1);
            accounts2.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public async Task RemoveUserAccountAsyncFailsWithNullProvider()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.RemoveUserAccountAsync(Guid.NewGuid(), null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("provider");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RemoveUserAccountAsyncFailsWithEmptyProvider()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.RemoveUserAccountAsync(Guid.NewGuid(), "  ");
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("provider");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        #endregion

        #region GetUserAccountsByUserIdAsync test cases

        [TestMethod]
        public async Task GetUserAccountsAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);
            var newAccount = new UserAccountDto
            {
                UserId = newUser.Id,
                Provider = "Facebook",
                ProviderData = "1111"
            };
            await srv.InsertUserAccountAsync(newAccount);
            newAccount = new UserAccountDto
            {
                UserId = newUser.Id,
                Provider = "Google",
                ProviderData = "2222"
            };
            await srv.InsertUserAccountAsync(newAccount);

            // --- Act
            var accounts = await srv.GetUserAccountsByUserIdAsync(newUser.Id);

            // --- Assert
            accounts.ShouldHaveCountOf(2);
        }

        #endregion

        #region GetInvitedUsersAsync test cases

        #endregion

        #region InvitUsersAsync test cases

        [TestMethod]
        public async Task InviteUserAsyncWorksAsExpected()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));
            TestAppPrincipalProvider.SetPrincipal(new AppPrincipal(newUser.Id, newUser.UserName, false, subscrId, false, null));

            // --- Act
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "vsxguy@gmail.com",
                InvitedUserName = "vsxguy"
            };
            await srv.InviteUserAsync(newInvitation);

            // --- Assert
            var invitations = await srv.GetInvitedUsersAsync();
            invitations.ShouldHaveCountOf(1);
        }

        [TestMethod]
        public async Task InviteUserAccountAsyncFailsWithNullUserInfo()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();

            // --- Act
            try
            {
                await srv.InviteUserAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userInfo");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InviteUserAccountAsyncFailsWithNullEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = null,
                InvitedUserName = "vsxguy"
            };

            // --- Act
            try
            {
                await srv.InviteUserAsync(newInvitation);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userInfo.InvitedEmail");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InviteUserAccountAsyncFailsWithInvalidEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "email",
                InvitedUserName = "vsxguy"
            };

            // --- Act
            try
            {
                await srv.InviteUserAsync(newInvitation);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.INVALID_EMAIL);
                ex.Notifications.Items[0].Target.ShouldEqual("userInfo.InvitedEmail");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InviteUserAccountAsyncFailsWithNullUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "email@company.hu",
                InvitedUserName = null
            };

            // --- Act
            try
            {
                await srv.InviteUserAsync(newInvitation);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userInfo.InvitedUserName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task InviteUserAccountAsyncFailsWithEmptyUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "email@company.hu",
                InvitedUserName = "   "
            };

            // --- Act
            try
            {
                await srv.InviteUserAsync(newInvitation);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                ex.Notifications.Items[0].Target.ShouldEqual("userInfo.InvitedUserName");
                return;
            }

            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        [ExpectedException(typeof(EmailReservedException))]
        public async Task InviteUserAsyncFailsWithReservedEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));
            TestAppPrincipalProvider.SetPrincipal(new AppPrincipal(newUser.Id, newUser.UserName, false, subscrId, false, null));

            // --- Act
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "dotneteer@hotmail.com",
                InvitedUserName = "vsxguy"
            };
            await srv.InviteUserAsync(newInvitation);
        }

        [TestMethod]
        [ExpectedException(typeof(EmailAlreadyInvitedException))]
        public async Task InviteUserAsyncFailsWithAlreadyInvitedEmail()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));
            TestAppPrincipalProvider.SetPrincipal(new AppPrincipal(newUser.Id, newUser.UserName, false, subscrId, false, null));
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "vsxguy@gmail.com",
                InvitedUserName = "vsxguy"
            };
            await srv.InviteUserAsync(newInvitation);

            // --- Act
            newInvitation = new InviteUserDto
            {
                InvitedEmail = "vsxguy@gmail.com",
                InvitedUserName = "vsxguy1"
            };
            await srv.InviteUserAsync(newInvitation);
        }

        [TestMethod]
        [ExpectedException(typeof(UserNameReservedException))]
        public async Task InviteUserAsyncFailsWithReservedUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));
            TestAppPrincipalProvider.SetPrincipal(new AppPrincipal(newUser.Id, newUser.UserName, false, subscrId, false, null));

            // --- Act
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "vsxguy@gmail.com",
                InvitedUserName = "dotneteer"
            };
            await srv.InviteUserAsync(newInvitation);
        }

        [TestMethod]
        [ExpectedException(typeof(UserAlreadyInvitedException))]
        public async Task InviteUserAsyncFailsWithAlreadyInvitedUserName()
        {
            // --- Arrange
            var srv = ServiceManager.GetService<ISubscriptionService>();
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                SubscriptionId = null,
                UserName = "dotneteer",
                Email = "dotneteer@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await srv.InsertUserAsync(newUser);

            var newSubscr = new SubscriptionDto
            {
                SubscriberName = "Mr. Novak",
                PrimaryEmail = "vsxguy@gmail.com"
            };
            var subscrId = await srv.CreateSubscriptionAsync(newSubscr, newUser.Id.ToString("N"));
            TestAppPrincipalProvider.SetPrincipal(new AppPrincipal(newUser.Id, newUser.UserName, false, subscrId, false, null));
            var newInvitation = new InviteUserDto
            {
                InvitedEmail = "vsxguy@gmail.com",
                InvitedUserName = "vsxguy"
            };
            await srv.InviteUserAsync(newInvitation);

            // --- Act
            newInvitation = new InviteUserDto
            {
                InvitedEmail = "vsxguy1@gmail.com",
                InvitedUserName = "vsxguy"
            };
            await srv.InviteUserAsync(newInvitation);
        }

        #endregion

        // ReSharper disable once ClassNeverInstantiated.Local
        class TestAppPrincipalProvider : IAppPrincipalProvider
        {
            private static AppPrincipal s_Principal;

            public static void SetPrincipal(AppPrincipal principal)
            {
                s_Principal = principal;
            }

            public AppPrincipal Principal
            {
                get { return s_Principal; }
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class TestEmailSender : IEmailSender
        {
            public void SendEmail(string fromAddr, string fromName, string[] toAddrs, string subject, string message,
                AppointmentDefinitionDto appointment = null)
            {
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class FakeConfigurationReader : IConfigurationReader
        {
            public bool GetConfigurationValue(string category, string key, out string value)
            {
                if (category == "SmtpConfig")
                {
                    switch (key)
                    {
                        case "Enabled":
                            value = "true";
                            return true;
                        case "ThrowException":
                            value = "false";
                            return true;
                        case "SmtpServer":
                            value = "smtp.gmail.com";
                            return true;
                        case "PortNumber":
                            value = "587";
                            return true;
                        case "UserName":
                            value = "seemplestcloud";
                            return true;
                        case "Password":
                            value = "@2014Seemplest";
                            return true;
                        case "UseSsl":
                            value = "true";
                            return true;
                        case "SmtpAuth":
                            value = "true";
                            return true;
                        case "MaxRetry":
                            value = "1";
                            return true;
                        case "RetryMinutes":
                            value = "5";
                            return true;
                        case "SendInterval":
                            value = "1000";
                            return true;
                        default:
                            value = null;
                            return false;
                    }
                } 
                if (category == "SubscriptionConfig")
                {
                    switch (key)
                    {
                        case "InvitationLinkPrefix":
                            value = "prefix";
                            return true;
                        default:
                            value = null;
                            return false;
                    }
                }
                value = null;
                return false;
            }
        }
    }
}
