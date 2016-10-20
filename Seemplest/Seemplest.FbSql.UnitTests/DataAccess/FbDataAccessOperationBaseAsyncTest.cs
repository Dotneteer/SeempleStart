using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using Seemplest.FbSql.DataAccess;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.FbSql.UnitTests.DataAccess
{
    [TestClass]
    public class FbDataAccessOperationBaseAsyncTest
    {
        private const string DB_CONN = "connStr=Seemplest";

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            var dc = new FbDatabase(DB_CONN);
            dc.Execute(
                @"EXECUTE BLOCK AS BEGIN
                  if (exists(select 1 from rdb$relations where rdb$relation_name = 'Data')) then 
                  execute statement 'drop table ""Data"";';
                  END");
            dc.Execute(
                @"create table ""Data""
                  (
                    ""Id"" int not null,
                    ""Name"" varchar(64) not null,
                    constraint ""pk_data"" primary key(""Id"")
                  )");
        }

        [TestInitialize]
        public void Initialize()
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<ITestDataOperations, TestDataOperations>(DB_CONN);
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
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
        public async Task BeginTransactionAsyncWorksAsExpected()
        {
            // --- Arrange
            var data = new DataRecord
            {
                Id = 1,
                Name = "Data"
            };

            // --- Act
            using (var dc = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                await dc.BeginTransactionAsync();
                await dc.InsertDataAsync(data);
                await dc.CompleteAsync();
            }

            // --- Assert
            DataRecord back;
            using (var dc = DataAccessFactory.CreateReadOnlyContext<ITestDataOperations>())
            {
                back = await dc.GetDataAsync(data.Id);
            }

            back.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task AbortTransactionAsyncWorksAsExpected()
        {
            // --- Arrange
            var data = new DataRecord
            {
                Id = 2,
                Name = "Data"
            };

            // --- Act
            using (var dc = DataAccessFactory.CreateContext<ITestDataOperations>())
            {
                await dc.BeginTransactionAsync();
                await dc.InsertDataAsync(data);
                await dc.AbortTransactionAsync();
            }

            // --- Assert
            DataRecord back;
            using (var dc = DataAccessFactory.CreateReadOnlyContext<ITestDataOperations>())
            {
                back = await dc.GetDataAsync(data.Id);
            }

            back.ShouldBeNull();
        }

        internal class MyDataAccessOperation : FbDataAccessOperationBase
        {
            public MyDataAccessOperation(string connectionOrName)
                : base(connectionOrName)
            {
            }

            public SqlOperationMode Mode => OperationMode;
        }

        [TableName("Data")]
        public class DataRecord : DataRecord<DataRecord>
        {
            private int _id;
            private string _name;

            [PrimaryKey]
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
            Task<int> InsertDataAsync(DataRecord record);

            Task<DataRecord> GetDataAsync(int id);
        }

        internal class TestDataOperations : MyDataAccessOperation, ITestDataOperations
        {
            public TestDataOperations(string connectionOrName)
                : base(connectionOrName)
            {
            }

            public async Task<int> InsertDataAsync(DataRecord record)
            {
                await Operation(ctx => ctx.InsertAsync(record));
                return record.Id;
            }

            public async Task<DataRecord> GetDataAsync(int id)
            {
                return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<DataRecord>(@"where ""Id"" = @0", id));
            }
        }
    }
}
