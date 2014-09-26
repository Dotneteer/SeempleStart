using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.PerformanceCounters;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.PerformanceCounters
{
    [TestClass]
    public class PerformanceCounterManagerTest
    {
        [TestMethod]
        public void InstallWorksAsExpected()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();
            pmcData.Add(typeof(TestCounter1));
            pmcData.Add(typeof(TestCounter2));
            pmcData.Add(typeof(TestCounter3));
            pmcData.Add(typeof(TestCounter4));
            pmcData.Add(typeof(TestCounter5));
            pmcData.Add(typeof(TestCounter6));

            // --- Act
            var results = PmcManager.InstallPerformanceCounters(pmcData);

            // --- Assert
            results.InstalledCategories.ShouldHaveCountOf(3);
            results.Errors.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void RemoveWorksAsExpected()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();
            pmcData.Add(typeof(TestCounter1));
            pmcData.Add(typeof(TestCounter2));
            pmcData.Add(typeof(TestCounter3));
            pmcData.Add(typeof(TestCounter4));
            pmcData.Add(typeof(TestCounter5));
            pmcData.Add(typeof(TestCounter6));

            // --- Act
            var installResults = PmcManager.InstallPerformanceCounters(pmcData);
            var removeResults = PmcManager.RemovePerformanceCounters(pmcData);

            // --- Assert
            installResults.InstalledCategories.ShouldHaveCountOf(3);
            installResults.Errors.ShouldHaveCountOf(0);
            removeResults.InstalledCategories.ShouldHaveCountOf(3);
            removeResults.Errors.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void PredefinedCountersWork()
        {
            // --- Act
            var counterInstance1 = PmcManager.GetCounter<ProcessorTimePercentagePmc>("_Total");
            var counterInstance2 = PmcManager.GetCounter<ProcessorTimePercentagePmc>("_Total");
            var value = counterInstance1.RawValue;

            // --- Assert
            counterInstance1.ShouldBeSameAs(counterInstance2);
            counterInstance1.HasInstance.ShouldBeTrue();
            value.ShouldBeGreaterThanOrEqualTo(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PredefinedCountersWithInvalidInstanceNameFail()
        {
            // --- Act
            var counterInstance = PmcManager.GetCounter<ProcessorTimePercentagePmc>("x");
            // ReSharper disable UnusedVariable
            var value = counterInstance.RawValue;
            // ReSharper restore UnusedVariable
        }

        [TestMethod]
        public void MultipleInstanceWork()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();
            pmcData.Add(typeof(TestCounter1));
            pmcData.Add(typeof(TestCounter2));
            PmcManager.InstallPerformanceCounters(pmcData);

            // --- Act
            var inst1 = PmcManager.GetCounter<TestCounter1>("1");
            var inst2 = PmcManager.GetCounter<TestCounter1>("2");
            inst1.RawValue = 10;
            inst2.RawValue = 20;

            // --- Assert
            inst1.RawValue.ShouldEqual(10);
            inst2.RawValue.ShouldEqual(20);
        }

        [TestMethod]
        public void BuiltInCountersWork()
        {
            // --- Act
            var counter = PmcManager.GetCounter<ProcessorTimePercentagePmc>();
            var sample1 = counter.NextSample();
            Thread.Sleep(400);
            var sample2 = counter.NextSample();
            var value = CounterSample.Calculate(sample1, sample2);
            Console.WriteLine(value);

            // --- Assert
            value.ShouldBeGreaterThanOrEqualTo(0.0F);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UsingInstallWithNullDataFails()
        {
            // --- Act
            PmcManager.InstallPerformanceCounters(null);
        }

        [TestMethod]
        public void UsingInstallWithWrongCounterFails()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();
            pmcData.Add(typeof(TestCounter7));

            // --- Act
            var result = PmcManager.InstallPerformanceCounters(pmcData);

            // --- Assert
            result.Errors.ShouldHaveCountOf(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UsingRemoveWithNullDataFails()
        {
            // --- Act
            PmcManager.RemovePerformanceCounters(null);
        }

        [TestMethod]
        public void UsingRemoveWithWrongCounterFails()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();
            pmcData.Add(typeof(TestProcessorTimePmc));

            // --- Act
            var result = PmcManager.RemovePerformanceCounters(pmcData);

            // --- Assert
            result.Errors.ShouldHaveCountOf(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UsingGetCounterWithNullDataFails()
        {
            // --- Act
            PmcManager.GetCounter(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UsingGetCounterWithInvalidTypeFails()
        {
            // --- Act
            PmcManager.GetCounter(typeof(int));
        }

        [PmcCategoryName("TestCategory1")]
        [PmcCategoryHelp("TestCategory1 Help")]
        [PmcCategoryType(PerformanceCounterCategoryType.MultiInstance)]
        class TestCategory1 : PmcCategoryDefinitionBase { }

        [PmcCategoryName("TestCategory2")]
        [PmcCategoryHelp("TestCategory2 Help")]
        class TestCategory2 : PmcCategoryDefinitionBase { }

        [PmcCategoryName("TestCategory3")]
        class TestCategory3 : PmcCategoryDefinitionBase { }

        [PmcCategoryName("TestC\\ategory4")]
        class TestCategory4 : PmcCategoryDefinitionBase { }

        [PmcName("TestCounter1")]
        [PmcHelp("TestCounter1 Help")]
        [PmcType(PerformanceCounterType.NumberOfItems64)]
        class TestCounter1 : PmcDefinitionBase<TestCategory1> { }

        [PmcName("TestCounter2")]
        [PmcHelp("TestCounter2 Help")]
        class TestCounter2 : PmcDefinitionBase<TestCategory1> { }

        [PmcName("TestCounter3")]
        class TestCounter3 : PmcDefinitionBase<TestCategory2> { }

        [PmcName("TestCounter4")]
        [PmcHelp("TestCounter4 Help")]
        [PmcType(PerformanceCounterType.ElapsedTime)]
        class TestCounter4 : PmcDefinitionBase<TestCategory2> { }

        [PmcName("TestCounter5")]
        [PmcHelp("TestCounter5 Help")]
        class TestCounter5 : PmcDefinitionBase<TestCategory3> { }

        [PmcName("TestCounter6")]
        class TestCounter6 : PmcDefinitionBase<TestCategory3> { }

        [PmcName("TestCounter7")]
        [PmcType(PerformanceCounterType.AverageCount64)]
        class TestCounter7 : PmcDefinitionBase<TestCategory4> { }

        [PmcCategoryName("Processor")]
        [PmcCategoryType(PerformanceCounterCategoryType.MultiInstance)]
        public sealed class TestProcessorCategory : PmcCategoryDefinitionBase { }

        [PmcName("% Processor Time")]
        [PmcDefaultInstance("_Total")]
        public sealed class TestProcessorTimePmc : PmcDefinitionBase<TestProcessorCategory> { }
    }
}
