using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DataAccess.Exceptions;
using Seemplest.Core.DependencyInjection;
using Seemplest.MsSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.MsSql.UnitTests.DataAccess
{
    [TestClass]
    public class DatabaseExceptionHelperTest
    {
        private const string DB_CONN = "connStr=Seemplest";

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            var dc = new SqlDatabase(DB_CONN);
            dc.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Child')
                  drop table Child");
            dc.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ChildNoPk')
                  drop table ChildNoPk");
            dc.Execute(
                @"if exists (select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Parent')
                  drop table Parent");
            dc.Execute(
                @"create table [dbo].[Parent]
                  (
                    [Id] int not null primary key identity,
                    [Name] nvarchar(64) not null
                  )");
            dc.Execute(
                @"create table [dbo].[Child]
                  (
                    [Id] int not null primary key identity,
                    [ParentId] int null,
                    [Name] nvarchar(64) not null,
                    [Value] int not null, 
                    constraint [CK_Child_Value] check ([Value] between 1 and 10), 
                    constraint [AK_Child_Name] unique ([Name]),
                    constraint [FK_Child_ToParent] foreign key ([ParentId]) REFERENCES [Parent]([Id])
                  )");
            dc.Execute(
                @"create unique index [IX_Child_Value] on [dbo].[Child] ([Value])");
            dc.Execute(
                @"create table [dbo].[ChildNoPk]
                  (
                    [Id] int not null identity,
                    [ParentId] int null,
                    [Name] nvarchar(64) not null,
                    [Value] int not null, 
                    constraint [CK_ChildNoPk_Value] check ([Value] between 1 and 10), 
                    constraint [AK_ChildNoPk_Name] unique ([Name]),
                    constraint [FK_ChildNoPk_ToParent] foreign key ([ParentId]) REFERENCES [Parent]([Id])
                  )");
        }

        [TestInitialize]
        public void Initialize()
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<ITestDataOperations, TestDataOperations>(DB_CONN);
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);

            var dc = new SqlDatabase(DB_CONN);
            dc.Execute("delete from [dbo].[Child]");
            dc.Execute("delete from [dbo].[Parent]");
        }

        [TestMethod]
        public void UniqueKeyExceptionIsRecognized1()
        {
            // --- Arrange
            var child1 = new ChildRecord
            {
                Name = "Child1",
                Value = 1
            };
            var child2 = new ChildRecord
            {
                Name = "Child1",
                Value = 2
            };

            // --- Act
            UniqueKeyViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertChild(child1);
                    ctx.InsertChild(child2);
                }
            }
            catch (UniqueKeyViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("dbo.Child");
            exCaught.KeyName.ShouldEqual("AK_Child_Name");
        }

        [TestMethod]
        public void UniqueKeyExceptionIsRecognized2()
        {
            // --- Arrange
            var child1 = new ChildRecord
            {
                Name = "Child1",
                Value = 1
            };
            var child2 = new ChildRecord
            {
                Name = "Child2",
                Value = 2
            };
            using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                ctx.InsertChild(child1);
                ctx.InsertChild(child2);
            }

            // --- Act
            UniqueKeyViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    child2.Name = "Child1";
                    ctx.UpdateChild(child2);
                }
            }
            catch (UniqueKeyViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("dbo.Child");
            exCaught.KeyName.ShouldEqual("AK_Child_Name");
        }

        [TestMethod]
        public void UniqueKeyExceptionIsRecognized3()
        {
            // --- Arrange
            var child1 = new ChildNoPkRecord
            {
                Name = "Child1",
                Value = 1
            };
            var child2 = new ChildNoPkRecord
            {
                Name = "Child1",
                Value = 2
            };

            // --- Act
            UniqueKeyViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertChildNoPk(child1);
                    ctx.InsertChildNoPk(child2);
                }
            }
            catch (UniqueKeyViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("dbo.ChildNoPk");
            exCaught.KeyName.ShouldEqual("AK_ChildNoPk_Name");
        }

        [TestMethod]
        public void UniqueIndexExceptionIsRecognized()
        {
            // --- Arrange
            var child1 = new ChildRecord
            {
                Name = "Child1",
                Value = 1
            };
            var child2 = new ChildRecord
            {
                Name = "Child2",
                Value = 1
            };

            // --- Act
            UniqueIndexViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertChild(child1);
                    ctx.InsertChild(child2);
                }
            }
            catch (UniqueIndexViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("dbo.Child");
            exCaught.IndexName.ShouldEqual("IX_Child_Value");
        }

        [TestMethod]
        public void ForeignKeyExceptionIsRecognized1()
        {
            // --- Arrange
            var parent = new ParentRecord
            {
                Name = "Parent",
            };
            var child = new ChildRecord
            {
                Name = "Child1",
                Value = 1,
                ParentId = -1
            };

            // --- Act
            ForeignKeyViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertParent(parent, false);
                    ctx.InsertChild(child);
                }
            }
            catch (ForeignKeyViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.KeyName.ShouldEqual("FK_Child_ToParent");
        }

        [TestMethod]
        public void ForeignKeyExceptionIsRecognized2()
        {
            // --- Arrange
            var parent = new ParentRecord
            {
                Name = "Parent",
            };
            var child = new ChildRecord
            {
                Name = "Child1",
                Value = 1,
            };
            using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                ctx.InsertParent(parent, false);
                child.ParentId = parent.Id;
            }

            // --- Act
            ForeignKeyViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertChild(child);
                    child.ParentId = -1;
                    ctx.UpdateChild(child);
                }
            }
            catch (ForeignKeyViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.KeyName.ShouldEqual("FK_Child_ToParent");
        }

        [TestMethod]
        public void ForeignKeyExceptionIsRecognized3()
        {
            // --- Arrange
            var parent = new ParentRecord
            {
                Name = "Parent",
            };
            var child = new ChildRecord
            {
                Name = "Child1",
                Value = 1,
            };
            using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                ctx.InsertParent(parent, false);
                child.ParentId = parent.Id;
                ctx.InsertChild(child);
            }

            // --- Act
            ForeignKeyViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.DeleteParent(parent.Id);
                }
            }
            catch (ForeignKeyViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.KeyName.ShouldEqual("FK_Child_ToParent");
        }

        [TestMethod]
        public void PrimaryKeyExceptionIsRecognized()
        {
            // --- Arrange
            var parent = new ParentRecord
            {
                Name = "Parent"
            };

            // --- Act
            PrimaryKeyViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertParent(parent, false);
                    ctx.InsertParent(parent, true);
                }
            }
            catch (PrimaryKeyViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("dbo.Parent");
        }

        [TestMethod]
        public void NullValueNotAllowedExceptionIsRecognized1()
        {
            // --- Arrange
            var child = new ChildRecord
            {
                Name = "Child",
                Value = null
            };

            // --- Act
            NullValueNotAllowedException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertChild(child);
                }
            }
            catch (NullValueNotAllowedException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("Seemplest.Test.dbo.Child");
            exCaught.ColumnName.ShouldEqual("Value");
        }

        [TestMethod]
        public void NullValueNotAllowedExceptionIsRecognized2()
        {
            // --- Arrange
            var child = new ChildRecord
            {
                Name = "Child",
                Value = 2
            };
            using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                ctx.InsertChild(child);
            }

            // --- Act
            NullValueNotAllowedException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    child.Value = null;
                    ctx.UpdateChild(child);
                }
            }
            catch (NullValueNotAllowedException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("Seemplest.Test.dbo.Child");
            exCaught.ColumnName.ShouldEqual("Value");
        }

        [TestMethod]
        public void CheckConstraintViolationExceptionIsRecognized1()
        {
            // --- Arrange
            var child = new ChildRecord
            {
                Name = "Child",
                Value = 14
            };

            // --- Act
            CheckConstraintViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.InsertChild(child);
                }
            }
            catch (CheckConstraintViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("dbo.Child");
            exCaught.CheckName.ShouldEqual("CK_Child_Value");
        }

        [TestMethod]
        public void CheckConstraintViolationExceptionIsRecognized2()
        {
            // --- Arrange
            var child = new ChildRecord
            {
                Name = "Child",
                Value = 4
            };
            using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                ctx.InsertChild(child);
            }

            // --- Act
            CheckConstraintViolationException exCaught = null;
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    child.Value = 14;
                    ctx.UpdateChild(child);
                }
            }
            catch (CheckConstraintViolationException ex)
            {
                exCaught = ex;
            }

            // --- Assert
            // ReSharper disable once PossibleNullReferenceException
            exCaught.TableName.ShouldEqual("dbo.Child");
            exCaught.CheckName.ShouldEqual("CK_Child_Value");
        }

        [TestMethod]
        public void OtherExceptionsAreNotTranslated()
        {
            // --- Act
            try
            {
                using (var ctx = DataAccessFactory.CreateContext<ITestDataOperations>())
                {
                    ctx.DummyCommand();
                }
            }
            catch (SqlException ex)
            {
                // --- Assert
                ex.ShouldBeOfType(typeof(SqlException));
                return;
            }
            Assert.Fail("This line should not be reached.");
        }

        [TableName("Parent")]
        public class ParentRecord : DataRecord<ParentRecord>
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

        [TableName("Child")]
        public class ChildRecord : DataRecord<ChildRecord>
        {
            private int _id;
            private string _name;
            private int? _value;
            private int? _parentId;

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

            public int? Value
            {
                get { return _value; }
                set { _value = Modify(value, "Value"); }
            }

            public int? ParentId
            {
                get { return _parentId; }
                set { _parentId = Modify(value, "ParentId"); }
            }
        }

        [TableName("ChildNoPk")]
        public class ChildNoPkRecord : DataRecord<ChildNoPkRecord>
        {
            private int _id;
            private string _name;
            private int? _value;
            private int? _parentId;

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

            public int? Value
            {
                get { return _value; }
                set { _value = Modify(value, "Value"); }
            }

            public int? ParentId
            {
                get { return _parentId; }
                set { _parentId = Modify(value, "ParentId"); }
            }
        }

        interface ITestDataOperations : IDataAccessOperation
        {
            int InsertParent(ParentRecord record, bool identity);

            int InsertChild(ChildRecord record);

            int InsertChildNoPk(ChildNoPkRecord record);

            int UpdateChild(ChildRecord record);

            void DeleteParent(int id);

            void DummyCommand();
        }

        public class TestDataOperations : SqlDataAccessOperationBase, ITestDataOperations
        {
            public TestDataOperations(string connectionOrName)
                : base(connectionOrName)
            {
            }

            public int InsertParent(ParentRecord record, bool identity)
            {
                Operation(ctx => ctx.Insert(record, identity));
                return record.Id;
            }

            public int InsertChild(ChildRecord record)
            {
                Operation(ctx => ctx.Insert(record));
                return record.Id;
            }

            public int InsertChildNoPk(ChildNoPkRecord record)
            {
                Operation(ctx => ctx.Insert(record));
                return record.Id;
            }

            public int UpdateChild(ChildRecord record)
            {
                Operation(ctx => ctx.Update(record));
                return record.Id;
            }

            public void DeleteParent(int id)
            {
                Operation(ctx => ctx.Execute("delete from [dbo].[Parent] where [Id]=@0", id));
            }

            public void DummyCommand()
            {
                Operation(ctx => ctx.Execute("dummy"));
            }
        }
    }
}
