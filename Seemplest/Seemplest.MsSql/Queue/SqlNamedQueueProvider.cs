using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Seemplest.Core.Queue;
using Seemplest.MsSql.DataAccess;

namespace Seemplest.MsSql.Queue
{
    /// <summary>
    /// This class is a provider that helps you to manage your mssql queues. 
    /// </summary>
    public class SqlNamedQueueProvider : INamedQueueProvider
    {
        /// <summary>
        /// Default name of the table storing queue information
        /// </summary>
        public const string QUEUE_TABLE_NAME = "[Messages].[Queue]";

        /// <summary>
        /// Default name of the table storing message information
        /// </summary>
        public const string MESSAGE_TABLE_NAME = "[Messages].[QueueMessage]";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlNamedQueueProvider"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public SqlNamedQueueProvider(string nameOrConnectionString)
            : this(nameOrConnectionString,
                QUEUE_TABLE_NAME, MESSAGE_TABLE_NAME)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlNamedQueueProvider"/> class
        /// using the specified table names.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        /// <param name="queueTableName">The name of the table storing queue information</param>
        /// <param name="messageTableName">The name of the table storing message information</param>
        public SqlNamedQueueProvider(string nameOrConnectionString,
            string queueTableName,
            string messageTableName)
        {
            if (nameOrConnectionString == null) throw new ArgumentNullException("nameOrConnectionString");
            NameOrConnectionString = nameOrConnectionString;
            QueueTableName = queueTableName;
            MessageTableName = messageTableName;
        }

        /// <summary>
        /// Gets or sets the (indirect) connection string of this provider.
        /// </summary>
        /// <remarks>
        /// The connection can be provided in two ways: use a direct connection string, or
        /// use the 'name=connName' form, where connName refers to an element in the
        /// connectionStrings section of the application configuration file.
        /// </remarks>
        public string NameOrConnectionString { get; private set; }

        /// <summary>
        /// Gets the name of the table storing queue information
        /// </summary>
        public string QueueTableName { get; private set; }

        /// <summary>
        /// Gets the name of the tables storing message information
        /// </summary>
        public string MessageTableName { get; private set; }

        /// <summary>
        /// Gets the queue that has the given name from the database
        /// </summary>
        /// <param name="name">The name of the queue you want</param>
        /// <returns>The queue</returns>
        public INamedQueue GetQueue(string name)
        {
            INamedQueue retqueue = null;
            using (var conn = SqlHelper.CreateSqlConnection(NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.Parameters.Add("@name", SqlDbType.NVarChar, 63).Value = name;
                    command.CommandText = GetSelectQueueSql();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            retqueue =
                                new SqlNamedQueue(reader.GetString(1), reader.GetInt32(0), this);
                        }
                    }
                }
            }
            return retqueue;
        }

        /// <summary>
        /// This method gets all the queues from the database and returns them as a List
        /// </summary>
        /// <returns>The list of the queues</returns>
        public IEnumerable<INamedQueue> ListQueues()
        {
            var returnlist = new List<INamedQueue>();
            using (var conn = SqlHelper.CreateSqlConnection(NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(GetSelectAllQueueSql(), conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnlist.Add(
                                new SqlNamedQueue(reader.GetString(1), reader.GetInt32(0), this));
                        }
                    }
                }
            }
            return returnlist;
        }

        /// <summary>
        /// Creates a new queue with the given name
        /// </summary>
        /// <param name="name">The name you want for the queue</param>
        /// <returns>The queue you have just created</returns>
        public INamedQueue CreateQueue(string name)
        {
            INamedQueue queue;
            using (var conn = SqlHelper.CreateSqlConnection(NameOrConnectionString))
            {
                conn.Open();
                using (var commandInsert = new SqlCommand())
                {
                    commandInsert.Parameters.Add("@queuename", SqlDbType.NVarChar).Value = name;
                    commandInsert.CommandText = GetInsertQueueSql();
                    commandInsert.Connection = conn;
                    using (var reader = commandInsert.ExecuteReader())
                    {
                        reader.Read();
                        queue = new SqlNamedQueue(name, (int)reader.GetDecimal(0), this);
                    }
                }
            }
            return queue;
        }

        /// <summary>
        /// Deletes the queue from the database. Does nothing if queue does not exist
        /// </summary>
        /// <param name="name">The name of the queue you want to delete</param>
        public void DeleteQueue(string name)
        {
            using (var conn = SqlHelper.CreateSqlConnection(NameOrConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.Parameters.Add("@queuename", SqlDbType.NVarChar, 63).Value = name;
                    command.CommandText = GetDeleteQueueSql();
                    var todeletequeue = GetQueue(name);
                    if (todeletequeue != null) todeletequeue.Clear();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the SQL statement selecting a queue;
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetSelectQueueSql()
        {
            return string.Format(
                @"SELECT [QueueId], [QueueName] FROM {0} WHERE QueueName=@name",
                QueueTableName);
        }

        /// <summary>
        /// Gets the SQL statement selecting all queue;
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetSelectAllQueueSql()
        {
            return string.Format(
                @"SELECT [QueueId], [QueueName] FROM {0}",
                QueueTableName);
        }

        /// <summary>
        /// Gets the SQL statement inserting a new queue record;
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetInsertQueueSql()
        {
            return string.Format(
                @"INSERT INTO {0} (QueueName)
                          VALUES (@queuename)
                          SELECT SCOPE_IDENTITY() as ID",
                QueueTableName);
        }

        /// <summary>
        /// Gets the SQL statement selecting a queue;
        /// </summary>
        /// <returns>SQL statement</returns>
        protected virtual string GetDeleteQueueSql()
        {
            return string.Format(
                @"DELETE FROM {0} WHERE QueueName=@queuename",
                QueueTableName);
        }
    }
}