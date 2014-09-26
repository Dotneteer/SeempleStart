using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration.ResourceConnections;

namespace Seemplest.Core.UnitTests.Configuration.ResourceConnection
{
    [TestClass]
    public class DefaultResourceConnectionProviderRegistryTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterFailsWithInvalidType()
        {
            // --- Arrange
            var registry = new DefaultResourceConnectionProviderRegistry();

            // --- Act
            registry.RegisterResourceConnectionProvider(typeof(int));
        }
    }
}
