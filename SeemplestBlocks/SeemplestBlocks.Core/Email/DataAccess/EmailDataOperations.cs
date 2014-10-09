using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeemplestBlocks.Core.ServiceInfrastructure;

namespace SeemplestBlocks.Core.Email.DataAccess
{
    /// <summary>
    /// This class implements operations that handle email data
    /// </summary>
    public class EmailDataOperations : CoreSqlDataAccessOperationBase, IEmailDataOperations
    {
        /// <summary>
        /// Initializes the object with the specified connection information
        /// </summary>
        /// <param name="connectionOrName">Connection information</param>
        public EmailDataOperations(string connectionOrName)
            : base(connectionOrName)
        {
        }

        /// <summary>
        /// Gets all Template records from the database
        /// </summary>
        /// <returns>
        /// List of Template records
        /// </returns>
        public async Task<List<TemplateRecord>> GetAllTemplateAsync()
        {
            return await OperationAsync(ctx => ctx.FetchAsync<TemplateRecord>());
        }

        /// <summary>
        /// Gets the list of emails to be sent after the specified time
        /// </summary>
        /// <param name="sendAfter">The nearest time to send</param>
        /// <returns>List of emails to send</returns>
        public async Task<List<EmailToSendRecord>> GetEmailsToSendAsync(DateTimeOffset sendAfter)
        {
            return await OperationAsync(ctx => ctx.FetchAsync<EmailToSendRecord>(
                "where [SendAfterUtc] is null or [SendAfterUtc]<=@0", sendAfter));
        }

        /// <summary>
        /// Inserts a EmailToSend record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertEmailToSendAsync(EmailToSendRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }

        /// <summary>
        /// Updates a EmailToSend record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        public async Task UpdateEmailToSendAsync(EmailToSendRecord record)
        {
            await OperationAsync(ctx => ctx.UpdateAsync(record));
        }

        /// <summary>
        /// Deletes a EmailToSend the specidfied record
        /// </summary>
        /// <param name="id">Id key value</param>
        public async Task DeleteEmailToSendAsync(int id)
        {
            await OperationAsync(ctx => ctx.DeleteByIdAsync<EmailToSendRecord>(
                id));
        }

        /// <summary>
        /// Inserts a EmailSent record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertEmailSentAsync(EmailSentRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }

        /// <summary>
        /// Inserts a EmailFailed record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public async Task InsertEmailFailedAsync(EmailFailedRecord record)
        {
            await OperationAsync(ctx => ctx.InsertAsync(record));
        }
    }
}