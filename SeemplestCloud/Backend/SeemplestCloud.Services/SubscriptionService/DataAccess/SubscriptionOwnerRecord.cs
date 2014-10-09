using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 16:03:05
    [TableName("SubscriptionOwner")]
    [SchemaName("Platform")]
    public class SubscriptionOwnerRecord : DataRecord<SubscriptionOwnerRecord>
    {
        private int _subscriptionId;
        private Guid _userId;

        [PrimaryKey(1)]
        public int SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = Modify(value, "SubscriptionId"); }
        }

        [PrimaryKey(2)]
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = Modify(value, "UserId"); }
        }
    }
}
