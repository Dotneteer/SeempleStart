using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    [TableName("Locale")]
    [SchemaName("Config")]
    public class LocaleRecord: DataRecord<LocaleRecord>
    {
        private string _code;
        private string _displayName;

        [PrimaryKey]
        public string Code
        {
            get { return _code; }
            set { _code = Modify(value, "Code"); }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = Modify(value, "DisplayName"); }
        }
    }
}