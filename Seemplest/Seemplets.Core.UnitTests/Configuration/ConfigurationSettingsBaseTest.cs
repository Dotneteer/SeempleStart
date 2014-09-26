using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.Common;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration
{
    [TestClass]
    public class ConfigurationSettingsBaseTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullElementIsNotAccepted()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new DummySettings((XElement)null);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void CloneWorksAsExpected()
        {
            // --- Arrange
            var dummy = new DummySettings("DummyValue");

            // --- Act
            var clone = dummy.Clone();

            // --- Assert
            clone.Dummy.ShouldEqual(dummy.Dummy);
        }
    }

    internal class DummySettings: ConfigurationSettingsBase
    {
        public string Dummy { get; private set; }

        public DummySettings(string dummy)
        {
            Dummy = dummy;
        }

        public DummySettings(XElement element) : base(element)
        {
        }

        public override XElement WriteToXml(XName rootElement)
        {
            return new XElement(rootElement, new XAttribute("Dummy", Dummy));
        }

        protected override void ParseFromXml(XElement element)
        {
            Dummy = element.StringAttribute("Dummy");
        }

        public DummySettings Clone()
        {
            return Clone<DummySettings>();
        }
    }
}
