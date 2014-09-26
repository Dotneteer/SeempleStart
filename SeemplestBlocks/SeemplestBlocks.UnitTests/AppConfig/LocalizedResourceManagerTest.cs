using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.AppConfig;
using SeemplestBlocks.Core.AppConfig.DataAccess;
using SeemplestBlocks.UnitTests.Helpers;
using SoftwareApproach.TestingExtensions;

namespace SeemplestBlocks.UnitTests.AppConfig
{
    [TestClass]
    public class LocalizedResourceManagerTest
    {
        private const string DB_CONN = "connStr=Core";

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<IConfigurationDataOperations, ConfigurationDataOperations>(DB_CONN);
            ServiceManager.Register<IConfigurationService, ConfigurationService>();
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
            SqlScriptHelper.RunScript("InitMultipleLocales.sql");
        }

        [TestInitialize]
        public void Initialize()
        {
            LocalizedResourceManager.Reset(TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public void GetResourceWorksAsExpected()
        {
            // --- Act
            var resource = LocalizedResourceManager.GetResourceByLocale("hu-hu", "Cat1", "1");

            // --- Assert
            resource.ShouldEqual("Cat1Value1-hu-hu");
        }

        [TestMethod]
        public void GetResourceFallsbackToLanguage()
        {
            // --- Act
            var resource = LocalizedResourceManager.GetResourceByLocale("de-au", "Cat1", "2");

            // --- Assert
            resource.ShouldEqual("Cat1Value2-de");
        }

        [TestMethod]
        public void GetResourceFallsbackToDefaultLanguage()
        {
            // --- Act
            var resource = LocalizedResourceManager.GetResourceByLocale("fr-ca","Cat2", "1");

            // --- Assert
            resource.ShouldEqual("Cat2Value1");
        }

        [TestMethod]
        public void NoLocaleFallsbackToDefaultLanguage()
        {
            // --- Act
            var resource = LocalizedResourceManager.GetResourceByLocale("", "Cat1", "1");

            // --- Assert
            resource.ShouldEqual("Cat1Value1");
        }

        [TestMethod]
        public void CacheIsUsedAsExpected()
        {
            // --- Act
            var cacheHit1 = LocalizedResourceManager.IsCached("hu-hu", "Cat1");
            var resource = LocalizedResourceManager.GetResourceByLocale("hu-hu", "Cat1", "1");
            var cacheHit2 = LocalizedResourceManager.IsCached("hu-hu", "Cat1");

            // --- Assert
            resource.ShouldEqual("Cat1Value1-hu-hu");
            cacheHit1.ShouldBeFalse();
            cacheHit2.ShouldBeTrue();
        }

        [TestMethod]
        public void ExpiredCacheItemIsRemoved()
        {
            // --- Arrange
            LocalizedResourceManager.Reset(TimeSpan.FromMilliseconds(100));

            // --- Act
            var cacheHit1 = LocalizedResourceManager.IsCached("hu-hu", "Cat1");
            LocalizedResourceManager.GetResourceByLocale("hu-hu", "Cat1", "1");
            var cacheHit2 = LocalizedResourceManager.IsCached("hu-hu", "Cat1");
            Thread.Sleep(200);
            var cacheHit3 = LocalizedResourceManager.IsCached("hu-hu", "Cat1");
            var resource = LocalizedResourceManager.GetResourceByLocale("hu-hu", "Cat1", "1");

            // --- Assert
            resource.ShouldEqual("Cat1Value1-hu-hu");
            cacheHit1.ShouldBeFalse();
            cacheHit2.ShouldBeTrue();
            cacheHit3.ShouldBeFalse();
        }
    }
}
