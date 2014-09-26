﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.Exceptions;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class DataRecordTest
    {
        [TestMethod]
        public void MutableDataRecordCanBeModified()
        {
            // --- Act
            var record = new Record1 {Id = 123, DisplayName = "Hello"};
            var dataRecord = (IDataRecord)record;

            // --- Assert
            record.Id.ShouldEqual(123);
            record.DisplayName.ShouldEqual("Hello");
            dataRecord.IsModified("Id").ShouldBeFalse();
            dataRecord.IsModified("DisplayName").ShouldBeFalse();
        }

        [TestMethod]
        public void ModifiedPropertiesAreMarked()
        {
            // --- Act
            var record = new Record1 { Id = 123, DisplayName = "Hello" };
            var dataRecord = (IDataRecord)record;
            dataRecord.SignLoaded();
            record.Id = 234;

            // --- Assert
            record.Id.ShouldEqual(234);
            record.DisplayName.ShouldEqual("Hello");
            dataRecord.IsModified("Id").ShouldBeTrue();
            dataRecord.IsModified("DisplayName").ShouldBeFalse();
        }

        [TestMethod]
        public void ImmutableDataRecordCanBeModifiedDuringLoad()
        {
            // --- Act
            var record = new Record2 { Id = 123, DisplayName = "Hello" };
            var dataRecord = (IDataRecord)record;

            // --- Assert
            record.Id.ShouldEqual(123);
            record.DisplayName.ShouldEqual("Hello");
            dataRecord.IsModified("Id").ShouldBeFalse();
            dataRecord.IsModified("DisplayName").ShouldBeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(ImmutableRecordChangedException))]
        public void ImmutableDataRecordCannotBeModified()
        {
            // --- Arrange
            var record = new Record2 { Id = 123, DisplayName = "Hello" };
            var dataRecord = (IDataRecord)record;

            // --- Act
            dataRecord.SignLoaded();
            dataRecord.SetImmutable();
            
            record.DisplayName = "Hi";
        }

        [TestMethod]
        public void CloneWorksAsExpected()
        {
            // --- Arrange
            var original = new Record1 { Id = 23, DisplayName = "Original" };

            // --- Act
            var clone = ((ICloneableRecord<Record1>) original).Clone();

            // ---
            clone.ShouldBeOfType(typeof (Record1));
            clone.ShouldNotBeSameAs(original);
            clone.Id.ShouldEqual(original.Id);
            clone.DisplayName.ShouldEqual(original.DisplayName);
        }

        [TableName("Record1")]
        public class Record1 : DataRecord<Record1>
        {
            private int _id;
            private string _displayName;

            [AutoGenerated]
            public int Id 
            {
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            public string DisplayName
            {
                get { return _displayName; }
                set { _displayName = Modify(value, "DisplayName"); }
            }
        }

        [ImmutableRecord]
        public class Record2 : DataRecord<Record2>
        {
            private int _id;
            private string _displayName;

            [AutoGenerated]
            public int Id
            {
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            public string DisplayName
            {
                get { return _displayName; }
                set { _displayName = Modify(value, "DisplayName"); }
            }
        }
    }
}
