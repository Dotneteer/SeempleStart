using System;

namespace SeemplestCloud.Dto.Subscription
{
    // --- This class has been automatically generated from the database schema
    // --- 2014.10.08. 13:58:59
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public string SubscriberName { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryPhone { get; set; }
        public string AddrCountry { get; set; }
        public string AddrZip { get; set; }
        public string AddrTown { get; set; }
        public string AddrLine1 { get; set; }
        public string AddrLine2 { get; set; }
        public string AddrState { get; set; }
        public string TaxId { get; set; }
        public string BankAccountNo { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset? LastModifiedUtc { get; set; }
    }

}