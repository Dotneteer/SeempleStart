using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class ResourceConnectionProviderCollectionTest
    {
        [TestMethod]
        public void ProviderDictionaryWorksAsExpected()
        {
            // --- Arrange
            var coll1 = new ResourceConnectionProviderCollection();
            var coll2 = new ResourceConnectionProviderCollection
                {
                    new IntValueConnectionProvider("provider", 12345)
                };

            // --- Act/Assert
            coll1.ProviderDictionary.Count.ShouldEqual(0);
            coll2.ProviderDictionary.Count.ShouldEqual(1);
        }

        class IntValueConnectionProvider: SingleValueResourceConnectionProvider<int>
        {
            public IntValueConnectionProvider(string name, int value) : base(name, value)
            {
            }

            public IntValueConnectionProvider(XElement element) : base(element)
            {
            }

            public override object GetResourceConnectionFromSettings()
            {
                return Value;
            }
        }
    }
}
