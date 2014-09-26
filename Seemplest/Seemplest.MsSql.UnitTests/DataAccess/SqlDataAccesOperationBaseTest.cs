﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class SqlDataAccessOperationBaseTest
    {
        private const string DB_CONN = "connStr=Seemplest";

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            var dc = new SqlDatabase(DB_CONN);
            dc.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Data')
                  drop table Data");
            dc.Execute(
                @"create table [dbo].[Data]
                  (
                    [Id] int not null primary key identity,
                    [Name] nvarchar(64) not null
                  )");
        }

        [TestInitialize]
        public void Initialize()
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<ITestDataOperations, TestDataOperations>(DB_CONN);
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);

            var dc = new SqlDatabase(DB_CONN);
            dc.Execute("delete from [dbo].[Parent]");
        }

        [TestMethod]
        public void SetOperationModeWorksOnlyOnce()
        {
            // --- Arrange
            var dao = new MyDataAccessOperation(DB_CONN);
            dao.SetOperationMode(SqlOperationMode.ReadOnly);
            
            // --- Act
            dao.SetOperationMode(SqlOperationMode.ReadWrite);

            // --- Assert
            dao.Mode.ShouldEqual(SqlOperationMode.ReadOnly);
        }

        [TestMethod]
        public void BeginTransactionWorksAsExpected()
        {
            // --- Arrange
            var data = new DataRecord
            {
                Name = "Data"
            };

            // --- Act
            using (var dc = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                dc.BeginTransaction();
                dc.InsertData(data);
                dc.Complete();
            }

            // --- Assert
            DataRecord back;
            using (var dc = DataAccessFactory.CreateReadOnlyContext<ITestDataOperations>())
            {
                back = dc.GetData(data.Id);
            }

            back.ShouldNotBeNull();
        }

        [TestMethod]
        public void AbortTransactionWorksAsExpected()
        {
            // --- Arrange
            var data = new DataRecord
            {
                Name = "Data"
            };

            // --- Act
            using (var dc = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                dc.BeginTransaction();
                dc.InsertData(data);
                dc.AbortTransaction();
            }

            // --- Assert
            DataRecord back;
            using (var dc = DataAccessFactory.CreateReadOnlyContext<ITestDataOperations>())
            {
                back = dc.GetData(data.Id);
            }

            back.ShouldBeNull();
        }

        internal class MyDataAccessOperation : SqlDataAccessOperationBase 
        {
            public MyDataAccessOperation(string connectionOrName) 
                : base(connectionOrName)
            {
            }

            public SqlOperationMode Mode
            {
                get { return OperationMode; }
            }
        }

        [TableName("Data")]
        public class DataRecord : DataRecord<DataRecord>
        {
            private int _id;
            private string _name;

            [PrimaryKey]
            [AutoGenerated]
            public int Id
            {
                get { return _id; }
                set { _id = Modify(value, "Id"); }
            }

            public string Name
            {
                get { return _name; }
                set { _name = Modify(value, "Name"); }
            }
        }

        interface ITestDataOperations : IDataAccessOperation
        {
            int InsertData(DataRecord record);

            DataRecord GetData(int id);
        }

        internal class TestDataOperations : MyDataAccessOperation, ITestDataOperations
        {
            public TestDataOperations(string connectionOrName)
                : base(connectionOrName)
            {
            }

            public int InsertData(DataRecord record)
            {
                Operation(ctx => ctx.Insert(record));
                return record.Id;
            }

            public DataRecord GetData(int id)
            {
                return Operation(ctx => ctx.FirstOrDefault<DataRecord>("where Id = @0", id));
            }
        }
    }
}
