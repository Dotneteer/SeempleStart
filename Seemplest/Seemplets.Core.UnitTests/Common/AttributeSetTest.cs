using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Common;

namespace Seemplest.Core.UnitTests.Common
{
    [TestClass]
    public class AttributeSetTest
    {
        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ConstructingWithNullMemberInfoFails()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new AttributeSet(null);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void SingleWithMultipleInstancesFails()
        {
            // --- Act
            var set = new AttributeSet(typeof (MultipleAttributesUsed));
            set.Single<MultiAttribute>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OptionalWithMultipleInstancesFails()
        {
            // --- Act
            var set = new AttributeSet(typeof(MultipleAttributesUsed));
            set.Optional<MultiAttribute>();
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        class MultiAttribute : Attribute {}
        
        [Multi]
        [Multi]
        class MultipleAttributesUsed { }
    }
}
