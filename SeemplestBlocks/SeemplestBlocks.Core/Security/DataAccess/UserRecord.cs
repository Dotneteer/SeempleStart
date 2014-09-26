using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.Security.DataAccess
{
    [TableName("User")]
    [SchemaName("Security")]
    public class UserRecord: DataRecord<UserRecord>
    {
        private string _id;
        private string _userName;
        private string _email;
        private string _securityStamp;
        private bool _emailConfirmed;
        private string _passwordHash;
        private string _phoneNumber;
        private bool _phoneNumberConfirmed;
        private DateTimeOffset? _lockOutEndDateUtc;
        private int _accessFailedCount;
        private bool _active;
        private DateTime _created;
        private DateTime? _lastModified;

        [PrimaryKey]
        public string Id
        {
            get { return _id; }
            set { _id = Modify(value, "Id"); }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = Modify(value, "UserName"); }
        }

        public string Email
        {
            get { return _email; }
            set { _email = Modify(value, "Email"); }
        }

        public string SecurityStamp
        {
            get { return _securityStamp; }
            set { _securityStamp = Modify(value, "SecurityStamp"); }
        }

        public bool EmailConfirmed
        {
            get { return _emailConfirmed; }
            set { _emailConfirmed = Modify(value, "EmailConfirmed"); }
        }

        public string PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = Modify(value, "PasswordHash"); }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = Modify(value, "PhoneNumber"); }
        }

        public bool PhoneNumberConfirmed
        {
            get { return _phoneNumberConfirmed; }
            set { _phoneNumberConfirmed = Modify(value, "PhoneNumberConfirmed"); }
        }

        public DateTimeOffset? LockOutEndDateUtc
        {
            get { return _lockOutEndDateUtc; }
            set { _lockOutEndDateUtc = Modify(value, "LockOutEndDateUtc"); }
        }

        public int AccessFailedCount
        {
            get { return _accessFailedCount; }
            set { _accessFailedCount = Modify(value, "AccessFailedCount"); }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = Modify(value, "Active"); }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = Modify(value, "Created"); }
        }

        public DateTime? LastModified
        {
            get { return _lastModified; }
            set { _lastModified = Modify(value, "LastModified"); }
        }
    }
}