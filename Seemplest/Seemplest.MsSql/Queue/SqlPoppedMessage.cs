using System;
using Seemplest.Core.Configuration;
using Seemplest.Core.Queue;

namespace Seemplest.MsSql.Queue
{
    /// <summary>
    /// The message that has been popped. Has a PopReceipt and a NextVisibleTime
    /// </summary>
    public class SqlPoppedMessage : SqlQueuedMessage, IPoppedMessage
    {
        private readonly int _visibilityTimeoutInSeconds;
        private DateTime _popTime;

        /// <summary>
        /// Creates a new instance of an SQL popped message
        /// </summary>
        /// <param name="messageText">The data/text of the message</param>
        /// <param name="id">The messageid property column in the database </param>
        /// <param name="expirationTime">The time until the message should be processed</param>
        /// <param name="insertionTime">The date of the insertion into the queue</param>
        /// <param name="dequeueCount">It shows that how much times the message was gotten out of the queue</param>
        /// <param name="visibilityTimeoutInSeconds">
        /// Until this time ellapses, no other process can access to this message in the queue. After that, it is free
        /// </param>
        /// <param name="popReceipt">The popreceipt of the popped message</param>
        public SqlPoppedMessage(string messageText, string id, DateTime expirationTime, DateTime insertionTime, int dequeueCount,
            int visibilityTimeoutInSeconds, string popReceipt)
            : base(messageText, id, expirationTime, insertionTime, dequeueCount)
        {
            MessageText = messageText;
            Id = id;
            InsertionTime = insertionTime;
            ExpirationTime = expirationTime;
            DequeueCount = dequeueCount;
            _popTime = EnvironmentInfo.GetCurrentDateTimeUtc();
            _visibilityTimeoutInSeconds = visibilityTimeoutInSeconds;
            PopReceipt = popReceipt;
        }

        /// <summary>
        /// Creates a new instance of an SQL popped message
        /// </summary>
        /// <param name="message">The queued message that should be encapsulated into a popped message</param>
        /// <param name="visibilityTimeoutInSeconds">
        /// Until this time ellapses, no other process can access to this message in the queue. After that, it is free
        /// </param>
        /// <param name="popReceipt">The popreceipt of the popped message</param>
        public SqlPoppedMessage(IQueuedMessage message, int visibilityTimeoutInSeconds, string popReceipt)
            : this(message.MessageText, message.Id, message.ExpirationTime, message.InsertionTime,
            message.DequeueCount, visibilityTimeoutInSeconds, popReceipt)
        { }

        /// <summary>
        /// Pop time delayed by visiblitytimeout
        /// </summary>
        public DateTime NextVisibleTime
        {
            get { return _popTime.AddSeconds(_visibilityTimeoutInSeconds); }
        }

        /// <summary>
        /// Gets pop receipt information
        /// </summary>
        public string PopReceipt { get; private set; }
    }
}