using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;

namespace SeemplestBlocks.Core.Email.DataAccess
{
    /// <summary>
    /// This interface defines operations that handle email data
    /// </summary>
    public interface IEmailDataOperations: IDataAccessOperation
    {
        /// <summary>
        /// Gets all Template records from the database
        /// </summary>
        /// <returns>
        /// List of Template records
        /// </returns>
        Task<List<TemplateRecord>> GetAllTemplateAsync();

        /// <summary>
        /// Gets the list of emails to be sent after the specified time
        /// </summary>
        /// <param name="sendAfter">The nearest time to send</param>
        /// <returns>List of emails to send</returns>
        Task<List<EmailToSendRecord>> GetEmailsToSendAsync(DateTimeOffset sendAfter);

        /// <summary>
        /// Inserts a EmailToSend record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertEmailToSendAsync(EmailToSendRecord record);

        /// <summary>
        /// Updates a EmailToSend record in the database
        /// </summary>
        /// <param name="record">Record to update</param>
        Task UpdateEmailToSendAsync(EmailToSendRecord record);

        /// <summary>
        /// Deletes a EmailToSend the specidfied record
        /// </summary>
        /// <param name="id">Id key value</param>
        Task DeleteEmailToSendAsync(int id);

        /// <summary>
        /// Inserts a EmailSent record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertEmailSentAsync(EmailSentRecord record);

        /// <summary>
        /// Inserts a EmailFailed record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        Task InsertEmailFailedAsync(EmailFailedRecord record);
       

    }
}