using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DependencyInjection
{
    [TestClass]
    public class InjectedParameterSettingsCollectionTest
    {
        [TestMethod]
        public void ReadAndWriteWorksAsExpected()
        {
            // --- Arrange
            var collection = new InjectedParameterSettingsCollection(
                new List<InjectedParameterSettings>
                    {
                        new InjectedParameterSettings(typeof (int), "12"),
                        new InjectedParameterSettings(typeof (string)),
                        new InjectedParameterSettings(typeof (string), null, true)
                    });

            // --- Act
            var element = collection.WriteToXml("Temp");
            var newColl = new InjectedParameterSettingsCollection(element);

            // --- Assert
            newColl.ShouldHaveCountOf(3);
            newColl[0].Type.ShouldEqual(typeof (int));
            newColl[0].Value.ShouldEqual("12");
            newColl[0].Resolve.ShouldBeFalse();
            newColl[1].Type.ShouldEqual(typeof(string));
            newColl[1].Value.ShouldEqual(null);
            newColl[1].Resolve.ShouldBeFalse();
            newColl[2].Type.ShouldEqual(typeof(string));
            newColl[2].Value.ShouldEqual(null);
            newColl[2].Resolve.ShouldBeTrue();
        }
    }
}
