using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 15:56:33
    [TableName("ServiceDefinition")]
    [SchemaName("Platform")]
    public class ServiceDefinitionRecord : DataRecord<ServiceDefinitionRecord>
    {
        private string _code;
        private string _name;
        private string _marketingName;
        private string _description;
        private DateTimeOffset? _startDateUtc;
        private DateTimeOffset? _endDateUtc;
        private string _defaultOwnerRole;
        private string _defaultUserRole;
        private DateTimeOffset _createdUtc;
        private DateTimeOffset? _lastModifiedUtc;

        [PrimaryKey]
        public string Code
        {
            get { return _code; }
            set { _code = Modify(value, "Code"); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = Modify(value, "Name"); }
        }

        public string MarketingName
        {
            get { return _marketingName; }
            set { _marketingName = Modify(value, "MarketingName"); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = Modify(value, "Description"); }
        }

        public DateTimeOffset? StartDateUtc
        {
            get { return _startDateUtc; }
            set { _startDateUtc = Modify(value, "StartDateUtc"); }
        }

        public DateTimeOffset? EndDateUtc
        {
            get { return _endDateUtc; }
            set { _endDateUtc = Modify(value, "EndDateUtc"); }
        }

        public string DefaultOwnerRole
        {
            get { return _defaultOwnerRole; }
            set { _defaultOwnerRole = Modify(value, "DefaultOwnerRole"); }
        }

        public string DefaultUserRole
        {
            get { return _defaultUserRole; }
            set { _defaultUserRole = Modify(value, "DefaultUserRole"); }
        }

        public DateTimeOffset CreatedUtc
        {
            get { return _createdUtc; }
            set { _createdUtc = Modify(value, "CreatedUtc"); }
        }

        public DateTimeOffset? LastModifiedUtc
        {
            get { return _lastModifiedUtc; }
            set { _lastModifiedUtc = Modify(value, "LastModifiedUtc"); }
        }
    }
}
