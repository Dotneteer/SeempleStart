using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    [TableName("ListItemDefinition")]
    [SchemaName("Config")]
    public class ListItemDefinitionRecord : DataRecord<ListItemDefinitionRecord>
    {
        [PrimaryKey(1)]
        public string ListId { get; set; }

        [PrimaryKey(2)]
        public string ItemId { get; set; }

        public bool IsSystemItem { get; set; }
    }
}