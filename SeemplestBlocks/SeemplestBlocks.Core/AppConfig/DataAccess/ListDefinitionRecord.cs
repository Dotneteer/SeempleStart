using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    [TableName("ListDefinition")]
    [SchemaName("Config")]
    public class ListDefinitionRecord : DataRecord<ListDefinitionRecord>
    {
        [PrimaryKey]
        public string Id { get; set; }

        public bool IsSystemList { get; set; }
    }
}