using SeemplestBlocks.Dto.Email;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// This interface defines the operations of an email sender service object
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Send the email defined with the specified parameters
        /// </summary>
        /// <param name="fromAddr">Address of the sender</param>
        /// <param name="fromName">Name of the sender</param>
        /// <param name="toAddrs">Addressees</param>
        /// <param name="subject">Subject</param>
        /// <param name="message">Message body</param>
        /// <param name="appointment">Aptional appointment data</param>
        void SendEmail(string fromAddr, string fromName, string[] toAddrs, string subject, string message,
            AppointmentDefinitionDto appointment = null);
    }
}