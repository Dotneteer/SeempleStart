using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    [TableName("LocalizedResource")]
    [SchemaName("Config")]
    public class LocalizedResourceRecord : DataRecord<LocalizedResourceRecord>
    {
        private string _locale;
        private string _category;
        private string _resourceKey;
        private string _value;

        [PrimaryKey(1)]
        public string Locale
        {
            get { return _locale; }
            set { _locale = Modify(value, "Locale"); }
        }

        [PrimaryKey(2)]
        public string Category
        {
            get { return _category; }
            set { _category = Modify(value, "Category"); }
        }

        [PrimaryKey(3)]
        public string ResourceKey
        {
            get { return _resourceKey; }
            set { _resourceKey = Modify(value, "ResourceKey"); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = Modify(value, "Value"); }
        }
    }
}