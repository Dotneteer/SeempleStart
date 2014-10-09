using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 15:57:56
    [TableName("ServiceParameter")]
    [SchemaName("Platform")]
    public class ServiceParameterRecord : DataRecord<ServiceParameterRecord>
    {
        private string _serviceCode;
        private string _parameterCode;
        private string _name;
        private string _description;
        private string _type;

        [PrimaryKey(1)]
        public string ServiceCode
        {
            get { return _serviceCode; }
            set { _serviceCode = Modify(value, "ServiceCode"); }
        }

        [PrimaryKey(2)]
        public string ParameterCode
        {
            get { return _parameterCode; }
            set { _parameterCode = Modify(value, "ParameterCode"); }
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

        public string Type
        {
            get { return _type; }
            set { _type = Modify(value, "Type"); }
        }
    }
}
