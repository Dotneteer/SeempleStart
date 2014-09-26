using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.Security.DataAccess
{
    [TableName("UserAccount")]
    [SchemaName("Security")]
    public class UserAccountRecord: DataRecord<UserAccountRecord>
    {
        [PrimaryKey(1)]
        public string UserId { get; set; }

        [PrimaryKey(2)]
        public string Provider { get; set; }

        public string ProviderData { get; set; }
    }
}