using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.PerformanceCounters;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.PerformanceCounters
{
    [TestClass]
    public class PerformanceCounterHandleTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var counters = new PmcCreationData();
            counters.Add(typeof(TestCounter2));
            PmcManager.InstallPerformanceCounters(counters);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var counters = new PmcCreationData();
            counters.Add(typeof(TestCounter2));
            PmcManager.RemovePerformanceCounters(counters);
        }

        [TestMethod]
        public void SingleInstanceCreationWorks()
        {
            // --- Act
            var handle = new PerformanceCounterHandle(typeof (TestCounter1));

            // --- Assert
            handle.ShouldNotBeNull();
            handle.HasInstance.ShouldBeFalse();
            handle.InstanceName.ShouldBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidCounterTypeFails()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new PerformanceCounterHandle(typeof(int));
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void CounterOperationWorkWithPhysicalCounter()
        {
            // --- Arrange
            // ReSharper disable UseObjectOrCollectionInitializer
            var handle = new PerformanceCounterHandle(typeof(TestCounter2));
            // ReSharper restore UseObjectOrCollectionInitializer

            // --- Act
            handle.RawValue = 100;
            handle.Increment();
            var value1 = handle.RawValue;
            handle.IncrementBy(2);
            var value2 = handle.RawValue;
            handle.Decrement();
            var value3 = handle.RawValue;
            var sample = handle.NextSample();
            var nextValue = handle.NextValue();
            handle.ReadOnly = true;

            // --- Assert
            value1.ShouldEqual(101);
            value2.ShouldEqual(103);
            value3.ShouldEqual(102);
            sample.ShouldNotEqual(CounterSample.Empty);
            nextValue.ShouldNotEqual(0.0F);
            handle.ReadOnly.ShouldBeTrue();
        }

        [TestMethod]
        public void CounterOperationWorkWithVirtualCounter()
        {
            // --- Arrange
            // ReSharper disable UseObjectOrCollectionInitializer
            var handle = new PerformanceCounterHandle(typeof(TestCounter1));
            // ReSharper restore UseObjectOrCollectionInitializer

            // --- Act
            handle.RawValue = 100;
            handle.Increment();
            var value1 = handle.RawValue;
            handle.IncrementBy(2);
            var value2 = handle.RawValue;
            handle.Decrement();
            var value3 = handle.RawValue;
            var sample = handle.NextSample();
            var nextValue = handle.NextValue();
            handle.ReadOnly = true;

            // --- Assert
            value1.ShouldEqual(0);
            value2.ShouldEqual(0);
            value3.ShouldEqual(0);
            sample.ShouldEqual(CounterSample.Empty);
            nextValue.ShouldEqual(0.0F);
            handle.ReadOnly.ShouldBeFalse();
        }

        [PmcCategoryName("TestCategory1")]
        [PmcCategoryHelp("TestCategory1 Help")]
        internal class TestCategory1 : PmcCategoryDefinitionBase { }

        [PmcName("TestCounter1")]
        [PmcHelp("TestCounter1 Help")]
        [PmcType(PerformanceCounterType.NumberOfItems64)]
        internal class TestCounter1 : PmcDefinitionBase<TestCategory1> { }

        [PmcCategoryName("TestCategory2")]
        [PmcCategoryHelp("TestCategory2 Help")]
        internal class TestCategory2 : PmcCategoryDefinitionBase { }

        [PmcName("TestCounter2")]
        [PmcHelp("TestCounter2 Help")]
        [PmcType(PerformanceCounterType.NumberOfItems64)]
        internal class TestCounter2 : PmcDefinitionBase<TestCategory2> { }


    }
}
