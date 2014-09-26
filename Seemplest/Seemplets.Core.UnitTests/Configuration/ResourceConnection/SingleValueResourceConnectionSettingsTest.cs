using System.Globalization;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class SingleValueResourceConnectionSettingsTest
    {
        [TestMethod]
        public void ReadAndWriteXmlWorksAsExpected()
        {
            // --- Arrange
            var setting = new MyProvider("name", 12345);

            // --- Act
            var element = setting.WriteToXml("Settings");
            var newSetting = new MyProvider(element);

            // -- Assert
            setting.Name.ShouldEqual(newSetting.Name);
            setting.Value.ShouldEqual(newSetting.Value);
            setting.GetResourceConnectionFromSettings().ShouldEqual(newSetting.Value.ToString(CultureInfo.InvariantCulture));
        }

        private class MyProvider : SingleValueResourceConnectionProvider<int>
        {
            public MyProvider(string name, int value)
                : base(name, value)
            {
            }

            public MyProvider(XElement element)
                : base(element)
            {
            }

            public override object GetResourceConnectionFromSettings()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
