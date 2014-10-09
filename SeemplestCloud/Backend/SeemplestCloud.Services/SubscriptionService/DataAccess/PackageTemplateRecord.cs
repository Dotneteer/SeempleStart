using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace SeemplestCloud.Services.SubscriptionService.DataAccess
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.07. 15:54:28
    [TableName("PackageTemplate")]
    [SchemaName("Platform")]
    public class PackageTemplateRecord : DataRecord<PackageTemplateRecord>
    {
        private string _code;
        private string _name;
        private string _upgradeCode;
        private string _marketingName;
        private string _description;
        private string _marketingDescription;
        private bool _isFree;
        private decimal? _monthlyPrice;
        private string _currencyType;
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

        public string UpgradeCode
        {
            get { return _upgradeCode; }
            set { _upgradeCode = Modify(value, "UpgradeCode"); }
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

        public string MarketingDescription
        {
            get { return _marketingDescription; }
            set { _marketingDescription = Modify(value, "MarketingDescription"); }
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
