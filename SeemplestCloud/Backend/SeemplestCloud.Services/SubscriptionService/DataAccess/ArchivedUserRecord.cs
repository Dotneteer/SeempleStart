using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 15:53:17
    [TableName("ArchivedUser")]
    [SchemaName("Platform")]
    public class ArchivedUserRecord : DataRecord<ArchivedUserRecord>
    {
        private Guid _id;
        private string _userName;
        private string _email;
        private string _securityStamp;
        private string _passwordHash;
        private DateTimeOffset? _lastFailedAuthUtc;
        private int _accessFailedCount;
        private bool _lockedOut;
        private bool _ownerSuspend;
        private bool _passwordResetSuspend;
        private DateTimeOffset _createdUtc;
        private DateTimeOffset? _lastModifiedUtc;
        private DateTimeOffset _archivedUtc;

        [PrimaryKey]
        public Guid Id
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

        public string PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = Modify(value, "PasswordHash"); }
        }

        public DateTimeOffset? LastFailedAuthUtc
        {
            get { return _lastFailedAuthUtc; }
            set { _lastFailedAuthUtc = Modify(value, "LastFailedAuthUtc"); }
        }

        public int AccessFailedCount
        {
            get { return _accessFailedCount; }
            set { _accessFailedCount = Modify(value, "AccessFailedCount"); }
        }

        public bool LockedOut
        {
            get { return _lockedOut; }
            set { _lockedOut = Modify(value, "LockedOut"); }
        }

        public bool OwnerSuspend
        {
            get { return _ownerSuspend; }
            set { _ownerSuspend = Modify(value, "OwnerSuspend"); }
        }

        public bool PasswordResetSuspend
        {
            get { return _passwordResetSuspend; }
            set { _passwordResetSuspend = Modify(value, "PasswordResetSuspend"); }
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

        public DateTimeOffset ArchivedUtc
        {
            get { return _archivedUtc; }
            set { _archivedUtc = Modify(value, "ArchivedUtc"); }
        }
    }
}
