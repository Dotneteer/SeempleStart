using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.MsSql.DataAccess;

namespace Seemplest.MsSql.UnitTests.DataAccess.Regression
{
    [TestClass]
    public class SqlDatabaseRegressionTest
    {
        const string DB_CONN = "connStr=Seemplest";
        [TestMethod]
        public void TableWithReservedNameWorksAsExpected()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            db.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Group')
                  drop table [Group]");
            db.Execute("create table [Group] ([Id] int not null, [Name] varchar(100) not null)");

            // --- Act
            db.Fetch<GroupRecord>();
        }

        [TableName("Group")]
        class GroupRecord : DataRecord<GroupRecord>
        {
            [PrimaryKey]
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
