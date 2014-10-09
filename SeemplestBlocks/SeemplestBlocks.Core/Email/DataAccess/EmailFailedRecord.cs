using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestBlocks.Core.Email.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.09. 14:40:46
    [TableName("EmailFailed")]
    [SchemaName("Email")]
    public class EmailFailedRecord : DataRecord<EmailFailedRecord>
    {
        private int _id;
        private DateTimeOffset _queuedUtc;
        private string _fromAddr;
        private string _fromName;
        private string _toAddr;
        private string _subject;
        private string _message;
        private string _appointment;
        private DateTimeOffset _failedUtc;
        private int _retryCount;
        private string _lastError;

        [PrimaryKey]
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

        public string Subject
        {
            get { return _subject; }
            set { _subject = Modify(value, "Subject"); }
        }

        public string Message
        {
            get { return _message; }
            set { _message = Modify(value, "Message"); }
        }

        public string Appointment
        {
            get { return _appointment; }
            set { _appointment = Modify(value, "Appointment"); }
        }

        public DateTimeOffset FailedUtc
        {
            get { return _failedUtc; }
            set { _failedUtc = Modify(value, "FailedUtc"); }
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
