using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Seemplest.Core.Configuration;
using Seemplest.Core.Queue;
using Seemplest.MsSql.DataAccess;

namespace Seemplest.MsSql.Queue
{
    /// <summary>
    /// This class is a queue using SQL server
    /// </summary>
    public class SqlNamedQueue : INamedQueue
    {
        /// <summary>
        /// The id of the queue. It is needed to speed up the database transactions
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the provider used by this named queue
        /// </summary>
        public SqlNamedQueueProvider Provider { get; private set; }

        /// <summary>
        /// The constructor of the queue
        /// </summary>
        /// <param name="name">The name of the named queue</param>
        /// <param name="id">The id that should be from the database (identity)</param>
        /// <param name="provider">The provider that made the queue</param>
        internal SqlNamedQueue(string name, int id, SqlNamedQueueProvider provider)
        {
            Name = name;
            Id = id;
            Provider = provider;
        }

        /// <summary>
        /// The name of the queue. It is unique in the database
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// This function communicates with the database directly. Most of the message attributes in the 
        /// database are generated or gained from the queue itself.
        /// </summary>
        /// <param name="content">The content(text) of the message</param>
        /// <param name="timeToLiveInSeconds">
        /// The worker threads will try to process your message till 
        /// current time delayed by this parameter. After that, your message is dropped
        /// </param>
        public void PutMessage(string content, int timeToLiveInSeconds)
        {
            using (var conn = SqlHelper.CreateSqlConnection(Provider.NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.Parameters.Add("@queueid", SqlDbType.Int).Value = Id;
                    command.Parameters.Add("@visibilitystarttime", SqlDbType.DateTime).Value =
                        EnvironmentInfo.GetCurrentDateTimeUtc();
                    command.Parameters.Add("@messageid", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    command.Parameters.Add("@expitytime", SqlDbType.DateTime).Value =
                        EnvironmentInfo.GetCurrentDateTimeUtc().AddSeconds(timeToLiveInSeconds);
                    command.Parameters.Add("@insertiontime", SqlDbType.DateTime).Value =
                        EnvironmentInfo.GetCurrentDateTimeUtc();
                    command.Parameters.Add("@dequeuecount", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@data", SqlDbType.NVarChar).Value = content;
                    command.CommandText = GetInsertMessageSql();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// You mark the given amount of messages as invisible until the visibilityTimeoutInSeconds. You can freely process the messages then. All messages get a GUID named PopReceipt in the database
        /// </summary>
        /// <param name="numberOfMessages">The amount of messages you want to process</param>
        /// <param name="visibilityTimeoutInSeconds">The time you ask to process the data in seconds</param>
        /// <returns></returns>
        public IEnumerable<IPoppedMessage> GetMessages(int numberOfMessages, int visibilityTimeoutInSeconds)
        {
            List<IPoppedMessage> returnlist;
            using (var conn = SqlHelper.CreateSqlConnection(Provider.NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.Parameters.Add("@dequeueCount", SqlDbType.Int).Value = numberOfMessages;
                    command.Parameters.Add("@newVisibilityTime", SqlDbType.DateTime).Value =
                        EnvironmentInfo.GetCurrentDateTimeUtc().AddSeconds(visibilityTimeoutInSeconds);
                    command.Parameters.Add("@queueId", SqlDbType.Int).Value = Id;
                    command.Parameters.Add("@currentTime", SqlDbType.DateTime).Value =
                        EnvironmentInfo.GetCurrentDateTimeUtc();
                    command.CommandText = GetDequeueMessagesSql();
                    using (var reader = command.ExecuteReader())
                    {
                        returnlist = new List<IPoppedMessage>();
                        while (reader.Read())
                        {
                            returnlist.Add(new SqlPoppedMessage(
                                               new SqlQueuedMessage(
                                                   reader.GetString(6),
                                                   reader.GetGuid(2).ToString(),
                                                   reader.GetDateTime(3),
                                                   reader.GetDateTime(4),
                                                   reader.GetInt32(5)
                                                   ),
                                               visibilityTimeoutInSeconds,
                                               reader.GetGuid(7).ToString()));
                        }
                        reader.Close();

                        if (returnlist.Count == 0)
                        {
                            using (var doesExistCommand = new SqlCommand())
                            {
                                doesExistCommand.Connection = conn;
                                doesExistCommand.Parameters.Add("@queueid", SqlDbType.Int).Value = Id;
                                doesExistCommand.CommandText = GetSelectQueueSql();
                                using (var reader2 = doesExistCommand.ExecuteReader())
                                {
                                    if (reader2.HasRows == false)
                                        throw new InvalidOperationException("The queue was already deleted");
                                }
                            }
                        }

                    }
                }
            }
            return returnlist;
        }

        /// <summary>
        /// It gets the given number of messages, but leave them unmodified in the database
        /// </summary>
        /// <param name="numberOfMessages">The amount of messages you ask for</param>
        /// <returns>The Messages from the database</returns>
        public IEnumerable<IQueuedMessage> PeekMessages(int numberOfMessages)
        {
            List<IQueuedMessage> returnlist;
            using (var conn = SqlHelper.CreateSqlConnection(Provider.NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.Parameters.Add("@dequeueCount", SqlDbType.Int).Value = numberOfMessages;
                    command.Parameters.Add("@queueId", SqlDbType.Int).Value = Id;
                    command.Parameters.Add("@currentTime", SqlDbType.DateTime).Value =
                        EnvironmentInfo.GetCurrentDateTimeUtc();
                    command.CommandText = GetPeekMessagesSql();
                    using (var reader = command.ExecuteReader())
                    {
                        returnlist = new List<IQueuedMessage>();
                        while (reader.Read())
                        {
                            returnlist.Add(new SqlQueuedMessage(
                                               reader.GetString(6),
                                               reader.GetGuid(2).ToString(),
                                               reader.GetDateTime(3),
                                               reader.GetDateTime(4),
                                               reader.GetInt32(5)));
                        }
                        reader.Close();
                        if (returnlist.Count == 0)
                        {
                            using (var doesExistCommand = new SqlCommand())
                            {
                                doesExistCommand.Connection = conn;
                                doesExistCommand.Parameters.Add("@queueid", SqlDbType.Int).Value = Id;
                                doesExistCommand.CommandText = GetSelectQueueSql();
                                using (var reader2 = doesExistCommand.ExecuteReader())
                                {
                                    if (reader2.HasRows == false)
                                        throw new InvalidOperationException("The queue was already deleted");
                                }
                            }
                        }
                    }
                }
            }

            return returnlist;
        }

        /// <summary>
        /// This method will delete the popped message from the queue. Does nothing when message does not exist
        /// </summary>
        /// <param name="poppedMessage">The nessage you want to delete</param>
        public void DeleteMessage(IPoppedMessage poppedMessage)
        {
            using (var conn = SqlHelper.CreateSqlConnection(Provider.NameOrConnectionString))
            {
                conn.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.Parameters.Add("@messageid", SqlDbType.UniqueIdentifier).Value =
                        Guid.Parse(poppedMessage.Id);
                    command.Parameters.Add("@popreceipt", SqlDbType.UniqueIdentifier).Value =
                        Guid.Parse(poppedMessage.PopReceipt);
                    command.CommandText = GetDeleteMessagesSql();
                    var rows = command.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        throw new InvalidOperationException("No message has been deleted.");
                    }
                }
            }
        }

        /// <summary>
        /// It deletes all messages that are in the queue from the database
        /// </summary>
        public void Clear()
        {
            using (var conn = SqlHelper.CreateSqlConnection(Provider.NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(GetClearMessageQueueSql(), conn))
                {
                    command.Parameters.Add("@queueid", SqlDbType.Int).Value = Id;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the approximate count of messages.
        /// </summary>
        /// <returns>Approximate count of messages</returns>
        public int GetApproximateMessageCount()
        {
            using (var conn = SqlHelper.CreateSqlConnection(Provider.NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(GetMessageCountSql(), conn))
                {
                    command.Parameters.Add("@queueid", SqlDbType.Int).Value = Id;
                    return (int)command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Gets the SQL statement inserting a new message
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetInsertMessageSql()
        {
            return string.Format(
                @"INSERT INTO {0}
                  (
                      [QueueID],
                      [VisibilityStartTime],
                      [MessageId],
                      [ExpiryTime],
                      [InsertionTime],
                      [DequeueCount],
                      [Data]
                  )
                  VALUES
                  (
                      @queueid,
                      @visibilitystarttime,
                      @messageid,
                      @expitytime,
                      @insertiontime,
                      @dequeuecount,
                      @data
                  )",
                Provider.MessageTableName);
        }

        /// <summary>
        /// Gets the SQL statement dequeueing messages
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetDequeueMessagesSql()
        {
            return string.Format(
                @"DECLARE @MYGUID uniqueidentifier
                  SET @MYGUID = NEWID()
                  UPDATE TOP(@dequeueCount) {0}
                      SET VisibilityStartTime = @newVisibilityTime,
                          DequeueCount = DequeueCount + 1,
                          PopReceipt = @MYGUID
                      OUTPUT inserted.[QueueID],
                          inserted.[VisibilityStartTime],
                          inserted.[MessageId],
                          inserted.[ExpiryTime],
                          inserted.[InsertionTime],
                          inserted.[DequeueCount],
                          inserted.[Data],
                          inserted.[PopReceipt]
                      WHERE QueueID = @queueID
                          AND VisibilityStartTime <= @currentTime
                          AND ExpiryTime >= @currentTime",
                Provider.MessageTableName);
        }

        /// <summary>
        /// Gets the SQL statement selecting a queue
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetSelectQueueSql()
        {
            return string.Format(
                @"SELECT [QueueId], [QueueName]
                  FROM {0} 
                  WHERE [QueueId]=@queueid",
                Provider.QueueTableName);
        }

        /// <summary>
        /// Gets the SQL statement peeking messages
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetPeekMessagesSql()
        {
            return string.Format(
                @"SELECT TOP(@dequeueCount)
                      [QueueID],
                      [VisibilityStartTime],
                      [MessageId],
                      [ExpiryTime],
                      [InsertionTime],
                      [DequeueCount],
                      [Data]
                  FROM {0}
                  WHERE QueueID = @queueID
                      AND VisibilityStartTime <= @currentTime
                      AND ExpiryTime >= @currentTime",
                Provider.MessageTableName);
        }

        /// <summary>
        /// Gets the SQL statement deleting messages
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetDeleteMessagesSql()
        {
            return string.Format(
                @"DELETE FROM {0}
                  WHERE PopReceipt=@popreceipt AND MessageId=@messageid",
                Provider.MessageTableName);
        }

        /// <summary>
        /// Gets the SQL statement clearing a message queue
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetClearMessageQueueSql()
        {
            return string.Format(
                 @"DELETE FROM {0}
                   WHERE QueueID=@queueid",
                Provider.MessageTableName);
        }

        /// <summary>
        /// Gets the SQL statement clearing a message queue
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetMessageCountSql()
        {
            return string.Format(
                 @"SELECT COUNT(*) FROM {0}
                   WHERE QueueID=@queueid",
                Provider.MessageTableName);
        }
    }
}