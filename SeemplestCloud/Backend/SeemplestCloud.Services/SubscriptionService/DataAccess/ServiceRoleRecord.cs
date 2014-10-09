using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 15:58:52
    [TableName("ServiceRole")]
    [SchemaName("Platform")]
    public class ServiceRoleRecord : DataRecord<ServiceRoleRecord>
    {
        private string _serviceCode;
        private string _roleCode;
        private string _name;
        private string _description;

        [PrimaryKey(1)]
        public string ServiceCode
        {
            get { return _serviceCode; }
            set { _serviceCode = Modify(value, "ServiceCode"); }
        }

        [PrimaryKey(2)]
        public string RoleCode
        {
            get { return _roleCode; }
            set { _roleCode = Modify(value, "RoleCode"); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = Modify(value, "Name"); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = Modify(value, "Description"); }
        }
    }
}
