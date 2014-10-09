using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 16:08:50
    [TableName("UserRole")]
    [SchemaName("Platform")]
    public class UserRoleRecord : DataRecord<UserRoleRecord>
    {
        private Guid _userId;
        private string _serviceCode;
        private string _roleCode;

        [PrimaryKey(1)]
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = Modify(value, "UserId"); }
        }

        [PrimaryKey(2)]
        public string ServiceCode
        {
            get { return _serviceCode; }
            set { _serviceCode = Modify(value, "ServiceCode"); }
        }

        [PrimaryKey(3)]
        public string RoleCode
        {
            get { return _roleCode; }
            set { _roleCode = Modify(value, "RoleCode"); }
        }
    }
}
