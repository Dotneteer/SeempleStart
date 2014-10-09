﻿using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.09. 16:50:23
    [TableName("UserInvitation")]
    [SchemaName("Platform")]
    public class UserInvitationRecord : DataRecord<UserInvitationRecord>
    {
        private Guid _userId;
        private int? _subscriptionId;
        private string _invitedEmail;
        private string _invitationCode;
        private DateTimeOffset? _expirationDateUtc;
        private string _state;
        private string _type;
        private DateTimeOffset _createdUtc;
        private DateTimeOffset? _lastModifiedUtc;

        [PrimaryKey]
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = Modify(value, "UserId"); }
        }

        public int? SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = Modify(value, "SubscriptionId"); }
        }

        public string InvitedEmail
        {
            get { return _invitedEmail; }
            set { _invitedEmail = Modify(value, "InvitedEmail"); }
        }

        public string InvitationCode
        {
            get { return _invitationCode; }
            set { _invitationCode = Modify(value, "InvitationCode"); }
        }

        public DateTimeOffset? ExpirationDateUtc
        {
            get { return _expirationDateUtc; }
            set { _expirationDateUtc = Modify(value, "ExpirationDateUtc"); }
        }

        public string State
        {
            get { return _state; }
            set { _state = Modify(value, "State"); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = Modify(value, "Type"); }
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
