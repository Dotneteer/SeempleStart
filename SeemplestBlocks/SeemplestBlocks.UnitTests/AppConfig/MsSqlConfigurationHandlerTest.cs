using System;
using System.Threading;
using System.Threading.Tasks;
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
    public class MsSqlConfigurationHandlerTest
    {
        private const string DB_CONN = "connStr=Core";

        [TestInitialize]
        public void TestInitialize()
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<IConfigurationDataOperations, ConfigurationDataOperations>(DB_CONN);
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);

            SqlScriptHelper.RunScript("InitConfigurationValues.sql");
            MsSqlConfigurationHandler.Reset(TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        public void GetConfigurationValueWorksAsExpected()
        {
            // --- Arrange
            var handler = new MsSqlConfigurationHandler();

            // --- Act
            string value1;
            var found1 = handler.GetConfigurationValue("NonExistingCategory", "key", out value1);
            string value2;
            var found2 = handler.GetConfigurationValue("Category1", "NonExistingKey", out value2);
            string value3;
            var found3 = handler.GetConfigurationValue("Category1", "Key1", out value3);

            // --- Assert
            found1.ShouldBeFalse();
            found2.ShouldBeFalse();
            found3.ShouldBeTrue();
            value3.ShouldEqual("123");
        }

        [TestMethod]
        public void SetConfigurationValueWorksWithNullValue()
        {
            // --- Arrange
            var handler = new MsSqlConfigurationHandler();
            var ctx = ServiceManager.GetService<IConfigurationDataOperations>();

            // --- Act
            handler.SetConfigurationValue("Category1", "Key1", null);

            // --- Assert
            var records = ctx.GetAllConfigurationValues();
            records.ShouldHaveCountOf(5);
        }

        [TestMethod]
        public void SetConfigurationValueWorksWithNewValue()
        {
            // --- Arrange
            var handler = new MsSqlConfigurationHandler();
            var ctx = ServiceManager.GetService<IConfigurationDataOperations>();

            // --- Act
            handler.SetConfigurationValue("Category1", "Key4", "Insert OK");

            // --- Assert
            var records = ctx.GetAllConfigurationValues();
            records.ShouldHaveCountOf(7);
            string value;
            var found = handler.GetConfigurationValue("Category1", "Key4", out value);
            found.ShouldBeTrue();
            value.ShouldEqual("Insert OK");
        }

        [TestMethod]
        public void SetConfigurationValueWorksWithUpdatedValue()
        {
            // --- Arrange
            var handler = new MsSqlConfigurationHandler();
            var ctx = ServiceManager.GetService<IConfigurationDataOperations>();

            // --- Act
            handler.SetConfigurationValue("Category1", "Key1", "Update OK");

            // --- Assert
            var records = ctx.GetAllConfigurationValues();
            records.ShouldHaveCountOf(6);
            string value;
            var found = handler.GetConfigurationValue("Category1", "Key1", out value);
            found.ShouldBeTrue();
            value.ShouldEqual("Update OK");
        }

        [TestMethod]
        public void ConfigurationIsRefreshedAfterRetention()
        {
            // --- Arrange
            MsSqlConfigurationHandler.Reset(TimeSpan.FromMilliseconds(100));
            var handler = new MsSqlConfigurationHandler();

            // --- Act
            var refreshTime1 = MsSqlConfigurationHandler.LastRefreshTime;
            string value1;
            handler.GetConfigurationValue("Category1", "Key1", out value1);
            string value2;
            handler.SetConfigurationValue("Category1", "Key1", "New");
            handler.GetConfigurationValue("Category1", "Key1", out value2);
            var refreshTime2 = MsSqlConfigurationHandler.LastRefreshTime;
            Thread.Sleep(100);
            string value3;
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            Thread.Sleep(100);
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            var refreshTime3 = MsSqlConfigurationHandler.LastRefreshTime;

            // --- Assert
            refreshTime1.ShouldEqual(refreshTime2);
            value1.ShouldEqual(value2);
            refreshTime3.ShouldNotEqual(refreshTime2);
            value3.ShouldNotEqual(value2);
        }

        [TestMethod]
        public void ConfigurationIsNotRefreshedAfterHold()
        {
            // --- Arrange
            MsSqlConfigurationHandler.Reset(TimeSpan.FromMilliseconds(100));
            var handler = new MsSqlConfigurationHandler();

            // --- Act
            var refreshTime1 = MsSqlConfigurationHandler.LastRefreshTime;
            string value1;
            handler.GetConfigurationValue("Category1", "Key1", out value1);
            string value2;
            handler.SetConfigurationValue("Category1", "Key1", "New");
            
            MsSqlConfigurationHandler.Hold();
            
            handler.GetConfigurationValue("Category1", "Key1", out value2);
            var refreshTime2 = MsSqlConfigurationHandler.LastRefreshTime;
            Thread.Sleep(100);
            string value3;
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            Thread.Sleep(100);
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            var refreshTime3 = MsSqlConfigurationHandler.LastRefreshTime;

            // --- Assert
            refreshTime1.ShouldEqual(refreshTime2);
            value1.ShouldEqual(value2);
            refreshTime3.ShouldEqual(refreshTime2);
            value3.ShouldEqual(value2);
        }

        [TestMethod]
        public void ConfigurationIsRefreshedAfterReset()
        {
            // --- Arrange
            MsSqlConfigurationHandler.Reset(TimeSpan.FromMilliseconds(100));
            var handler = new MsSqlConfigurationHandler();

            // --- Act
            var refreshTime1 = MsSqlConfigurationHandler.LastRefreshTime;
            string value1;
            handler.GetConfigurationValue("Category1", "Key1", out value1);
            string value2;
            handler.SetConfigurationValue("Category1", "Key1", "New");

            MsSqlConfigurationHandler.Hold();

            handler.GetConfigurationValue("Category1", "Key1", out value2);
            var refreshTime2 = MsSqlConfigurationHandler.LastRefreshTime;
            Thread.Sleep(100);
            string value3;
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            Thread.Sleep(100);
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            var refreshTime3 = MsSqlConfigurationHandler.LastRefreshTime;

            MsSqlConfigurationHandler.Reset(TimeSpan.FromMilliseconds(100));

            var refreshTime4 = MsSqlConfigurationHandler.LastRefreshTime;
            string value4;
            handler.GetConfigurationValue("Category1", "Key1", out value4);

            // --- Assert
            refreshTime1.ShouldEqual(refreshTime2);
            value1.ShouldEqual(value2);
            refreshTime3.ShouldEqual(refreshTime2);
            value3.ShouldEqual(value2);

            refreshTime4.ShouldNotEqual(refreshTime3);
            value4.ShouldNotEqual(value3);
        }

        [TestMethod]
        public void ParallelRefreshWorksAsExpected()
        {
            // --- Arrange
            MsSqlConfigurationHandler.Reset(TimeSpan.FromMilliseconds(100));
            var handler = new MsSqlConfigurationHandler();

            // --- Act
            var refreshTime1 = MsSqlConfigurationHandler.LastRefreshTime;
            string value1;
            handler.GetConfigurationValue("Category1", "Key1", out value1);
            string value2;
            handler.SetConfigurationValue("Category1", "Key1", "New");
            handler.GetConfigurationValue("Category1", "Key1", out value2);
            var refreshTime2 = MsSqlConfigurationHandler.LastRefreshTime;
            Thread.Sleep(100);
            string value3;
            Parallel.Invoke(
                () => handler.GetConfigurationValue("Category1", "Key1", out value3),
                () => handler.GetConfigurationValue("Category1", "Key1", out value3));
            Thread.Sleep(100);
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            var refreshTime3 = MsSqlConfigurationHandler.LastRefreshTime;

            // --- Assert
            refreshTime1.ShouldEqual(refreshTime2);
            value1.ShouldEqual(value2);
            refreshTime3.ShouldNotEqual(refreshTime2);
            value3.ShouldNotEqual(value2);
        }

        [TestMethod]
        public void RefreshDoesNotThrowException()
        {
            // --- Arrange
            MsSqlConfigurationHandler.Reset(TimeSpan.FromMilliseconds(100));
            var handler = new MsSqlConfigurationHandler();

            // --- Act
            string value1;
            handler.GetConfigurationValue("Category1", "Key1", out value1);
            string value2;
            handler.GetConfigurationValue("Category1", "Key1", out value2);
            Thread.Sleep(100);

            ServiceManager.RemoveService(typeof(IConfigurationDataOperations));
            
            string value3;
            handler.GetConfigurationValue("Category1", "Key1", out value3);
            Thread.Sleep(100);

            // --- Assert
            MsSqlConfigurationHandler.LastRefreshException.ShouldBeOfType(typeof (AggregateException));
        }
    }
}
