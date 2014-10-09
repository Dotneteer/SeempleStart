using Newtonsoft.Json;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.ServiceObjects;
using SeemplestBlocks.Core.Email.DataAccess;
using SeemplestBlocks.Dto.Email;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// This class implements a service to compose emails
    /// </summary>
    public class EmailComposerService : ServiceObjectBase, IEmailComposerService
    {
        /// <summary>
        /// Sends the email in the definition
        /// </summary>
        /// <param name="email">email definition</param>
        public void SendEmail(EmailDefinitionDto email)
        {
            Verify.NotNull(email, "email");
            Verify.RaiseWhenFailed();
            Verify.NotNullOrEmpty(email.FromAddr, "FromAddr");
            Verify.NotNullOrEmpty(email.ToAddr, "ToAddr");
            Verify.NotNullOrEmpty(email.TemplateType, "TemplateType");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateContext<IEmailDataOperations>())
            {
                string parameters = null;
                if (email.Parameters != null)
                {
                    parameters = JsonConvert.SerializeObject(email.Parameters);
                }
                string appointment = null;
                if (email.Appointment != null)
                {
                    appointment = JsonConvert.SerializeObject(email.Appointment);
                }

                ctx.InsertEmailToSendAsync(new EmailToSendRecord
                {
                    FromAddr = email.FromAddr,
                    FromName = email.FromName,
                    ToAddr = email.ToAddr,
                    TemplateType = email.TemplateType,
                    Parameters = parameters,
                    Appointment = appointment,
                    SendAfterUtc = email.SendAfterUtc,
                    RetryCount = 0,
                    LastError = null
                }).Wait();
            }
        }
    }
}