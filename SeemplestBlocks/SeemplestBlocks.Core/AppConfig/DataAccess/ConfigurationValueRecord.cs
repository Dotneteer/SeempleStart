using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    [TableName("ConfigurationValue")]
    [SchemaName("Config")]
    public class ConfigurationValueRecord: DataRecord<ConfigurationValueRecord>
    {
        private int _versionId;
        private string _category;
        private string _configKey;
        private string _value;


        [PrimaryKey(1)]
        public int VersionId
        {
            get { return _versionId; }
            set { _versionId = Modify(value, "VersionId"); }
        }

        [PrimaryKey(2)]
        public string Category
        {
            get { return _category; }
            set { _category = Modify(value, "Category"); }
        }
    
        [PrimaryKey(3)]
        public string ConfigKey
        {
            get { return _configKey; }
            set { _configKey = Modify(value, "ConfigKey"); }
        }
        public string Value
        {
            get { return _value; }
            set { _value = Modify(value, "Value"); }
        }
    }
}