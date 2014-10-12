﻿using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.Email.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.09. 15:01:07
    [TableName("EmailToSend")]
    [SchemaName("Email")]
    public class EmailToSendRecord : DataRecord<EmailToSendRecord>
    {
        private int _id;
        private DateTimeOffset _queuedUtc;
        private string _fromAddr;
        private string _fromName;
        private string _toAddr;
        private string _templateType;
        private string _parameters;
        private string _appointment;
        private DateTimeOffset? _sendAfterUtc;
        private int _retryCount;
        private string _lastError;

        [PrimaryKey]
        [AutoGenerated]
        public int Id
        {
            get { return _id; }
            set { _id = Modify(value, "Id"); }
        }

        public DateTimeOffset QueuedUtc
        {
            get { return _queuedUtc; }
            set { _queuedUtc = Modify(value, "QueuedUtc"); }
        }

        public string FromAddr
        {
            get { return _fromAddr; }
            set { _fromAddr = Modify(value, "FromAddr"); }
        }

        public string FromName
        {
            get { return _fromName; }
            set { _fromName = Modify(value, "FromName"); }
        }

        public string ToAddr
        {
            get { return _toAddr; }
            set { _toAddr = Modify(value, "ToAddr"); }
        }

        public string TemplateType
        {
            get { return _templateType; }
            set { _templateType = Modify(value, "TemplateType"); }
        }

        public string Parameters
        {
            get { return _parameters; }
            set { _parameters = Modify(value, "Parameters"); }
        }

        public string Appointment
        {
            get { return _appointment; }
            set { _appointment = Modify(value, "Appointment"); }
        }

        public DateTimeOffset? SendAfterUtc
        {
            get { return _sendAfterUtc; }
            set { _sendAfterUtc = Modify(value, "SendAfterUtc"); }
        }

        public int RetryCount
        {
            get { return _retryCount; }
            set { _retryCount = Modify(value, "RetryCount"); }
        }

        public string LastError
        {
            get { return _lastError; }
            set { _lastError = Modify(value, "LastError"); }
        }
    }
}