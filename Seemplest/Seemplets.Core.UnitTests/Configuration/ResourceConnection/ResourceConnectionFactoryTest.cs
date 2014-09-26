using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class ResourceConnectionFactoryTest
    {
        [TestInitialize]
        public void Initilaize()
        {
            ResourceConnectionProviderRegistry.Reset();
            ResourceConnectionFactory.Reset();
        }

        [TestMethod]
        public void ConfigurationWithFactoryInstanceWorks()
        {
            // --- Act
            ResourceConnectionFactory.Configure(ResourceConnectionFactory.Current);
        }

        [TestMethod]
        public void ConfigurationWithDefaultSectionWorks()
        {
            // --- Act
            ResourceConnectionFactory.Configure();
        }


        [TestMethod]
        public void ConfigurationWorksFromAlternateSection()
        {
            // --- Arrange
            var onChangedCalled = false;
            ResourceConnectionFactory.ConfigurationChanged +=
                (sender, args) => { onChangedCalled = true; };

            // --- Act
            ResourceConnectionFactory.Configure("ResourceConnections1");

            // --- Assert
            onChangedCalled.ShouldBeTrue();
        }
    }
}
