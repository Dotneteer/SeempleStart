using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 16:03:56
    [TableName("SubscriptionParameterValue")]
    [SchemaName("Platform")]
    public class SubscriptionParameterValueRecord : DataRecord<SubscriptionParameterValueRecord>
    {
        private int _subscriptionId;
        private string _serviceCode;
        private string _parameterCode;
        private string _value;

        [PrimaryKey(1)]
        public int SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = Modify(value, "SubscriptionId"); }
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
