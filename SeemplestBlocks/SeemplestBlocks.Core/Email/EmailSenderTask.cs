using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.Tasks;
using SeemplestBlocks.Core.Email.DataAccess;
using SeemplestBlocks.Dto.Email;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// This background task is responsible for sending emails
    /// </summary>
    public class EmailSenderTask : TaskBase
    {
        private static readonly object s_Locker = new object();

        /// <summary>
        /// Record holding email template definitions
        /// </summary>
        public static List<TemplateRecord> TemplateDefinitions { get; private set; }

        /// <summary>
        /// Clears the cache of email templates
        /// </summary>
        public static void ResetCache()
        {
            lock (s_Locker)
            {
                TemplateDefinitions = null;
            }
        }

        /// <summary>
        /// Sets up the task by loading email template definitions
        /// </summary>
        /// <param name="context">Execution context</param>
        public override void Setup(ITaskExecutionContext context)
        {
            lock (s_Locker)
            {
                if (TemplateDefinitions == null)
                {
                    using (var ctx = DataAccessFactory.CreateReadOnlyContext<IEmailDataOperations>())
                    {

                        TemplateDefinitions = ctx.GetAllTemplateAsync().Result;
                    }
                }
            }
            base.Setup(context);
        }

        /// <summary>
        /// Runs the specific task.
        /// </summary>
        public override void Run()
        {
            // --- Get the emails waiting to be sent
            List<EmailToSendRecord> emailsToSend;
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IEmailDataOperations>())
            {
                var emailCount = SmtpConfig.EmailCount;
                if (emailCount <= 1)
                {
                    emailCount = 1;
                }
                emailsToSend = ctx.GetEmailsToSendAsync(DateTimeOffset.UtcNow).Result.Take(emailCount).ToList();
            }

            // --- Process each email to send
            foreach (var emailToSend in emailsToSend)
            {
                // --- Assemble email body
                var templateId = emailToSend.TemplateType;
                var template = TemplateDefinitions.FirstOrDefault(t => t.Id == templateId);
                if (template == null)
                {
                    // --- Do not send emails without a template
                    emailToSend.RetryCount = SmtpConfig.MaxRetry;
                    ProcessEmail(emailToSend,
                        null,
                        null,
                        null,
                        string.Format("Email template '{0}' not found.", templateId));
                    continue;
                }

                // --- Set the email body
                var subject = template.Subject;
                var message = template.Body;
                try
                {
                    var parameters = GetEmailParametersDictionary(emailToSend.Parameters);
                    foreach (var key in parameters.Keys)
                    {
                        var placeholder = "{{" + key + "}}";
                        message = message.Replace(placeholder, parameters[key]);
                        subject = subject.Replace(placeholder, parameters[key]);
                    }
                }
                catch (Exception ex)
                {
                    // --- Email body assembly failed
                    emailToSend.RetryCount = SmtpConfig.MaxRetry;
                    ProcessEmail(emailToSend,
                        null,
                        null,
                        null,
                        string.Format("Email message construction failed. {0}", ex));
                    continue;
                }

                // --- Send out the email
                try
                {
                    AppointmentDefinitionDto appointmentInfo = null;
                    if (emailToSend.Appointment != null)
                    {
                        appointmentInfo =
                            JsonConvert.DeserializeObject<AppointmentDefinitionDto>(emailToSend.Appointment);
                    }
                    var senderService = ServiceManager.GetService<IEmailSender>();
                    senderService.SendEmail(
                        emailToSend.FromAddr,
                        emailToSend.FromName,
                        new[] { emailToSend.ToAddr },
                        subject,
                        message,
                        appointmentInfo);
                    ProcessEmail(emailToSend, subject, message, emailToSend.Appointment, null);
                }
                catch (Exception ex)
                {
                    ProcessEmail(emailToSend, null, null, null, ex.ToString());
                }
            }
            Thread.Sleep(SmtpConfig.SendInterval);
        }

        /// <summary>
        /// Processes the specified email
        /// </summary>
        /// <param name="emailToSend">Email data</param>
        /// <param name="subject">Subject</param>
        /// <param name="emailMessage">Message body</param>
        /// <param name="appointment">Optional appointment info</param>
        /// <param name="errorMessage">Error messages</param>
        private static void ProcessEmail(EmailToSendRecord emailToSend, string subject, string emailMessage,
            string appointment, string errorMessage)
        {
            using (var ctx = DataAccessFactory.CreateContext<IEmailDataOperations>())
            {
                ctx.BeginTransaction();

                if (errorMessage == null)
                {
                    ctx.InsertEmailSentAsync(new EmailSentRecord
                    {
                        Id = emailToSend.Id,
                        FromAddr = emailToSend.FromAddr,
                        FromName = emailToSend.FromName,
                        ToAddr = emailToSend.ToAddr,
                        Subject = subject,
                        Message = emailMessage,
                        Appointment = appointment,
                        SentUtc = DateTime.Now
                    }).Wait();
                    ctx.DeleteEmailToSendAsync(emailToSend.Id).Wait();
                }
                else if (emailToSend.RetryCount < SmtpConfig.MaxRetry)
                {
                    emailToSend.LastError = errorMessage;
                    emailToSend.RetryCount++;
                    emailToSend.SendAfterUtc = DateTimeOffset.UtcNow.AddMinutes(SmtpConfig.RetryMinutes);
                    ctx.UpdateEmailToSendAsync(emailToSend).Wait();
                }
                else
                {
                    ctx.InsertEmailFailedAsync(new EmailFailedRecord
                    {
                        Id = emailToSend.Id,
                        FromAddr = emailToSend.FromAddr,
                        FromName = emailToSend.FromName,
                        ToAddr = emailToSend.ToAddr,
                        Subject = subject,
                        Message = emailMessage,
                        LastError = errorMessage,
                        RetryCount = emailToSend.RetryCount,
                        FailedUtc = DateTime.Now
                    });
                    ctx.DeleteEmailToSendAsync(emailToSend.Id).Wait();
                }

                ctx.Complete();
            }
        }

        /// <summary>
        /// Creates the dictionary describing email parameters
        /// </summary>
        /// <param name="parameters">JSON string with parameters</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEmailParametersDictionary(string parameters)
        {
            var result = new Dictionary<string, string>();
            return parameters == null
                ? result
                : JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters);
        }
    }
}