using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects.Validation;
using SeemplestBlocks.Core.AppConfig;
using SeemplestBlocks.Core.AppConfig.DataAccess;
using SeemplestBlocks.Core.AppConfig.Exceptions;
using SeemplestBlocks.Dto.AppConfig;
using SeemplestBlocks.UnitTests.Helpers;
using SoftwareApproach.TestingExtensions;

namespace SeemplestBlocks.UnitTests.AppConfig
{
    [TestClass]
    public class ConfigurationServiceTest
    {
        private const string DB_CONN = "connStr=Core";

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<IConfigurationDataOperations, ConfigurationDataOperations>(DB_CONN);
            ServiceManager.Register<IConfigurationService, ConfigurationService>();
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            SqlScriptHelper.RunScript("InitLocales.sql");    
        }

        #region GetAllLocales tesztesetek

        [TestMethod]
        public void GetAllLocalesWorksAsExpected()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();
            
            // --- Act
            var locales = service.GetAllLocales();

            // --- Assert
            locales.ShouldHaveCountOf(6);
        }

        #endregion 

        #region IsLocaleCodeUnique tesztesetek

        [TestMethod]
        public void IsLocaleCodeUniqueFailsWithEmptyCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.IsLocaleCodeUnique("");
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void IsLocaleCodeUniqueFailsWithNullCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.IsLocaleCodeUnique(null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void IsLocaleCodeUniqueWorksAsExpected()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            var test1 = service.IsLocaleCodeUnique("def");
            var test2 = service.IsLocaleCodeUnique("fr-fr");

            // --- Assert
            test1.ShouldBeFalse();
            test2.ShouldBeTrue();
        }


        #endregion

        #region IsLocaleDisplayNameUnique tesztesetek

        [TestMethod]
        public void IsLocaleDisplayNameUniqueFailsWithEmptyCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.IsLocaleDisplayNameUnique("");
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("name");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void IsLocaleDisplayNameUniqueFailsWithNullCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.IsLocaleDisplayNameUnique(null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("name");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void IsLocaleDisplayNameUniqueWorksAsExpected()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            var test1 = service.IsLocaleDisplayNameUnique("Alapértelmezett nyelv");
            var test2 = service.IsLocaleDisplayNameUnique("Francia nyelvű erőforrások");

            // --- Assert
            test1.ShouldBeFalse();
            test2.ShouldBeTrue();
        }

        #endregion

        #region AddLocale tesztesetek

        [TestMethod]
        public void AddLocalFailsWithNullRequest()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.AddLocale(null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void AddLocaleFailsWithNullCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.AddLocale(new LocaleDto
                {
                    Code = null,
                    DisplayName = "Hello!"
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.Code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void AddLocaleFailsWithWrongCodeFormat()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.AddLocale(new LocaleDto
                {
                    Code = "  ",
                    DisplayName = "Hello!"
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.Code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NO_MATCH);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void AddLocaleFailsWithNullDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.AddLocale(new LocaleDto
                {
                    Code = "hu",
                    DisplayName = null
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.DisplayName");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void AddLocaleFailsWithEmptyDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.AddLocale(new LocaleDto
                {
                    Code = "hu",
                    DisplayName = ""
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.DisplayName");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void AddLocaleFailsWithToLongDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.AddLocale(new LocaleDto
                {
                    Code = "hu",
                    DisplayName = "".PadRight(1000, 'a')
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.DisplayName");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.TOO_LONG);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicatedLocaleCodeException))]
        public void AddLocaleFailsWithDuplicatedCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.AddLocale(new LocaleDto
            {
                Code = "hu",
                DisplayName = "Magyar"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicatedLocaleDisplayNameException))]
        public void AddLocaleFailsWithDuplicatedDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.AddLocale(new LocaleDto
            {
                Code = "fr",
                DisplayName = "Alapértelmezett magyar nyelvű beállítások"
            });
        }

        [TestMethod]
        public void AddLocaleWorksAsExpected()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.AddLocale(new LocaleDto
            {
                Code = "fr",
                DisplayName = "Francia"
            });

            // --- Assert
            var locales = service.GetAllLocales();
            locales.ShouldHaveCountOf(7);
            locales.First(l => l.Code == "fr").DisplayName.ShouldEqual("Francia");
        }

        #endregion

        #region ModifyLocale tesztesetek

        [TestMethod]
        public void ModifyLocalFailsWithNullRequest()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.ModifyLocale(null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void ModifyLocaleFailsWithNullCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.ModifyLocale(new LocaleDto
                {
                    Code = null,
                    DisplayName = "Hello!"
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.Code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void ModifyLocaleFailsWithWrongCodeFormat()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.ModifyLocale(new LocaleDto
                {
                    Code = "  ",
                    DisplayName = "Hello!"
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.Code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NO_MATCH);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void ModifyLocaleFailsWithNullDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.ModifyLocale(new LocaleDto
                {
                    Code = "hu",
                    DisplayName = null
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.DisplayName");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void ModifyLocaleFailsWithEmptyDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.ModifyLocale(new LocaleDto
                {
                    Code = "hu",
                    DisplayName = ""
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.DisplayName");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void ModifyLocaleFailsWithToLongDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.ModifyLocale(new LocaleDto
                {
                    Code = "hu",
                    DisplayName = "".PadRight(1000, 'a')
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale.DisplayName");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.TOO_LONG);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        [ExpectedException(typeof(LocaleNotFoundException))]
        public void ModifyLocaleFailsWithNonExistingCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.ModifyLocale(new LocaleDto
            {
                Code = "fr",
                DisplayName = "Módosított magyar erőforrások"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicatedLocaleDisplayNameException))]
        public void ModifyLocaleFailsWithDuplicatedDisplayName()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.ModifyLocale(new LocaleDto
            {
                Code = "hu",
                DisplayName = "Alapértelmezett nyelv"
            });
        }

        [TestMethod]
        public void ModifyLocaleWorksAsExpected()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.ModifyLocale(new LocaleDto
            {
                Code = "hu",
                DisplayName = "Módosított magyar erőforrások"
            });

            // --- Assert
            var locales = service.GetAllLocales();
            locales.First(l => l.Code == "hu").DisplayName.ShouldEqual("Módosított magyar erőforrások");
        }

        #endregion

        #region RemoveLocale tesztesetek

        [TestMethod]
        public void RemoveLocaleFailsWithNullCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.RemoveLocale(null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void RemoveLocaleFailsWithEmptyCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.RemoveLocale("");
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("code");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        [ExpectedException(typeof(LocaleNotFoundException))]
        public void RemoveLocaleFailsWithUnknownLocale()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.RemoveLocale("fr");
        }

        [TestMethod]
        public void RemoveLocaleWorksAsExpected()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitLocalizedResources.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            var huMessagesBefore = service.GetLocalizedResourcesByCategory("hu", "Message");
            var huErrorsBefore = service.GetLocalizedResourcesByCategory("hu", "Error");
            service.RemoveLocale("hu");
            var huMessagesAfter = service.GetLocalizedResourcesByCategory("hu", "Message");
            var huErrorsAfter = service.GetLocalizedResourcesByCategory("hu", "Error");

            // --- Assert
            huMessagesBefore.ShouldHaveCountOf(3);
            huMessagesAfter.ShouldHaveCountOf(0);
            huErrorsBefore.ShouldHaveCountOf(2);
            huErrorsAfter.ShouldHaveCountOf(0);
        }

        #endregion

        #region GetLocalizedResourceCategories tesztesetek

        [TestMethod]
        public void GetLocalizedResourceCategoriesWorksAsExpected()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitLocalizedResources.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            var categories = service.GetLocalizedResourceCategories();

            // --- Assert
            categories.ShouldHaveCountOf(2);
            categories.ShouldContain("Message");
            categories.ShouldContain("Error");
        }

        #endregion

        #region GetLocalizedResourcesByCategory tesztesetek

        [TestMethod]
        public void GetLocalizedResourcesCategoryWorksAsExpected()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitLocalizedResources.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            var cat1Def = service.GetLocalizedResourcesByCategory("def", "Message");
            var cat2EnUs = service.GetLocalizedResourcesByCategory("en-us", "Error");

            // --- Arrange
            cat1Def.ShouldHaveCountOf(3);
            cat1Def.Select(i => i.ResourceKey).ShouldContain("1");
            cat1Def.Select(i => i.ResourceKey).ShouldContain("2");
            cat1Def.Select(i => i.ResourceKey).ShouldContain("3");
            // ReSharper disable PossibleNullReferenceException
            cat1Def.FirstOrDefault(i => i.ResourceKey == "1").Value.ShouldEqual("Value1");
            cat1Def.FirstOrDefault(i => i.ResourceKey == "2").Value.ShouldEqual("Value2");
            cat1Def.FirstOrDefault(i => i.ResourceKey == "3").Value.ShouldEqual("Value3");
            // ReSharper restore PossibleNullReferenceException

            cat2EnUs.ShouldHaveCountOf(2);
            cat2EnUs.Select(i => i.ResourceKey).ShouldContain("1");
            cat2EnUs.Select(i => i.ResourceKey).ShouldContain("2");
            // ReSharper disable PossibleNullReferenceException
            cat2EnUs.FirstOrDefault(i => i.ResourceKey == "1").Value.ShouldEqual("One US");
            cat2EnUs.FirstOrDefault(i => i.ResourceKey == "2").Value.ShouldEqual("Two US");
            // ReSharper restore PossibleNullReferenceException
        }

        #endregion

        #region CloneLocalizedResources tesztesetek

        [TestMethod]
        public void CloneLocalizedResourcesFailsWithNullRequest()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.CloneLocalizedResources(null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("request");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void CloneLocalizedResourcesFailsWithNullBaseCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.CloneLocalizedResources(new CloneLocalizedResourcesDto
                {
                    BaseCode = null,
                    TargetCode = "fr",
                    DefaultResourceValue = null,
                    OverrideExistingResources = false
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("request.BaseCode");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void CloneLocalizedResourcesFailsWithNullTargetCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.CloneLocalizedResources(new CloneLocalizedResourcesDto
                {
                    BaseCode = "def",
                    TargetCode = null,
                    DefaultResourceValue = null,
                    OverrideExistingResources = false
                });
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("request.TargetCode");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void CloneLocalizedResourcesFailsWithUnknownBaseCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.CloneLocalizedResources(new CloneLocalizedResourcesDto
                {
                    BaseCode = "dummy",
                    TargetCode = "en",
                    DefaultResourceValue = null,
                    OverrideExistingResources = false
                });
            }
            catch (LocaleNotFoundException ex)
            {
                // --- Assert
                ex.Code.ShouldEqual("dummy");
                return;
            }
            Assert.Fail("LocaleNotFoundException expected");
        }

        [TestMethod]
        public void CloneLocalizedResourcesFailsWithUnknownTargetCode()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.CloneLocalizedResources(new CloneLocalizedResourcesDto
                {
                    BaseCode = "en",
                    TargetCode = "dummy",
                    DefaultResourceValue = null,
                    OverrideExistingResources = false
                });
            }
            catch (LocaleNotFoundException ex)
            {
                // --- Assert
                ex.Code.ShouldEqual("dummy");
                return;
            }
            Assert.Fail("LocaleNotFoundException expected");
        }

        [TestMethod]
        public void CloneLocalizedResourcesWorksAsExpectedWithNoOverride()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitLocalizedResources.sql");
            SqlScriptHelper.RunScript("SetupCloneScenario.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.CloneLocalizedResources(new CloneLocalizedResourcesDto
            {
                BaseCode = "def",
                TargetCode = "fr",
                DefaultResourceValue = null,
                OverrideExistingResources = false
            });

            // --- Assert
            var resources = service.GetLocalizedResourcesByCategory("fr", "Message");
            resources.ShouldHaveCountOf(2);
            // ReSharper disable PossibleNullReferenceException
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("Une");
            resources.FirstOrDefault(r => r.ResourceKey == "2").Value.ShouldEqual("Value2");
            resources = service.GetLocalizedResourcesByCategory("fr", "Error");
            resources.ShouldHaveCountOf(1);
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("Value1");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void CloneLocalizedResourcesWorksAsExpectedWithOverride()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitLocalizedResources.sql");
            SqlScriptHelper.RunScript("SetupCloneScenario.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.CloneLocalizedResources(new CloneLocalizedResourcesDto
            {
                BaseCode = "def",
                TargetCode = "fr",
                DefaultResourceValue = null,
                OverrideExistingResources = true
            });

            // --- Assert
            var resources = service.GetLocalizedResourcesByCategory("fr", "Message");
            resources.ShouldHaveCountOf(2);
            // ReSharper disable PossibleNullReferenceException
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("Value1"); // --- Ez az Override ereménye
            resources.FirstOrDefault(r => r.ResourceKey == "2").Value.ShouldEqual("Value2");
            resources = service.GetLocalizedResourcesByCategory("fr", "Error");
            resources.ShouldHaveCountOf(1);
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("Value1");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void CloneLocalizedResourcesWorksAsExpectedWithNoOverrideAndDefault()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitLocalizedResources.sql");
            SqlScriptHelper.RunScript("SetupCloneScenario.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.CloneLocalizedResources(new CloneLocalizedResourcesDto
            {
                BaseCode = "def",
                TargetCode = "fr",
                DefaultResourceValue = "magic",
                OverrideExistingResources = false
            });

            // --- Assert
            var resources = service.GetLocalizedResourcesByCategory("fr", "Message");
            resources.ShouldHaveCountOf(2);
            // ReSharper disable PossibleNullReferenceException
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("Une");
            resources.FirstOrDefault(r => r.ResourceKey == "2").Value.ShouldEqual("magic");
            resources = service.GetLocalizedResourcesByCategory("fr", "Error");
            resources.ShouldHaveCountOf(1);
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("magic");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void CloneLocalizedResourcesWorksAsExpectedWithOverrideAndDefault()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitLocalizedResources.sql");
            SqlScriptHelper.RunScript("SetupCloneScenario.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            service.CloneLocalizedResources(new CloneLocalizedResourcesDto
            {
                BaseCode = "def",
                TargetCode = "fr",
                DefaultResourceValue = "magic",
                OverrideExistingResources = true
            });

            // --- Assert
            var resources = service.GetLocalizedResourcesByCategory("fr", "Message");
            resources.ShouldHaveCountOf(2);
            // ReSharper disable PossibleNullReferenceException
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("magic"); // --- Ez az Override ereménye
            resources.FirstOrDefault(r => r.ResourceKey == "2").Value.ShouldEqual("magic");
            resources = service.GetLocalizedResourcesByCategory("fr", "Error");
            resources.ShouldHaveCountOf(1);
            resources.FirstOrDefault(r => r.ResourceKey == "1").Value.ShouldEqual("magic");
            // ReSharper restore PossibleNullReferenceException
        }

        #endregion

        #region GetListItem tesztesetek

        [TestMethod]
        public void GetListItemFailsWithNullListId()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.GetListItems(null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("listId");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void GetListItemFailsWithEmptyListId()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.GetListItems("   ");
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("listId");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void GetListItemFailsWithNullLocale()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.GetListItems("List", null);
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        public void GetListItemFailsWithEmptyLocale()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act
            try
            {
                service.GetListItems("List", "  ");
            }
            catch (ArgumentValidationException ex)
            {
                // --- Assert
                ex.Notifications.Items.ShouldHaveCountOf(1);
                ex.Notifications.Items[0].Target.ShouldEqual("locale");
                ex.Notifications.Items[0].Code.ShouldEqual(ValidationCodes.NULL_NOT_ALLOWED);
                return;
            }
            Assert.Fail("ArgumentValidationException expected");
        }

        [TestMethod]
        [ExpectedException(typeof(ListDefinitionNotFoundException))]
        public void GetListItemFailsWithNonExistingList()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitListItems.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act 
            service.GetListItems("Dummy");
        }

        [TestMethod]
        public void GetListItemWorksAsExpected()
        {
            // --- Arrange
            SqlScriptHelper.RunScript("InitListItems.sql");
            var service = ServiceManager.GetService<IConfigurationService>();

            // --- Act 
            var typesDef = service.GetListItems("Type");
            var catHu = service.GetListItems("Cat", "hu");
            var catDe = service.GetListItems("Cat", "de");

            // --- Assert
            typesDef.ShouldHaveCountOf(3);
            typesDef.First(l => l.Id == "1").DisplayName.ShouldEqual("Type.Value1");
            typesDef.First(l => l.Id == "1").Description.ShouldEqual("Type.Des1");
            typesDef.First(l => l.Id == "2").DisplayName.ShouldEqual("Type.Value2");
            typesDef.First(l => l.Id == "2").Description.ShouldEqual("Type.Des2");
            typesDef.First(l => l.Id == "3").DisplayName.ShouldEqual("Type.Value3");
            typesDef.First(l => l.Id == "3").Description.ShouldEqual("Type.Des3");

            catHu.ShouldHaveCountOf(2);
            catHu.First(l => l.Id == "F").DisplayName.ShouldEqual("Cat.1");
            catHu.First(l => l.Id == "F").Description.ShouldEqual("Cat.D1");
            catHu.First(l => l.Id == "S").DisplayName.ShouldEqual("Cat.2");
            catHu.First(l => l.Id == "S").Description.ShouldEqual("Cat.D2");

            catDe.ShouldHaveCountOf(2);
            catDe.First(l => l.Id == "F").DisplayName.ShouldEqual("Cat.Value1");
            catDe.First(l => l.Id == "F").Description.ShouldEqual("Cat.Des1");
            catDe.First(l => l.Id == "S").DisplayName.ShouldEqual("Cat.Value2");
            catDe.First(l => l.Id == "S").Description.ShouldEqual("Cat.Des2");
        }

        #endregion
    }
}
