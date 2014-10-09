using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 15:59:54
    [TableName("ServiceUser")]
    [SchemaName("Platform")]
    public class ServiceUserRecord : DataRecord<ServiceUserRecord>
    {
        private Guid _id;

        [PrimaryKey]
        public Guid Id
        {
            get { return _id; }
            set { _id = Modify(value, "Id"); }
        }
    }
}
