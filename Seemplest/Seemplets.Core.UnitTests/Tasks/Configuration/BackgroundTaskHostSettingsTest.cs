using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.Tasks;
using Seemplest.Core.Tasks.Configuration;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Tasks.Configuration
{
    [TestClass]
    public class BackgroundTaskHostSettingsTest
    {
        [TestMethod]
        public void ReadAndWriteWorksAsExpected()
        {
            // --- Arrange
            const string PROVIDER_KEY = "queue";
            var defaultProps = new TypedPropertySettingsCollection(
                new List<TypedPropertySettings>
                {
                    new TypedPropertySettings("prop1", "23", typeof (int)),
                    new TypedPropertySettings("prop2", "24", typeof (int)),
                    new TypedPropertySettings("prop3", "23", typeof (string)),
                });
            var defaultContext = new TaskExecutionContextSettings(PROVIDER_KEY, defaultProps);
            var props = new TypedPropertySettingsCollection(
                new List<TypedPropertySettings>
                {
                    new TypedPropertySettings("prop1", "99", typeof (int)),
                    new TypedPropertySettings("prop2", "100", typeof (int)),
                    new TypedPropertySettings("prop3", "101", typeof (string)),
                });
            var optionalContext = new TaskExecutionContextSettings(PROVIDER_KEY, props);
            var taskProps = new PropertySettingsCollection(
                new List<PropertySettings>
                {
                    new PropertySettings("prop1", "1000"),
                    new PropertySettings("prop2", "1001"),
                });
            var proc1 = new TaskProcessorSettings("Proc1", TaskProcessorSettings.SCHEDULED_TYPE, typeof(TaskBase), 3,
                                                  optionalContext, taskProps);
            var proc2 = new TaskProcessorSettings("Proc2", TaskProcessorSettings.QUEUED_TYPE, typeof(TaskBase), 2,
                                                  null, taskProps);
            var proc3 = new TaskProcessorSettings("Proc3", TaskProcessorSettings.DOUBLE_QUEUED, typeof(TaskBase), 1,
                                                  null);

            var host = new BackgroundTaskHostSettings(defaultContext, new List<TaskProcessorSettings> { proc1, proc2, proc3 });

            // --- Act
            var element = host.WriteToXml("BackgroundTaskHost");
            Console.WriteLine(element);
            var config = new BackgroundTaskHostSettings(element);

            // --- Assert
            config.DefaultContext.ShouldNotBeNull();
            config.DefaultContext.ProviderKey.ShouldEqual(PROVIDER_KEY);
            config.DefaultContext.Properties.ShouldHaveCountOf(3);
            config.DefaultContext.Properties["prop1"].Type.ShouldEqual(typeof(int));
            config.DefaultContext.Properties["prop1"].Value.ShouldEqual("23");
            config.DefaultContext.Properties["prop2"].Type.ShouldEqual(typeof(int));
            config.DefaultContext.Properties["prop2"].Value.ShouldEqual("24");
            config.DefaultContext.Properties["prop3"].Type.ShouldEqual(typeof(string));
            config.DefaultContext.Properties["prop3"].Value.ShouldEqual("23");

            var taskProcessors = config.GetTaskProcessors();
            taskProcessors.ShouldHaveCountOf(3);
            taskProcessors[0].Name.ShouldEqual("Proc1");
            taskProcessors[0].ProcessorType.ShouldEqual(TaskProcessorSettings.SCHEDULED_TYPE);
            taskProcessors[0].TaskType.ShouldEqual(typeof(TaskBase));
            taskProcessors[0].InstanceCount.ShouldEqual(3);
            taskProcessors[0].Context.ShouldNotBeNull();
            taskProcessors[0].Context.Properties.ShouldHaveCountOf(3);
            taskProcessors[0].Context.Properties["prop1"].Type.ShouldEqual(typeof(int));
            taskProcessors[0].Context.Properties["prop1"].Value.ShouldEqual("99");
            taskProcessors[0].Context.Properties["prop2"].Type.ShouldEqual(typeof(int));
            taskProcessors[0].Context.Properties["prop2"].Value.ShouldEqual("100");
            taskProcessors[0].Context.Properties["prop3"].Type.ShouldEqual(typeof(string));
            taskProcessors[0].Context.Properties["prop3"].Value.ShouldEqual("101");
            taskProcessors[0].Properties.ShouldHaveCountOf(2);
            taskProcessors[0].Properties["prop1"].Value.ShouldEqual("1000");
            taskProcessors[0].Properties["prop2"].Value.ShouldEqual("1001");

            taskProcessors[1].Name.ShouldEqual("Proc2");
            taskProcessors[1].ProcessorType.ShouldEqual(TaskProcessorSettings.QUEUED_TYPE);
            taskProcessors[1].TaskType.ShouldEqual(typeof(TaskBase));
            taskProcessors[1].InstanceCount.ShouldEqual(2);
            taskProcessors[1].Context.ShouldBeNull();
            taskProcessors[1].Properties.ShouldHaveCountOf(2);
            taskProcessors[1].Properties["prop1"].Value.ShouldEqual("1000");
            taskProcessors[1].Properties["prop2"].Value.ShouldEqual("1001");

            taskProcessors[2].Name.ShouldEqual("Proc3");
            taskProcessors[2].ProcessorType.ShouldEqual(TaskProcessorSettings.DOUBLE_QUEUED);
            taskProcessors[2].TaskType.ShouldEqual(typeof(TaskBase));
            taskProcessors[2].InstanceCount.ShouldEqual(1);
            taskProcessors[2].Context.ShouldBeNull();
            taskProcessors[2].Properties.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void ReadAndWriteWorksWithConfigFileAsExpected()
        {
            // --- Arrange
            const string PROVIDER_KEY = "queue";

            // --- Act
            var config = AppConfigurationManager.GetSettings<BackgroundTaskHostSettings>("BackgroundTaskHost1");

            // --- Assert
            config.DefaultContext.ShouldNotBeNull();
            config.DefaultContext.ProviderKey.ShouldEqual(PROVIDER_KEY);
            config.DefaultContext.Properties.ShouldHaveCountOf(3);
            config.DefaultContext.Properties["prop1"].Type.ShouldEqual(typeof(int));
            config.DefaultContext.Properties["prop1"].Value.ShouldEqual("23");
            config.DefaultContext.Properties["prop2"].Type.ShouldEqual(typeof(int));
            config.DefaultContext.Properties["prop2"].Value.ShouldEqual("24");
            config.DefaultContext.Properties["prop3"].Type.ShouldEqual(typeof(string));
            config.DefaultContext.Properties["prop3"].Value.ShouldEqual("23");

            var taskProcessors = config.GetTaskProcessors();

            taskProcessors.ShouldHaveCountOf(3);
            taskProcessors[0].Name.ShouldEqual("Proc1");
            taskProcessors[0].ProcessorType.ShouldEqual(TaskProcessorSettings.SCHEDULED_TYPE);
            taskProcessors[0].TaskType.ShouldEqual(typeof(TaskBase));
            taskProcessors[0].InstanceCount.ShouldEqual(3);
            taskProcessors[0].Context.ShouldNotBeNull();
            taskProcessors[0].Context.Properties.ShouldHaveCountOf(3);
            taskProcessors[0].Context.Properties["prop1"].Type.ShouldEqual(typeof(int));
            taskProcessors[0].Context.Properties["prop1"].Value.ShouldEqual("99");
            taskProcessors[0].Context.Properties["prop2"].Type.ShouldEqual(typeof(int));
            taskProcessors[0].Context.Properties["prop2"].Value.ShouldEqual("100");
            taskProcessors[0].Context.Properties["prop3"].Type.ShouldEqual(typeof(string));
            taskProcessors[0].Context.Properties["prop3"].Value.ShouldEqual("101");
            taskProcessors[0].Properties.ShouldHaveCountOf(2);
            taskProcessors[0].Properties["prop1"].Value.ShouldEqual("1000");
            taskProcessors[0].Properties["prop2"].Value.ShouldEqual("1001");

            taskProcessors[1].Name.ShouldEqual("Proc2");
            taskProcessors[1].ProcessorType.ShouldEqual(TaskProcessorSettings.QUEUED_TYPE);
            taskProcessors[1].TaskType.ShouldEqual(typeof(TaskBase));
            taskProcessors[1].InstanceCount.ShouldEqual(2);
            taskProcessors[1].Context.ShouldBeNull();
            taskProcessors[1].Properties.ShouldHaveCountOf(2);
            taskProcessors[1].Properties["prop1"].Value.ShouldEqual("1000");
            taskProcessors[1].Properties["prop2"].Value.ShouldEqual("1001");

            taskProcessors[2].Name.ShouldEqual("Proc3");
            taskProcessors[2].ProcessorType.ShouldEqual(TaskProcessorSettings.DOUBLE_QUEUED);
            taskProcessors[2].TaskType.ShouldEqual(typeof(TaskBase));
            taskProcessors[2].InstanceCount.ShouldEqual(1);
            taskProcessors[2].Context.ShouldBeNull();
            taskProcessors[2].Properties.ShouldHaveCountOf(0);
        }
    }
}
