using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    [TableName("ConfigurationVersion")]
    [SchemaName("Config")]
    public class ConfigurationVersionRecord: DataRecord<ConfigurationVersionRecord>
    {
        private int _id;
        private string _label;
        private DateTime _created;

        [PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { _id = Modify(value, "Id"); }
        }

        public string Label
        {
            get { return _label; }
            set { _label = Modify(value, "Label"); }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = Modify(value, "Created"); }
        }
    }
}