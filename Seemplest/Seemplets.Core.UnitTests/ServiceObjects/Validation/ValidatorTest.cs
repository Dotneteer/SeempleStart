using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.ServiceObjects.Validation;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.ServiceObjects.Validation
{
    [TestClass]
    public class ValidatorTest
    {
        [TestMethod]
        public void ValidatorWorksAsExpected1()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = 12,
                Names = new List<string> {"Peter", "Valerie"}
            };

            // --- Act
            val.Require(entity.Id.HasValue, "Id");
            val.Require(entity.Names.Count > 0, "Names");

            // --- Assert
            val.HasError.ShouldBeFalse();
        }

        [TestMethod]
        public void ValidatorWorksAsExpected2()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, "Id");
            val.Require(entity.Names.Count > 0, "Names");

            // --- Assert
            val.HasError.ShouldBeTrue();
            val.Notifications.Count.ShouldEqual(1);
            val.Notifications.FirstOrDefault(n => n.Target == "Id").ShouldNotBeNull();
        }

        [TestMethod]
        public void ValidatorWorksAsExpected3()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = 12,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, (SampleEntity e) => e.Id);
            val.Require(entity.Names.Count > 0, (SampleEntity e) => e.Names);

            // --- Assert
            val.HasError.ShouldBeFalse();
        }

        [TestMethod]
        public void ValidatorWorksAsExpected4()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, (SampleEntity e) => e.Id);
            val.Require(entity.Names.Count > 0, (SampleEntity e) => e.Names);

            // --- Assert
            val.HasError.ShouldBeTrue();
            val.Notifications.Count.ShouldEqual(1);
            val.Notifications.FirstOrDefault(n => n.Target == "Id").ShouldNotBeNull();
        }

        [TestMethod]
        public void ValidatorWorksAsExpected5()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, "Id", handleAsError: false);
            val.Require(entity.Names.Count > 0, "Names");

            // --- Assert
            val.HasError.ShouldBeFalse();
            val.Notifications.Count.ShouldEqual(1);
            val.Notifications.FirstOrDefault(n => n.Target == "Id").ShouldNotBeNull();
        }

        [TestMethod]
        public void ValidatorWorksAsExpected6()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, (SampleEntity e) => e.Id, handleAsError: false);
            val.Require(entity.Names.Count > 0, (SampleEntity e) => e.Names);

            // --- Assert
            val.HasError.ShouldBeFalse();
            val.Notifications.Count.ShouldEqual(1);
            val.Notifications.FirstOrDefault(n => n.Target == "Id").ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentValidationException))]
        public void ValidationExceptionIsRaised1()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, (SampleEntity e) => e.Id);
            val.Require(entity.Names.Count > 0, (SampleEntity e) => e.Names);
            val.RaiseWhenFailed();
        }

        [TestMethod]
        public void ValidationExceptionIsNotRaised1()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, (SampleEntity e) => e.Id, handleAsError: false);
            val.Require(entity.Names.Count > 0, (SampleEntity e) => e.Names);
            val.RaiseWhenFailed();

            // --- Assert: there is no exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentValidationException))]
        public void ValidationExceptionIsRaised2()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, (SampleEntity e) => e.Id, handleAsError: false);
            val.Require(entity.Names.Count > 0, (SampleEntity e) => e.Names);
            val.RaiseIfHasIssues();
        }

        [TestMethod]
        public void ValidationExceptionIsNotRaised2()
        {
            // --- Arrange
            var val = new Validator();
            var entity = new SampleEntity
            {
                Id = 12,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            val.Require(entity.Id.HasValue, (SampleEntity e) => e.Id, handleAsError: false);
            val.Require(entity.Names.Count > 0, (SampleEntity e) => e.Names);
            val.RaiseIfHasIssues();

            // --- Assert: there is no exception
        }

        public class SampleEntity
        {
            public int? Id { get; set; }
            public List<string> Names { get; set; }
        }
    }
}
