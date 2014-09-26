using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Cache;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Cache
{
    [TestClass]
    public class TimeConstrainedCacheTest
    {
        [TestMethod]
        public void ResetWorksAsExpected()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();

            // --- Act
            cache.ResetTo(TimeSpan.FromSeconds(15));

            // --- Assert
            cache.Count.ShouldEqual(0);
            cache.ExpirationTimeSpan.ShouldEqual(TimeSpan.FromSeconds(15));
        }

        [TestMethod]
        public void ConstructorWithExpirationTimeSpanWorksAsExpected()
        {
            // --- Act
            var cache = new TimeConstrainedCache<int, string>(TimeSpan.FromMilliseconds(123));

            // --- Assert
            cache.Count.ShouldEqual(0);
            cache.ExpirationTimeSpan.ShouldEqual(TimeSpan.FromMilliseconds(123));
        }

        [TestMethod]
        public void ContainsWorksAsExpected()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");

            // --- Act
            var contains1 = cache.Contains(1);
            var contains2 = cache.Contains(2);
            var contains3 = cache.Contains(3);

            // --- Assert
            contains1.ShouldBeTrue();
            contains2.ShouldBeTrue();
            contains3.ShouldBeFalse();
        }

        [TestMethod]
        public void GetValueWorksAsExpected()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");

            // --- Act
            var value = cache.GetValue(2);

            // --- Assert
            value.ShouldEqual("two");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetValueFailsWithNonExistingItem()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");

            // --- Act
            cache.GetValue(3);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetValueFailsWithExpiredItem()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>(TimeSpan.FromMilliseconds(100));
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");
            Thread.Sleep(200);

            // --- Act
            cache.GetValue(2);
        }

        [TestMethod]
        public void RemoveWorksAsExpected()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");

            // --- Act
            var value = cache.GetValue(2);
            cache.Remove(2);

            // --- Assert
            value.ShouldEqual("two");
            cache.Contains(2).ShouldBeFalse();
        }

        [TestMethod]
        public void TryGetValueWorksAsExpected()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");

            // --- Act
            string value;
            var found = cache.TryGetValue(2, out value);

            // --- Assert
            found.ShouldBeTrue();
            value.ShouldEqual("two");
        }

        [TestMethod]
        public void TryGetValueRecognizesNonExistingItem()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");

            // --- Act
            string value;
            var found = cache.TryGetValue(3, out value);

            // --- Assert
            found.ShouldBeFalse();
        }

        [TestMethod]
        public void TryGetValueRecognizesWithExpiredItem()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>(TimeSpan.FromMilliseconds(100));
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");
            Thread.Sleep(200);

            // --- Act
            string value;
            var found = cache.TryGetValue(2, out value);

            // --- Assert
            found.ShouldBeFalse();
        }

        [TestMethod]
        public void ClearWorksAsExpected()
        {
            // --- Arrange
            var cache = new TimeConstrainedCache<int, string>();
            cache.SetValue(1, "one");
            cache.SetValue(2, "two");

            // --- Act
            cache.Clear();

            // --- Assert
            cache.Count.ShouldEqual(0);
        }

        [TestMethod]
        public void DisposeWorksAsExpected()
        {
            // --- Act
            // ReSharper disable once UnusedVariable
            using (var cache = new TimeConstrainedCache<int, string>())
            {
            }
        }


    }
}
