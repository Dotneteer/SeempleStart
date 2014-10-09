using Seemplest.Core.ServiceObjects;
using SeemplestBlocks.Dto.Email;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// This interface defines a service that sends emails physically
    /// </summary>
    public interface IEmailComposerService : IServiceObject
    {
        /// <summary>
        /// Sends the email in the definition
        /// </summary>
        /// <param name="email">email definition</param>
        void SendEmail(EmailDefinitionDto email);
    }
}