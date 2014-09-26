using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration
{
    [TestClass]
    public class ConstructorParameterSettingsCollectionTest
    {
        [TestMethod]
        public void ContructorWithCollectionInitializerWorks()
        {
            // --- Act
            var collection = new ConstructorParameterSettingsCollection(
                new List<ConstructorParameterSettings>
                    {
                        new ConstructorParameterSettings(typeof(int), "2"),
                        new ConstructorParameterSettings(typeof(string), "hello")
                    });

            // --- Assert
            collection.ShouldHaveCountOf(2);
            collection[0].Type.ShouldEqual(typeof (int));
            collection[0].Value.ShouldEqual("2");
            collection[1].Type.ShouldEqual(typeof (string));
            collection[1].Value.ShouldEqual("hello");
        }

        [TestMethod]
        public void ConstructorWorksWithNullRootName()
        {
            // --- Act
            var collection1 = new ConstructorParameterSettingsCollection(
                new List<ConstructorParameterSettings>
                    {
                        new ConstructorParameterSettings(typeof (int), "2"),
                        new ConstructorParameterSettings(typeof (string), "hello")
                    });
            var element1 = collection1.WriteToXml("Test");

            var collection2 = new ConstructorParameterSettingsCollection();
            collection2.ReadFromXml(element1);

            // --- Assert
            element1.Elements("Param").ShouldHaveCountOf(2);
            collection2.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void ConstructorWorksWithXElement()
        {
            // --- Act
            var collection1 = new ConstructorParameterSettingsCollection(
                new List<ConstructorParameterSettings>
                    {
                        new ConstructorParameterSettings(typeof (int), "2"),
                        new ConstructorParameterSettings(typeof (string), "hello")
                    });
            var element1 = collection1.WriteToXml("Test");

            var collection2 = new ConstructorParameterSettingsCollection(element1);

            // --- Assert
            element1.Elements("Param").ShouldHaveCountOf(2);
            collection2.ShouldHaveCountOf(2);
        }
    }
}
