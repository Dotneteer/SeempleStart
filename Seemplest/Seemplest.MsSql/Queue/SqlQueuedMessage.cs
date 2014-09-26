using System;
using Seemplest.Core.Configuration;
using Seemplest.Core.Queue;

namespace Seemplest.MsSql.Queue
{
    /// <summary>
    /// The message that will be used in the SqlNamedQueues 
    /// </summary>
    public class SqlQueuedMessage : IQueuedMessage
    {
        /// <summary>
        /// The text/data of the message
        /// </summary>
        public string MessageText { get; protected set; }

        /// <summary>
        /// </summary>
        /// <param name="messageText">The data/text of the message</param>
        /// <param name="id">The messageid property column in the database </param>
        /// <param name="expirationTime">The time until the message should be processed</param>
        /// <param name="insertionTime">The date of the insertion into the queue</param>
        /// <param name="dequeueCount">It shows that how much times the message was gotten out of the queue</param>
        public SqlQueuedMessage(string messageText, string id, DateTime expirationTime,
            DateTime insertionTime, int dequeueCount)
        {
            MessageText = messageText;
            Id = id;
            InsertionTime = insertionTime;
            ExpirationTime = expirationTime;
            DequeueCount = dequeueCount;
        }

        /// <summary>
        /// </summary>
        /// <param name="messageText">The data/text of the message</param>
        /// <param name="id">The messageid property column in the database</param>
        /// <param name="timetoliveinseconds">The message should be processed until this time is up</param>
        /// <param name="insertionTime">The date of the insertion into the queue</param>
        /// <param name="dequeueCount">It shows that how much times the message was gotten out of the queue</param>
        public SqlQueuedMessage(string messageText, string id, int timetoliveinseconds,
            DateTime insertionTime, int dequeueCount) :
            this(messageText, id, EnvironmentInfo.GetCurrentDateTimeUtc()
            .AddSeconds(timetoliveinseconds), insertionTime, dequeueCount)
        { }

        /// <summary>
        /// The unique identifier of the message
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// The date of the insertion into the queue
        /// </summary>
        public DateTime InsertionTime { get; protected set; }

        /// <summary>
        /// The time until the message should be processed
        /// </summary>
        public DateTime ExpirationTime { get; protected set; }


        /// <summary>
        /// It shows that how much times the message was gotten out of the queue
        /// </summary>
        public int DequeueCount { get; protected set; }
    }
}