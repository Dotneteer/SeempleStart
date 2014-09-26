using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class ResourceConnectionProviderRegistryTest
    {
        [TestMethod]
        public void OnConfigurationChangedWorksAsExpected()
        {
            // --- Arrange
            var onChangedCalled = false;
            var registry = new DefaultResourceConnectionProviderRegistry();
            registry.RegisterResourceConnectionProvider(typeof(MyConnection1));

            // --- Act
            ResourceConnectionProviderRegistry.ConfigurationChanged +=
                (sender, args) => { onChangedCalled = true; };
            ResourceConnectionProviderRegistry.Configure(registry);

            // --- Assert
            onChangedCalled.ShouldBeTrue();
        }

        class MyConnection1 : SingleValueResourceConnectionProvider<int>
        {
            // ReSharper disable UnusedMember.Local
            public MyConnection1(XElement element)
                // ReSharper restore UnusedMember.Local
                : base(element)
            {
            }

            public override object GetResourceConnectionFromSettings()
            {
                return Value;
            }
        }
    }
}
