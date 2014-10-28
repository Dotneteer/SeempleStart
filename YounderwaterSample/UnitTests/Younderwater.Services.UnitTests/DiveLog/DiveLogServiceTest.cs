using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects.Validation;
using SeemplestBlocks.Core.Security;
using SeemplestBlocks.Core.ServiceInfrastructure;
using SoftwareApproach.TestingExtensions;
using Younderwater.Dto.DiveLog;
using Younderwater.Services.DiveLog;
using Younderwater.Services.DiveLog.DataAccess;
using Younderwater.Services.DiveLog.Exceptions;
using Younderwater.Services.UnitTests.Helpers;

namespace Younderwater.Services.UnitTests.DiveLog
{
    [TestClass]
    public class DiveLogServiceTest
    {
        private const string DB_CONN = "connStr=YW";

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<IDiveLogDataAccessOperations, DiveLogDataAccessOperations>(DB_CONN);
            ServiceManager.Register<IDiveLogService, DiveLogService>();
            ServiceManager.Register<IUserIdContextProvider, TestUserIdContextProvider>();
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            SqlScriptHelper.RunScript("InitDiveLog.sql");
        }

        #region GetAllDivesOfUserAsync tesztesetek

        [TestMethod]
        public async Task GetAllDivesForUserWorksAsExpected()
        {
            // --- Arrange
            const int DIVES_IN_INIT_SCRIPT_FOR_TEST_USER = 9;
            var service = HttpServiceFactory.CreateService<IDiveLogService>();

            // --- Act
            var dives = await service.GetAllDivesOfUserAsync();

            // --- Assert
            dives.ShouldHaveCountOf(DIVES_IN_INIT_SCRIPT_FOR_TEST_USER);
        }

        #endregion

        #region GetDiveByIdAsync tesztesetek

        [TestMethod]
        public async Task GetDiveByIdWorksAsExpected()
        {
            // --- Arrange
            const int EXISTING_DIVE_ID = 1;
            var service = HttpServiceFactory.CreateService<IDiveLogService>();

            // --- Act
            var existingDive = await service.GetDiveByIdAsync(EXISTING_DIVE_ID);

            // --- Assert
            existingDive.ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(DiveNotFoundException))]
        public async Task GetDiveByIdFailsWithNonExistingId()
        {
            // --- Arrange
            const int NON_EXISTING_DIVE_ID = -1;
            var service = HttpServiceFactory.CreateService<IDiveLogService>();

            // --- Act
            await service.GetDiveByIdAsync(NON_EXISTING_DIVE_ID);
        }

        #endregion

        #region RegisterDiveLogEntryAsync tesztesetek

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithNullRequest()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithNullDiveSite()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = null,
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.DiveSite");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithEmptyDiveSite()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.DiveSite");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithNullLocation()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = null,
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.Location");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithEmptyLocation()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "",
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.Location");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithTooLowMaxDepth()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = -1,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.MaxDepth");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithTooHighMaxDepth()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 301,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.MaxDepth");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithTooLowBottomTime()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = -1
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.BottomTime");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithTooHighBottomTime()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 481
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.BottomTime");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryFailsWithPresetId()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Id = 123,
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.RegisterDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.Id");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.VALIDATION_FAILS);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task RegisterDiveLogEntryWorksAsExpected()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54,
                Comment = "It was a great dive"
            };

            // --- Act
            var id = await service.RegisterDiveLogEntryAsync(dive);

            // --- Assert
            var diveback = await service.GetDiveByIdAsync(id);
            diveback.Id.ShouldBeGreaterThan(0);
            diveback.Date.ShouldEqual(dive.Date);
            diveback.DiveSite.ShouldEqual(dive.DiveSite);
            diveback.Location.ShouldEqual(dive.Location);
            diveback.MaxDepth.ShouldEqual(dive.MaxDepth);
            diveback.BottomTime.ShouldEqual(dive.BottomTime);
            diveback.Comment.ShouldEqual(dive.Comment);
        }

        #endregion

        #region ModifyDiveLogEntryAsync tesztesetek

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithNullRequest()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(null);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithNullDiveSite()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = null,
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.DiveSite");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithEmptyDiveSite()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.DiveSite");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithNullLocation()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = null,
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.Location");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithEmptyLocation()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "",
                MaxDepth = 22.6M,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.Location");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithTooLowMaxDepth()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = -1,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.MaxDepth");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithTooHighMaxDepth()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 301,
                BottomTime = 54
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.MaxDepth");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithTooLowBottomTime()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = -1
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.BottomTime");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryFailsWithTooHighBottomTime()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 481
            };

            // --- Act
            try
            {
                await service.ModifyDiveLogEntryAsync(dive);
            }
            catch (ArgumentValidationException ex)
            {
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("dive.BottomTime");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.OUT_OF_RANGE);
                return;
            }
            Assert.Fail("ArgumentValidationException expected.");
        }

        [TestMethod]
        public async Task ModifyDiveLogEntryWorksAsExpected()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54,
                Comment = "It was a great dive"
            };
            var id = await service.RegisterDiveLogEntryAsync(dive);

            // --- Act
            dive.Id = id;
            dive.BottomTime = 102;
            dive.Comment = "It was a long dive";
            await service.ModifyDiveLogEntryAsync(dive);

            // --- Assert
            var diveback = await service.GetDiveByIdAsync(id);
            diveback.Id.ShouldBeGreaterThan(0);
            diveback.Date.ShouldEqual(dive.Date);
            diveback.DiveSite.ShouldEqual(dive.DiveSite);
            diveback.Location.ShouldEqual(dive.Location);
            diveback.MaxDepth.ShouldEqual(dive.MaxDepth);
            diveback.BottomTime.ShouldEqual(dive.BottomTime);
            diveback.Comment.ShouldEqual(dive.Comment);
        }

        [TestMethod]
        [ExpectedException(typeof(DiveNotFoundException))]
        public async Task ModifyDiveLogEntryFailsWithUnknownDive()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Id = -1,
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54,
                Comment = "It was a great dive"
            };

            // --- Act
            await service.ModifyDiveLogEntryAsync(dive);
        }

        [TestMethod]
        [ExpectedException(typeof(NoPermissionToDiveLogException))]
        public async Task ModifyDiveLogEntryFailsWithAnotherUser()
        {
            // --- Arrange
            const int ANOTHER_USERS_DIVE_ID = 10;
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Id = ANOTHER_USERS_DIVE_ID,
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54,
                Comment = "It was a great dive"
            };

            // --- Act
            await service.ModifyDiveLogEntryAsync(dive);
        }

        #endregion

        #region RemoveDiveLogEntryAsync tesztesetek

        [TestMethod]
        [ExpectedException(typeof(DiveNotFoundException))]
        public async Task RemoveDiveLogEntryFailsWithUnknownDive()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();

            // --- Act
            await service.RemoveDiveLogEntryAsync(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(NoPermissionToDiveLogException))]
        public async Task RemoveDiveLogEntryFailsWithAnotherUser()
        {
            // --- Arrange
            const int ANOTHER_USERS_DIVE_ID = 10;
            var service = HttpServiceFactory.CreateService<IDiveLogService>();

            // --- Act
            await service.RemoveDiveLogEntryAsync(ANOTHER_USERS_DIVE_ID);
        }

        [TestMethod]
        public async Task RemoveDiveLogEntryWorksAsExpected()
        {
            // --- Arrange
            var service = HttpServiceFactory.CreateService<IDiveLogService>();
            var dive = new DiveLogEntryDto
            {
                Date = new DateTime(2008, 6, 7),
                DiveSite = "Gotta Abu Ramada",
                Location = "Hurghada, Egypt",
                MaxDepth = 22.6M,
                BottomTime = 54,
                Comment = "It was a great dive"
            };
            var id = await service.RegisterDiveLogEntryAsync(dive);

            // --- Act
            var diveBack1 = await service.GetDiveByIdAsync(id);
            await service.RemoveDiveLogEntryAsync(id);
            var notFound = false;
            try
            {
                await service.GetDiveByIdAsync(id);
            }
            catch (DiveNotFoundException)
            {
                notFound = true;
            }

            // --- Assert
            diveBack1.ShouldNotBeNull();
            notFound.ShouldBeTrue();
        }


        #endregion

    }
}
