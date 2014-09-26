using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;

namespace Seemplest.Core.UnitTests.DataAccess.DataServices
{
    [TestClass]
    public class DefaultDataAccessFactoryTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateFailsWithNull()
        {
            // --- Act
            // ReSharper disable once ObjectCreationAsStatement
            new DefaultDataAccessFactory(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateContextFailsWithNonDataOperation()
        {
            // --- Arrange
            var factory = new DefaultDataAccessFactory(ServiceManager.ServiceRegistry);

            // --- Act
            factory.CreateContext(typeof (int));
        }
    }
}
