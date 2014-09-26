using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    [TableName("CurrentConfigurationVersion")]
    [SchemaName("Config")]
    public class CurrentConfigurationVersionRecord: DataRecord<CurrentConfigurationVersionRecord>
    {
        private int _id;
        private int _currentVersion;
        private DateTime _lastModified;

        [PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { _id = Modify(value, "Id"); }
        }

        public int CurrentVersion
        {
            get { return _currentVersion; }
            set { _currentVersion = Modify(value, "CurrentVersion"); }
        }

        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = Modify(value, "LastModified"); }
        }
    }
}