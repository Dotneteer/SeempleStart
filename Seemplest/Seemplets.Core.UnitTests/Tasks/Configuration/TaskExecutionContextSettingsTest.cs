using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.Tasks.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Tasks.Configuration
{
    [TestClass]
    public class TaskExecutionContextSettingsTest
    {
        [TestMethod]
        public void ReadAnWriteWorksAsExpected()
        {
            // --- Arrange
            const string PROVIDER_KEY = "queue";
            var properties = new TypedPropertySettingsCollection(
                new List<TypedPropertySettings>
                {
                    new TypedPropertySettings("prop1", "23", typeof (int)),
                    new TypedPropertySettings("prop2", "24", typeof (int)),
                    new TypedPropertySettings(new TypedPropertySettings("prop3", "23", typeof (string)).WriteToXml("temp")),
                });
            var settings = new TaskExecutionContextSettings(PROVIDER_KEY, properties);

            // --- Act
            var element = settings.WriteToXml("temp");
            var config = new TaskExecutionContextSettings(element);

            // --- Assert
            config.ProviderKey.ShouldEqual(PROVIDER_KEY);
            config.Properties.ShouldHaveCountOf(3);
        }
    }
}
