using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.ServiceObjects;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.ServiceObjects
{
    [TestClass]
    public class ServiceCallContextTest
    {
        [TestMethod]
        public void SetItemAndGetItemWorksAsExpected()
        {
            // --- Arrange
            var context = new ServiceCallContext();
            context.Clear();

            // --- Act
            context.Set(new IntContextItem(123));
            var item1 = context.Get<IntContextItem>();
            context.Set(new StringContextItem("hello"));
            var item2 = context.Get<StringContextItem>();
            var item3 = context.Get<EmptyContextItem>();

            // --- Assert
            item1.ShouldNotBeNull();
            item1.Value.ShouldEqual(123);
            var contextItem = item1 as IServiceCallContextItem;
            contextItem.ShouldNotBeNull();
            contextItem.GetValue().ShouldEqual(123);
            item2.ShouldNotBeNull();
            item2.Value.ShouldEqual("hello");
            item3.ShouldBeNull();
        }

        [TestMethod]
        public void RemoveWorksAsExpected()
        {
            // --- Arrange
            var context = new ServiceCallContext();
            context.Clear();
            context.Set(new IntContextItem(123));

            // --- Act
            var item1 = context.Get<IntContextItem>();
            context.Remove<IntContextItem>();
            var item2 = context.Get<IntContextItem>();

            // --- Assert
            item1.ShouldNotBeNull();
            item2.ShouldBeNull();
        }

        [TestMethod]
        public void SetByKeyAndGetByKeyWorksAsExpected()
        {
            // --- Arrange
            var context = new ServiceCallContext();
            context.Clear();

            // --- Act
            context.SetByKey("key1", 123);
            var item1 = context.GetByKey("key1");
            context.SetByKey("key2", "hello");
            var item2 = context.GetByKey("key2");
            var item3 = context.GetByKey("key3");

            // --- Assert
            item1.ShouldNotBeNull();
            item1.ShouldEqual(123);
            item2.ShouldNotBeNull();
            item2.ShouldEqual("hello");
            item3.ShouldBeNull();
        }

        [TestMethod]
        public void RemoveByKeyWorksAsExpected()
        {
            // --- Arrange
            var context = new ServiceCallContext();
            context.Clear();
            context.SetByKey("key1", 123);

            // --- Act
            var item1 = context.GetByKey("key1");
            context.RemoveByKey("key1");
            var item2 = context.GetByKey("key1");

            // --- Assert
            item1.ShouldNotBeNull();
            item2.ShouldBeNull();
        }

        [TestMethod]
        public void CloneWorksAsExpected()
        {
            // --- Arrange
            var context = new ServiceCallContext();
            context.Set(new IntContextItem(123));
            context.Set(new StringContextItem("hello"));

            // --- Act
            var clone = (context as ICloneable).Clone() as ServiceCallContext;

            // --- Assert
            clone.ShouldNotBeNull();
            clone.ShouldNotBeSameAs(context);
            // ReSharper disable PossibleNullReferenceException
            clone.Get<IntContextItem>().Value.ShouldEqual(123);
            // ReSharper restore PossibleNullReferenceException
            clone.Get<StringContextItem>().Value.ShouldEqual("hello");
        }

        class IntContextItem : ServiceCallContextItemBase<int>
        {
            public IntContextItem(int itemValue) : base(itemValue)
            {
            }
        }

        class StringContextItem : ServiceCallContextItemBase<string>
        {
            public StringContextItem(string itemValue) : base(itemValue)
            {
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class EmptyContextItem : ServiceCallContextItemBase<string>
        // ReSharper restore ClassNeverInstantiated.Local
        {
            public EmptyContextItem(string itemValue)
                : base(itemValue)
            {
            }
        }
    }
}
