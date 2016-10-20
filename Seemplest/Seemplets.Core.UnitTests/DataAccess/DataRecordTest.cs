using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.DataAccess
{
    [TestClass]
    public class DataRecordTest
    {
        [TestMethod]
        public void MergeChangesFromWorksAsExpected1()
        {
            // --- Arrange
            var baseRec = new TestRecord();
            var newRec = new TestRecord
            {
                IntField = 23,
                StringField = "Hi!",
                BoolField = true
            };

            // --- Act
            baseRec.MergeChangesFrom(newRec);

            // --- Assert
            var modCols = baseRec.GetModifiedColumns();
            modCols.ShouldHaveCountOf(3);
            modCols.ShouldContain("IntField");
            modCols.ShouldContain("StringField");
            modCols.ShouldContain("BoolField");
            modCols.ShouldNotContain("DateTimeField");

            baseRec.IntField.ShouldEqual(23);
            baseRec.StringField.ShouldEqual("Hi!");
            baseRec.DateTimeField.ShouldBeNull();
            baseRec.BoolField.ShouldBeTrue();
        }

        [TestMethod]
        public void MergeChangesFromWorksAsExpected2()
        {
            // --- Arrange
            var baseRec = new TestRecord
            {
                IntField = 23,
                StringField = "Hi!",
                BoolField = true
            };
            var newRec = new TestRecord
            {
                IntField = 23,
                StringField = "Hi!",
                BoolField = true
            };

            // --- Act
            baseRec.MergeChangesFrom(newRec);

            // --- Assert
            var modCols = baseRec.GetModifiedColumns();
            modCols.ShouldHaveCountOf(0);

            baseRec.IntField.ShouldEqual(23);
            baseRec.StringField.ShouldEqual("Hi!");
            baseRec.DateTimeField.ShouldBeNull();
            baseRec.BoolField.ShouldBeTrue();
        }

        [TestMethod]
        public void MergeChangesFromWorksAsExpected3()
        {
            // --- Arrange
            var baseRec = new TestRecord
            {
                IntField = 23,
                StringField = "Hi!",
                BoolField = true,
                DateTimeField = DateTime.Now
            };
            var newRec = new TestRecord
            {
                IntField = 23,
                StringField = "Hi!",
                BoolField = true
            };

            // --- Act
            baseRec.MergeChangesFrom(newRec);

            // --- Assert
            var modCols = baseRec.GetModifiedColumns();
            modCols.ShouldHaveCountOf(1);
            modCols.ShouldContain("DateTimeField");

            baseRec.IntField.ShouldEqual(23);
            baseRec.StringField.ShouldEqual("Hi!");
            baseRec.DateTimeField.ShouldBeNull();
            baseRec.BoolField.ShouldBeTrue();
        }

        [TestMethod]
        public void MergeChangesFromWorksAsExpected4()
        {
            // --- Arrange
            var baseRec = new TestRecord
            {
                IntField = 23,
                StringField = "Hi!",
                BoolField = true,
            };
            var newRec = new TestRecord
            {
                IntField = 23,
                StringField = "Hi!",
                BoolField = false,
                DateTimeField = DateTime.Now
            };

            // --- Act
            baseRec.MergeChangesFrom(newRec);

            // --- Assert
            var modCols = baseRec.GetModifiedColumns();
            modCols.ShouldHaveCountOf(2);
            modCols.ShouldContain("BoolField");
            modCols.ShouldContain("DateTimeField");

            baseRec.IntField.ShouldEqual(23);
            baseRec.StringField.ShouldEqual("Hi!");
            baseRec.DateTimeField.ShouldNotBeNull();
            baseRec.BoolField.ShouldBeFalse();
        }

        [TableName("Dummy")]
        class TestRecord : DataRecord<TestRecord>
        {
            private int _intField;
            private string _stringField;
            private DateTime? _dateTimeField;
            private bool _boolField;

            public int IntField
            {
                get { return _intField; }
                set { _intField = Modify(value); }
            }

            public string StringField
            {
                get { return _stringField; }
                set { _stringField = Modify(value); }
            }

            public DateTime? DateTimeField
            {
                get { return _dateTimeField; }
                set { _dateTimeField = Modify(value); }
            }

            public bool BoolField
            {
                get { return _boolField; }
                set { _boolField = Modify(value); }
            }
        }
    }
}
