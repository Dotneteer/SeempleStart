using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.ServiceObjects
{
    [TestClass]
    public class ServiceObjectValidationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            ServiceManager.Reset();
            ServiceManager.Register<ISampleService, SampleService>();
        }

        [TestMethod]
        public void OperationWorksAsExpected()
        {
            // --- Arrange
            var serv = ServiceManager.GetService<ISampleService>();
            var entity = new SampleEntity
            {
                Id = 12,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            serv.DoOperation(entity);

            // --- Assert
            serv.BusinessExceptionVisited.ShouldBeFalse();
            serv.InfrastructureExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void OperationRaisesBusinessException()
        {
            // --- Arrange
            var serv = ServiceManager.GetService<ISampleService>();
            var entity = new SampleEntity
            {
                Id = null,
                Names = new List<string> { "Peter", "Valerie" }
            };

            // --- Act
            try
            {
                serv.DoOperation(entity);
            }
            catch (BusinessOperationException ex)
            {
                // --- This exception is intentionally drained
            }

            // --- Assert
            serv.BusinessExceptionVisited.ShouldBeTrue();
            serv.InfrastructureExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void OperationRaisesInfrastructureException()
        {
            // --- Arrange
            var serv = ServiceManager.GetService<ISampleService>();
            var entity = new SampleEntity
            {
                Id = 12,
                Names = new List<string> { "Peter", "Valerie" },
                RaiseException = true
            };

            // --- Act
            try
            {
                serv.DoOperation(entity);
            }
            catch (InvalidOperationException ex)
            {
                // --- This exception is intentionally drained
            }

            // --- Assert
            serv.BusinessExceptionVisited.ShouldBeFalse();
            serv.InfrastructureExceptionVisited.ShouldBeTrue();
        }

        public class SampleEntity
        {
            public int? Id { get; set; }
            public List<string> Names { get; set; }

            public bool RaiseException { get; set; }
        }

        public interface ISampleService : IServiceObject
        {
            void DoOperation(SampleEntity entity);
            bool BusinessExceptionVisited { get; }
            bool InfrastructureExceptionVisited { get; }
        }

        public class SampleService: ServiceObjectBase, ISampleService
        {
            public void DoOperation(SampleEntity entity)
            {
                // --- Check for issues
                Verify.NotNull(entity, "entity");
                Verify.NotNull((object)entity.Id, (SampleEntity e) => e.Id);
                Verify.Require(entity.Id.HasValue, "Id");
                Verify.Require(entity.Names.Count > 0, "Names");
                Verify.RaiseWhenFailed();

                // --- Generate infrastructure exception
                if (entity.RaiseException) throw new InvalidOperationException("Infrastructure");
            }

            public bool BusinessExceptionVisited { get; private set; }
            public bool InfrastructureExceptionVisited { get; private set; }

            protected override Exception OnBusinessException(BusinessOperationException businessEx)
            {
                BusinessExceptionVisited = true;
                return base.OnBusinessException(businessEx);
            }

            protected override Exception OnInfrastructureException(Exception exceptionRaised)
            {
                InfrastructureExceptionVisited = true;
                return base.OnInfrastructureException(exceptionRaised);
            }
        }
    }
}
