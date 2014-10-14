using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestBlocks.Core.Internationalization; 
using SeemplestBlocks.Dto.Internationalization;
using SoftwareApproach.TestingExtensions;

namespace SeemplestBlocks.UnitTests.Internationalization
{
    [TestClass]
    public class ResourceControllerBaseTest
    {
        [TestMethod]
        public void GetResourceswithHuWorksAsExpected()
        {
            // --- Arrange
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("hu");
            var controller = new TestResourceController();

            // --- Act
            var resources = controller.GetResources();

            // --- Assert
            // ReSharper disable PossibleNullReferenceException
            resources.FirstOrDefault(r => r.Code == "Code1").Value.ShouldEqual("Érték1");
            resources.FirstOrDefault(r => r.Code == "Code2").Value.ShouldEqual("Érték2");
            // ReSharper restore PossibleNullReferenceException
        }

        [TestMethod]
        public void GetResourceswithEnWorksAsExpected()
        {
            // --- Arrange
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            var controller = new TestResourceController();

            // --- Act
            var resources = controller.GetResources();

            // --- Assert
            // ReSharper disable PossibleNullReferenceException
            resources.FirstOrDefault(r => r.Code == "Code1").Value.ShouldEqual("Value1");
            resources.FirstOrDefault(r => r.Code == "Code2").Value.ShouldEqual("Érték2");
            // ReSharper restore PossibleNullReferenceException
        }

        internal class TestResourceController : ResourceControllerBase
        {
            public List<ResourceStringDto> GetResources()
            {
                return GetResources(typeof (TestResource));
            } 
        }
    }
}
