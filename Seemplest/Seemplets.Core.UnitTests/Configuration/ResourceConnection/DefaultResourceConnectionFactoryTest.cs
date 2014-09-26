using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class DefaultResourceConnectionFactoryTest
    {
        [TestMethod]
        public void FactoryWorksAsExpected()
        {
            // --- Arrange
            var registry = new DefaultResourceConnectionProviderRegistry();
            registry.RegisterResourceConnectionProvider(typeof(MyConnection1));
            registry.RegisterResourceConnectionProvider(typeof(MyConnection2));

            var providers = new ResourceConnectionProviderCollection
                {
                    new MyConnection1("name1", 12345),
                    new MyConnection1("name2", 54321),
                    new MyConnection2("name3", "345"),
                    new MyConnection2("name4", "678")
                };
            var settings = new ResourceConnectionFactorySettings(providers);
            var factory = new DefaultResourceConnectionFactory(settings);

            // --- Act
            var resource1 = factory.CreateResourceConnection<int>("name1");
            var resource2 = factory.CreateResourceConnection<int>("name2");
            var resource3 = factory.CreateResourceConnection<int>("name3");
            var resource4 = factory.CreateResourceConnection<int>("name4");

            // --- Assert
            resource1.ShouldEqual(12345);
            resource2.ShouldEqual(54321);
            resource3.ShouldEqual(345);
            resource4.ShouldEqual(678);
        }

        [TestMethod]
        public void FactoryReadsFromConfiguration()
        {
            // --- Arrange
            var registry = new DefaultResourceConnectionProviderRegistry();
            registry.RegisterResourceConnectionProvider(typeof(MyConnection1));
            registry.RegisterResourceConnectionProvider(typeof(MyConnection2));
            ResourceConnectionProviderRegistry.Configure(registry);

            var providers = new ResourceConnectionProviderCollection
                {
                    new MyConnection1("name1", 12345),
                    new MyConnection1("name2", 54321),
                    new MyConnection2("name3", "345"),
                    new MyConnection2("name4", "678")
                };
            var settings = new ResourceConnectionFactorySettings(providers);
            var element = settings.WriteToXml("ResourceConnections");
            settings = new ResourceConnectionFactorySettings(element);
            var factory = new DefaultResourceConnectionFactory(settings);

            // --- Act
            var resource1 = factory.CreateResourceConnection<int>("name1");
            var resource2 = factory.CreateResourceConnection<int>("name2");
            var resource3 = factory.CreateResourceConnection<int>("name3");
            var resource4 = factory.CreateResourceConnection<int>("name4");

            // --- Assert
            resource1.ShouldEqual(12345);
            resource2.ShouldEqual(54321);
            resource3.ShouldEqual(345);
            resource4.ShouldEqual(678);
        }


        class MyConnection1 : SingleValueResourceConnectionProvider<int>
        {
            public MyConnection1(string name, int value)
                : base(name, value)
            {
            }

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

        private class MyConnection2 : SingleValueResourceConnectionProvider<string>
        {
            public MyConnection2(string name, string value)
                : base(name, value)
            {
            }

            // ReSharper disable UnusedMember.Local
            public MyConnection2(XElement element)
                // ReSharper restore UnusedMember.Local
                : base(element)
            {
            }

            public override object GetResourceConnectionFromSettings()
            {
                return int.Parse(Value);
            }
        }
    }
}
