using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 16:05:41
    [TableName("UserAccount")]
    [SchemaName("Platform")]
    public class UserAccountRecord : DataRecord<UserAccountRecord>
    {
        private Guid _userId;
        private string _provider;
        private string _providerData;

        [PrimaryKey(1)]
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = Modify(value, "UserId"); }
        }

        [PrimaryKey(2)]
        public string Provider
        {
            get { return _provider; }
            set { _provider = Modify(value, "Provider"); }
        }

        public string ProviderData
        {
            get { return _providerData; }
            set { _providerData = Modify(value, "ProviderData"); }
        }
    }
}
