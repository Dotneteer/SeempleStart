using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using Seemplest.MsSql.DataAccess;
using SeemplestBlocks.Core.AppConfig;
using SeemplestBlocks.Core.Email;
using SeemplestBlocks.Core.Email.DataAccess;
using SeemplestBlocks.Dto.Email;
using SeemplestBlocks.UnitTests.Helpers;
using SoftwareApproach.TestingExtensions;

namespace SeemplestBlocks.UnitTests.Email
{
    [TestClass]
    public class EmailSenderTaskTest
    {
        private const string DB_CONN = "connStr=Core";

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<IEmailDataOperations, EmailDataOperations>(DB_CONN);
            ServiceManager.Register<IEmailComposerService, EmailComposerService>();
            ServiceManager.Register<IConfigurationReader, FakeConfigurationReader>();
            FakeConfigurationReader.Reset();
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
        }

        [TestInitialize]
        public void Initialize()
        {
            SqlScriptHelper.RunScript("InitEmailTemplates.sql");
            EmailSenderTask.ResetCache();
        }

        [TestMethod]
        public void SetupReadsTemplates()
        {
            // --- Arrange
            var sendertask = new EmailSenderTask();

            // --- Act
            sendertask.Setup(new EmailTaskExecutionContext());

            // --- Assert
            EmailSenderTask.TemplateDefinitions.ShouldHaveCountOf(6);
        }

        [TestMethod]
        public void SetupCachesTemplates()
        {
            // --- Arrange
            var sendertask = new EmailSenderTask();

            // --- Act
            var templ1 = EmailSenderTask.TemplateDefinitions;
            sendertask.Setup(new EmailTaskExecutionContext());
            var templ2 = EmailSenderTask.TemplateDefinitions;
            sendertask.Setup(new EmailTaskExecutionContext());
            var templ3 = EmailSenderTask.TemplateDefinitions;

            // --- Assert
            templ1.ShouldBeNull();
            templ2.ShouldNotBeNull();
            templ3.ShouldNotBeNull();
            templ2.ShouldBeSameAs(templ3);
        }

        [TestMethod]
        public void EmailsProcessedSuccessfully()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IEmailComposerService>();
            var sendertask = new EmailSenderTask();
            ServiceManager.RemoveService(typeof(IEmailSender));
            ServiceManager.Register<IEmailSender, AlwaysWorkingEmailSender>();
            for (var i = 0; i < 5; i++)
            {
                var request = new EmailDefinitionDto
                {
                    FromAddr = "youremail@email.com",
                    FromName = "You",
                    ToAddr = "myemail@email.com",
                    TemplateType = "MeetingProposed",
                    Parameters = new Dictionary<string, string> { { "FullName", "MyName" } },
                    SendAfterUtc = null
                };
                service.SendEmail(request);
            }

            // --- Act
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "EmailCount", "4");
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "MaxRetry", "0");
            sendertask.Setup(new EmailTaskExecutionContext());
            sendertask.Run();

            // --- Assert
            var db = new SqlDatabase(DB_CONN);
            var emailsToSend = db.Fetch<EmailToSendRecord>();
            emailsToSend.ShouldHaveCountOf(1);
            var emailsSent = db.Fetch<EmailSentRecord>();
            emailsSent.ShouldHaveCountOf(4);
            var emailsFailed = db.Fetch<EmailFailedRecord>();
            emailsFailed.ShouldHaveCountOf(0);

            foreach (var email in emailsSent)
            {
                email.Subject.ShouldEqual("Evaluation meeting proposed");
                email.Message.ShouldContain("MyName");
                email.Message.ShouldNotContain("FullName");
            }
        }

        [TestMethod]
        public void EmailsProcessedSuccessfullyWithMultipleIntervals()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IEmailComposerService>();
            var sendertask = new EmailSenderTask();
            ServiceManager.RemoveService(typeof(IEmailSender));
            ServiceManager.Register<IEmailSender, AlwaysWorkingEmailSender>();
            for (var i = 0; i < 10; i++)
            {
                var request = new EmailDefinitionDto
                {
                    FromAddr = "youremail@email.com",
                    FromName = "You",
                    ToAddr = "myemail@email.com",
                    TemplateType = "MeetingProposed",
                    Parameters = new Dictionary<string, string> { { "FullName", "MyName" } },
                    SendAfterUtc = null
                };
                service.SendEmail(request);
            }

            // --- Act
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "EmailCount", "3");
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "MaxRetry", "0");
            sendertask.Setup(new EmailTaskExecutionContext());
            sendertask.Run();
            sendertask.Run();

            // --- Assert
            var db = new SqlDatabase(DB_CONN);
            var emailsToSend = db.Fetch<EmailToSendRecord>();
            emailsToSend.ShouldHaveCountOf(4);
            var emailsSent = db.Fetch<EmailSentRecord>();
            emailsSent.ShouldHaveCountOf(6);
            var emailsFailed = db.Fetch<EmailFailedRecord>();
            emailsFailed.ShouldHaveCountOf(0);

            foreach (var email in emailsSent)
            {
                email.Subject.ShouldEqual("Evaluation meeting proposed");
                email.Message.ShouldContain("MyName");
                email.Message.ShouldNotContain("FullName");
            }
        }

        [TestMethod]
        public void EmailsFailWithUnknownTemplate()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IEmailComposerService>();
            var sendertask = new EmailSenderTask();
            ServiceManager.RemoveService(typeof(IEmailSender));
            ServiceManager.Register<IEmailSender, AlwaysWorkingEmailSender>();
            for (var i = 0; i < 5; i++)
            {
                var request = new EmailDefinitionDto
                {
                    FromAddr = "youremail@email.com",
                    FromName = "You",
                    ToAddr = "myemail@email.com",
                    TemplateType = "Unknown",
                    Parameters = new Dictionary<string, string> { { "FullName", "MyName" } },
                    SendAfterUtc = null
                };
                service.SendEmail(request);
            }

            // --- Act
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "EmailCount", "4");
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "MaxRetry", "0");
            sendertask.Setup(new EmailTaskExecutionContext());
            sendertask.Run();

            // --- Assert
            var db = new SqlDatabase(DB_CONN);
            var emailsToSend = db.Fetch<EmailToSendRecord>();
            emailsToSend.ShouldHaveCountOf(1);
            var emailsSent = db.Fetch<EmailSentRecord>();
            emailsSent.ShouldHaveCountOf(0);
            var emailsFailed = db.Fetch<EmailFailedRecord>();
            emailsFailed.ShouldHaveCountOf(4);

            foreach (var email in emailsFailed)
            {
                email.Subject.ShouldBeNull();
                email.Message.ShouldBeNull();
                email.LastError.ShouldContain("not found");
            }
        }

        [TestMethod]
        public void EmailsFailWithProcessingException()
        {
            // --- Arrange
            var db = new SqlDatabase(DB_CONN);
            var sendertask = new EmailSenderTask();
            ServiceManager.RemoveService(typeof(IEmailSender));
            ServiceManager.Register<IEmailSender, AlwaysWorkingEmailSender>();
            for (var i = 0; i < 5; i++)
            {
                db.Insert(new EmailToSendRecord
                {
                    FromAddr = "me@email.com",
                    FromName = "Me",
                    ToAddr = "you@email.com",
                    TemplateType = "MeetingProposed",
                    Parameters = "<dummy><Parameter/></dummy>"
                });
            }

            // --- Act
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "EmailCount", "4");
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "MaxRetry", "0");
            sendertask.Setup(new EmailTaskExecutionContext());
            sendertask.Run();

            // --- Assert
            var emailsToSend = db.Fetch<EmailToSendRecord>();
            emailsToSend.ShouldHaveCountOf(1);  //1
            var emailsSent = db.Fetch<EmailSentRecord>();
            emailsSent.ShouldHaveCountOf(0);
            var emailsFailed = db.Fetch<EmailFailedRecord>();
            emailsFailed.ShouldHaveCountOf(4);

            foreach (var email in emailsFailed)
            {
                email.Subject.ShouldBeNull();
                email.Message.ShouldBeNull();
                email.LastError.ShouldContain("Email message");
            }
        }

        [TestMethod]
        public void EmailsFailsWithSenderException()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IEmailComposerService>();
            var sendertask = new EmailSenderTask();
            ServiceManager.RemoveService(typeof(IEmailSender));
            ServiceManager.Register<IEmailSender, AlwaysFailingEmailSender>();
            for (var i = 0; i < 5; i++)
            {
                var request = new EmailDefinitionDto
                {
                    FromAddr = "youremail@email.com",
                    FromName = "You",
                    ToAddr = "myemail@email.com",
                    TemplateType = "MeetingProposed",
                    Parameters = new Dictionary<string, string> { { "FullName", "MyName" } },
                    SendAfterUtc = null
                };
                service.SendEmail(request);
            }

            // --- Act
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "EmailCount", "4");
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "MaxRetry", "0");
            sendertask.Setup(new EmailTaskExecutionContext());
            sendertask.Run();

            // --- Assert
            var db = new SqlDatabase(DB_CONN);
            var emailsToSend = db.Fetch<EmailToSendRecord>();
            emailsToSend.ShouldHaveCountOf(1);
            var emailsSent = db.Fetch<EmailSentRecord>();
            emailsSent.ShouldHaveCountOf(0);
            var emailsFailed = db.Fetch<EmailFailedRecord>();
            emailsFailed.ShouldHaveCountOf(4);

            foreach (var email in emailsFailed)
            {
                email.LastError.ShouldContain("Sender failed");
            }
        }

        [TestMethod]
        public void RetryFailsAfterMaximum()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IEmailComposerService>();
            var sendertask = new EmailSenderTask();
            ServiceManager.RemoveService(typeof(IEmailSender));
            ServiceManager.Register<IEmailSender, EmailSenderWithRetry>();
            for (var i = 0; i < 5; i++)
            {
                var request = new EmailDefinitionDto
                {
                    FromAddr = "youremail@email.com",
                    FromName = "You",
                    ToAddr = "myemail@email.com",
                    TemplateType = "MeetingProposed",
                    Parameters = new Dictionary<string, string> { { "FullName", "MyName" } },
                    SendAfterUtc = null
                };
                service.SendEmail(request);
            }

            // --- Act
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "EmailCount", "4");
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "MaxRetry", "3");
            EmailSenderWithRetry.ForceFailure = true;

            sendertask.Setup(new EmailTaskExecutionContext());
            sendertask.Run();
            sendertask.Run();
            sendertask.Run();
            sendertask.Run();

            // --- Assert
            var db = new SqlDatabase(DB_CONN);
            var emailsToSend = db.Fetch<EmailToSendRecord>();
            emailsToSend.ShouldHaveCountOf(1);
            var emailsSent = db.Fetch<EmailSentRecord>();
            emailsSent.ShouldHaveCountOf(0);
            var emailsFailed = db.Fetch<EmailFailedRecord>();
            emailsFailed.ShouldHaveCountOf(4);

            foreach (var email in emailsFailed)
            {
                email.LastError.ShouldContain("Sender failed");
                email.RetryCount.ShouldEqual(3);
            }
        }

        [TestMethod]
        public void RetryWorksAsExpected()
        {
            // --- Arrange
            var service = ServiceManager.GetService<IEmailComposerService>();
            var sendertask = new EmailSenderTask();
            ServiceManager.RemoveService(typeof(IEmailSender));
            ServiceManager.Register<IEmailSender, EmailSenderWithRetry>();
            for (var i = 0; i < 5; i++)
            {
                var request = new EmailDefinitionDto
                {
                    FromAddr = "youremail@email.com",
                    FromName = "You",
                    ToAddr = "myemail@email.com",
                    TemplateType = "MeetingProposed",
                    Parameters = new Dictionary<string, string> { { "FullName", "MyName" } },
                    SendAfterUtc = null
                };
                service.SendEmail(request);
            }

            // --- Act
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "EmailCount", "4");
            FakeConfigurationReader.SetConfigurationValue("SmtpConfig", "MaxRetry", "3");
            EmailSenderWithRetry.ForceFailure = true;

            sendertask.Setup(new EmailTaskExecutionContext());
            sendertask.Run();
            sendertask.Run();
            EmailSenderWithRetry.ForceFailure = false;
            sendertask.Run();

            // --- Assert
            var db = new SqlDatabase(DB_CONN);
            var emailsToSend = db.Fetch<EmailToSendRecord>();
            emailsToSend.ShouldHaveCountOf(1);
            var emailsSent = db.Fetch<EmailSentRecord>();
            emailsSent.ShouldHaveCountOf(4);
            var emailsFailed = db.Fetch<EmailFailedRecord>();
            emailsFailed.ShouldHaveCountOf(0);

            foreach (var email in emailsSent)
            {
                email.Subject.ShouldEqual("Evaluation meeting proposed");
                email.Message.ShouldContain("MyName");
                email.Message.ShouldNotContain("FullName");
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class AlwaysWorkingEmailSender : IEmailSender
        {
            public void SendEmail(string fromAddr, string fromName, string[] toAddrs, string subject, string message,
                AppointmentDefinitionDto appointment = null)
            {
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class AlwaysFailingEmailSender : IEmailSender
        {
            public void SendEmail(string fromAddr, string fromName, string[] toAddrs, string subject, string message,
                AppointmentDefinitionDto appointment = null)
            {
                throw new InvalidOperationException("Sender failed");
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class EmailSenderWithRetry : IEmailSender
        {
            public static bool ForceFailure;

            public void SendEmail(string fromAddr, string fromName, string[] toAddrs, string subject, string message,
                AppointmentDefinitionDto appointment = null)
            {
                if (ForceFailure) throw new InvalidOperationException("Sender failed");
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class FakeConfigurationReader : IConfigurationReader
        {
            private static readonly Dictionary<string, string> s_Configs = new Dictionary<string, string>();

            public static void Reset()
            {
                s_Configs.Clear();
            }

            public static void SetConfigurationValue(string category, string key, string value)
            {
                var configKey = string.Format("{0}.{1}", category, key);
                s_Configs[configKey] = value;
            }

            public bool GetConfigurationValue(string category, string key, out string value)
            {
                var configKey = string.Format("{0}.{1}", category, key);
                return s_Configs.TryGetValue(configKey, out value);
            }
        }
    }
}
