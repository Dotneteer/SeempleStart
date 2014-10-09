using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 16:02:16
    [TableName("SubscriptionContent")]
    [SchemaName("Platform")]
    public class SubscriptionContentRecord : DataRecord<SubscriptionContentRecord>
    {
        private int _subscriptionId;
        private string _basePackageCode;
        private bool _isFree;
        private decimal? _monthlyPrice;
        private string _currencyType;
        private string _comment;
        private DateTimeOffset _createdUtc;
        private DateTimeOffset? _lastModifiedUtc;

        [PrimaryKey]
        public int SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = Modify(value, "SubscriptionId"); }
        }

        public string BasePackageCode
        {
            get { return _basePackageCode; }
            set { _basePackageCode = Modify(value, "BasePackageCode"); }
        }

        public bool IsFree
        {
            get { return _isFree; }
            set { _isFree = Modify(value, "IsFree"); }
        }

        public decimal? MonthlyPrice
        {
            get { return _monthlyPrice; }
            set { _monthlyPrice = Modify(value, "MonthlyPrice"); }
        }

        public string CurrencyType
        {
            get { return _currencyType; }
            set { _currencyType = Modify(value, "CurrencyType"); }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = Modify(value, "Comment"); }
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
