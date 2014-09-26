using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.PerformanceCounters;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.PerformanceCounters
{
    [TestClass]
    public class PerformanceCounterCreationDataTest
    {
        [TestMethod]
        public void DataCreationWorksAsExpected()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();

            // --- Act
            pmcData.Add(typeof(TestCounter1));
            pmcData.Add(typeof(TestCounter2));
            pmcData.Add(typeof(TestCounter3));
            pmcData.Add(typeof(TestCounter4));
            pmcData.Add(typeof(TestCounter5));
            pmcData.Add(typeof(TestCounter6));
            var counters1 = pmcData.GetCounters("TestCategory1");
            var counters2 = pmcData.GetCounters("TestCategory2");
            var counters3 = pmcData.GetCounters("TestCategory3");

            // --- Assert
            pmcData.Categories.ShouldHaveCountOf(3);
            pmcData.Categories.ContainsKey("TestCategory1").ShouldBeTrue();
            pmcData.Categories.ContainsKey("TestCategory2").ShouldBeTrue();
            pmcData.Categories.ContainsKey("TestCategory3").ShouldBeTrue();

            counters1.ShouldHaveCountOf(2);
            counters1["TestCounter1"].Name.ShouldEqual("TestCounter1");
            counters1["TestCounter1"].Help.ShouldEqual("TestCounter1 Help");
            counters1["TestCounter1"].Type.ShouldEqual(PerformanceCounterType.NumberOfItems64);

            counters1["TestCounter2"].Name.ShouldEqual("TestCounter2");
            counters1["TestCounter2"].Help.ShouldEqual("TestCounter2 Help");
            counters1["TestCounter2"].Type.ShouldEqual(PerformanceCounterType.NumberOfItems32);

            counters2["TestCounter3"].Name.ShouldEqual("TestCounter3");
            counters2["TestCounter3"].Help.ShouldEqual("");
            counters2["TestCounter3"].Type.ShouldEqual(PerformanceCounterType.NumberOfItems32);

            counters2["TestCounter4"].Name.ShouldEqual("TestCounter4");
            counters2["TestCounter4"].Help.ShouldEqual("TestCounter4 Help");
            counters2["TestCounter4"].Type.ShouldEqual(PerformanceCounterType.ElapsedTime);

            counters3["TestCounter5"].Name.ShouldEqual("TestCounter5");
            counters3["TestCounter5"].Help.ShouldEqual("TestCounter5 Help");
            counters3["TestCounter5"].Type.ShouldEqual(PerformanceCounterType.NumberOfItems32);

            counters3["TestCounter6"].Name.ShouldEqual("TestCounter6");
            counters3["TestCounter6"].Help.ShouldEqual("");
            counters3["TestCounter6"].Type.ShouldEqual(PerformanceCounterType.NumberOfItems32);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DuplicatedCategoryNameRaisesException()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();

            // --- Act
            pmcData.Add(typeof(FaultyCategory1));
            pmcData.Add(typeof(FaultyCategory2));
        }

        [TestMethod]
        public void DuplicatedCategoryNameOnSameCategoryWorks()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();

            // --- Act
            pmcData.Add(typeof(FaultyCategory1));
            pmcData.Add(typeof(FaultyCategory1));
        }

        [TestMethod]
        public void MissingCategoryNameFails()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();

            // --- Act
            try
            {
                pmcData.Add(typeof(FaultyCategory3));
                Assert.Fail();
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException.ShouldBeOfType(typeof (KeyNotFoundException));
            }
        }

        [TestMethod]
        public void MissingCounterNameFails()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();

            // --- Act
            try
            {
                pmcData.Add(typeof(FaultyCounter1));
                Assert.Fail();
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException.ShouldBeOfType(typeof(KeyNotFoundException));
            }
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void DuplicatedCounterNameFails()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();

            // --- Act
            pmcData.Add(typeof (TestCounter1));
            pmcData.Add(typeof (TestCounter1));
        }

        [TestMethod]
        public void CounterInAssemblyProcessedAsExpected()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();
            pmcData.MergeCountersFromAssembly(typeof(PmcCreationData).Assembly);

            // --- Act
            var result = PmcManager.InstallPerformanceCounters(pmcData);

            // --- Assert
            result.InstalledCategories.ShouldHaveCountOf(2);
            result.Errors.ShouldHaveCountOf(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddFailsWithInvalidType()
        {
            // --- Arrange
            var pmcData = new PmcCreationData();

            // --- Act
            pmcData.Add(typeof(int));

        }

        [TestMethod]
        public void ClearWorksAsExpected()
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
            pmcData.Clear();

            // --- Assert
            pmcData.Categories.ShouldHaveCountOf(0);
            
        }

        [PmcCategoryName("TestCategory1")]
        [PmcCategoryHelp("TestCategory1 Help")]
        [PmcCategoryType(PerformanceCounterCategoryType.MultiInstance)]
        [PmcCategoryPredefined(true)]
        class TestCategory1: PmcCategoryDefinitionBase {}

        [PmcCategoryName("TestCategory2")]
        [PmcCategoryHelp("TestCategory2 Help")]
        class TestCategory2 : PmcCategoryDefinitionBase { }

        [PmcCategoryName("TestCategory3")]
        class TestCategory3 : PmcCategoryDefinitionBase { }

        [PmcCategoryName("FaultyCategory")]
        class FaultyCategory1 : PmcCategoryDefinitionBase { }

        [PmcCategoryName("FaultyCategory")]
        class FaultyCategory2 : PmcCategoryDefinitionBase { }

        class FaultyCategory3 : PmcCategoryDefinitionBase { }

        [PmcName("TestCounter1")]
        [PmcHelp("TestCounter1 Help")]
        [PmcType(PerformanceCounterType.NumberOfItems64)]
        class TestCounter1: PmcDefinitionBase<TestCategory1> { }

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

        class FaultyCounter1 : PmcDefinitionBase<TestCategory3> { }
    }
}
