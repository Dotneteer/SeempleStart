using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class ResourceConnectionFactorySettingsTest
    {
        [TestMethod]
        public void DefaultConstructorWorks()
        {
            // --- Act
            var settings = new ResourceConnectionFactorySettings();

            // --- Assert
            settings.Providers.ShouldHaveCountOf(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructionWithNullProvidersFails()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ResourceConnectionFactorySettings((ResourceConnectionProviderCollection)null);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void ParseFailsWithUnknownProvider()
        {
            // --- Assert
            const string CONFIG =
                @"<ResourceConnections>
                    <MyProvider name='queueDB' value='Data Source=.\sqlexpress;Integrated Security=True;Initial Catalog=Seemplest.Test;'/>
                  </ResourceConnections>";
            ResourceConnectionProviderRegistry.Reset();

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ResourceConnectionFactorySettings(XElement.Parse(CONFIG));
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void ParseFailsWithProviderInitializationIssue()
        {
            // --- Assert
            const string CONFIG =
                @"<ResourceConnections>
                    <MyProvider name='aaa' value='123' />
                  </ResourceConnections>";
            ResourceConnectionProviderRegistry.Reset();
            ResourceConnectionProviderRegistry.Current.RegisterResourceConnectionProvider(typeof(MyProvider));

            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ResourceConnectionFactorySettings(XElement.Parse(CONFIG));
            // ReSharper restore ObjectCreationAsStatement
        }

        [DisplayName("MyProvider")]
        class MyProvider : SingleValueResourceConnectionProvider<int>
        {
            public MyProvider(XElement element) : base(element)
            {
                throw new NotImplementedException();
            }

            public override object GetResourceConnectionFromSettings()
            {
                throw new NotImplementedException();
            }
        }
    }
}
