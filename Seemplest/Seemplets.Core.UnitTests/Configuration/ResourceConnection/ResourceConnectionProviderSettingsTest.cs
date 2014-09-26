using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class ResourceConnectionProviderSettingsTest
    {
        [TestMethod]
        public void EmptySectionConstructionWorks()
        {
            // --- Act
            var settings = new ResourceConnectionProviderSettings();

            // --- Assert
            settings.Providers.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void ConstructorWithTypesWorks()
        {
            // --- Arrange
            var types = new List<Type>()
                {
                    typeof (int),
                    typeof (string)
                };
            
            // --- Act
            var settings = new ResourceConnectionProviderSettings(types);

            // --- Assert
            settings.Providers.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void ConstructFromXElementWorks()
        {
            // --- Arrange
            var types = new List<Type>()
                {
                    typeof (int),
                    typeof (string)
                };
            var settings = new ResourceConnectionProviderSettings(types);
            var xml = settings.WriteToXml("provider");

            // --- Act
            settings = new ResourceConnectionProviderSettings(xml);

            // --- Assert
            settings.Providers.ShouldHaveCountOf(2);
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void ParseFromNullTypeFails()
        {
            // --- Arrange
            var xml = XElement.Parse(
                @"<provider>
                    <Provider type='System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' />
                    <Provider type='12345' />
                  </provider>");
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new ResourceConnectionProviderSettings(xml);
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}
