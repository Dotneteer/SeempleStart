using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.TypeResolution;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.TypeResolution
{
    [TestClass]
    public class TypeResolverConfigurationSettingsTest
    {
        [TestMethod]
        public void WriteXmlAndReadXmlWorksAsExpected()
        {
            // --- Arrange
            var setting = new TypeResolverConfigurationSettings(
                new List<string> { "Asm1", "Asm2" },
                new List<string> { "Ns1", "Ns2", "Ns3" }
                );

            // --- Act
            var element = setting.WriteToXml("Test");

            // --- Assert
            var newSettings = new TypeResolverConfigurationSettings(element);
            newSettings.AssemblyNames.ShouldHaveCountOf(2);
            newSettings.AssemblyNames[0].ShouldEqual("Asm1");
            newSettings.AssemblyNames[1].ShouldEqual("Asm2");
            newSettings.Namespaces.ShouldHaveCountOf(3);
            newSettings.Namespaces[0].ShouldEqual("Ns1");
            newSettings.Namespaces[1].ShouldEqual("Ns2");
            newSettings.Namespaces[2].ShouldEqual("Ns3");
        }
    }
}
