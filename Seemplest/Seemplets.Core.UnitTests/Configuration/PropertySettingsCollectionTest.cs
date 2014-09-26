using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Configuration
{
    [TestClass]
    public class PropertySettingsCollectionTest
    {
        [TestMethod]
        public void ContructorWithCollectionInitializerWorks()
        {
            // --- Act
            var collection = new PropertySettingsCollection(
                new List<PropertySettings>
                    {
                        new PropertySettings("Prop1", "2"),
                        new PropertySettings("Prop2", "hello")
                    });

            // --- Assert
            collection.ShouldHaveCountOf(2);
            collection[0].Name.ShouldEqual("Prop1");
            collection[0].Value.ShouldEqual("2");
            collection[1].Name.ShouldEqual("Prop2");
            collection[1].Value.ShouldEqual("hello");

            collection.Count.ShouldEqual(2);
            collection["Prop1"].Value.ShouldEqual("2");
            collection["Prop2"].Value.ShouldEqual("hello");
        }

        [TestMethod]
        public void EmptyCollectionResultsEmptyProperties()
        {
            // --- Act
            var collection = new PropertySettingsCollection();

            // --- Assert
            collection.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void ConstructorWorksWithNullRootName()
        {
            // --- Act
            var collection1 = new PropertySettingsCollection(
                new List<PropertySettings>
                    {
                        new PropertySettings("Prop1", "2"),
                        new PropertySettings("Prop2", "hello")
                    });
            var element1 = collection1.WriteToXml("Test");

            var collection2 = new PropertySettingsCollection();
            collection2.ReadFromXml(element1);

            // --- Assert
            element1.Elements("Property").ShouldHaveCountOf(2);
            collection2.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void ConstructorWorksWithXElement1()
        {
            // --- Act
            var collection1 = new PropertySettingsCollection(
                new List<PropertySettings>
                    {
                        new PropertySettings("Prop1", "2"),
                        new PropertySettings("Prop2", "hello")
                    });
            var element1 = collection1.WriteToXml("Test");
            var collection2 = new PropertySettingsCollection(element1);

            // --- Assert
            element1.Elements("Property").ShouldHaveCountOf(2);
            collection2.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void ConstructorWorksWithXElement2()
        {
            // --- Act
            var collection1 = new PropertySettingsCollection(
                new List<PropertySettings>
                    {
                        new PropertySettings(new PropertySettings("Prop1", "2").WriteToXml("temp")),
                        new PropertySettings(new PropertySettings("Prop2", "Hello").WriteToXml("temp")),
                    });
            var element1 = collection1.WriteToXml("Test");
            var collection2 = new PropertySettingsCollection(element1);

            // --- Assert
            element1.Elements("Property").ShouldHaveCountOf(2);
            collection2.ShouldHaveCountOf(2);
        }
    }
}
