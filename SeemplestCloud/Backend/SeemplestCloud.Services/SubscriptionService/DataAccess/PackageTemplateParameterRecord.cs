using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 15:55:16
    [TableName("PackageTemplateParameter")]
    [SchemaName("Platform")]
    public class PackageTemplateParameterRecord : DataRecord<PackageTemplateParameterRecord>
    {
        private string _packageCode;
        private string _serviceCode;
        private string _parameterCode;
        private string _value;

        [PrimaryKey(1)]
        public string PackageCode
        {
            get { return _packageCode; }
            set { _packageCode = Modify(value, "PackageCode"); }
        }

        [PrimaryKey(2)]
        public string ServiceCode
        {
            get { return _serviceCode; }
            set { _serviceCode = Modify(value, "ServiceCode"); }
        }

        [PrimaryKey(3)]
        public string ParameterCode
        {
            get { return _parameterCode; }
            set { _parameterCode = Modify(value, "ParameterCode"); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = Modify(value, "Value"); }
        }
    }
}
