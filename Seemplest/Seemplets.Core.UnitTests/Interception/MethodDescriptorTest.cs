using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Interception;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Interception
{
    [TestClass]
    public class MethodDescriptorTest
    {
        [TestMethod]
        public void CallDescriptorConstructionWorksWithNoArguments()
        {
            // --- Act
            var descriptor = new MethodCallDescriptor(this, typeof (MethodDescriptorTest)
                .GetMethod("ConstructionWorksWithNoArguments"), null);

            // --- Assert
            descriptor.ArgumentCount.ShouldEqual(0);
        }

        [TestMethod]
        public void ResultDescriptorConstructionWorksWithNoArguments()
        {
            // --- Act
            var descriptor = new MethodResultDescriptor(false, null, null);

            // --- Assert
            descriptor.OutputArgumentCount.ShouldEqual(0);
        }
    }
}
