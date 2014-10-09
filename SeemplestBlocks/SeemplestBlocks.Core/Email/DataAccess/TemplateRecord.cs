using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.Email.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.09. 14:43:10
    [TableName("Template")]
    [SchemaName("Email")]
    public class TemplateRecord : DataRecord<TemplateRecord>
    {
        private string _id;
        private string _subject;
        private string _body;

        [PrimaryKey]
        public string Id
        {
            get { return _id; }
            set { _id = Modify(value, "Id"); }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = Modify(value, "Subject"); }
        }

        public string Body
        {
            get { return _body; }
            set { _body = Modify(value, "Body"); }
        }
    }
}
