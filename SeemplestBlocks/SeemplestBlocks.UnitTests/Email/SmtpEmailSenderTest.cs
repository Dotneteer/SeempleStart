using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.AppConfig;
using SeemplestBlocks.Core.Email;
using SeemplestBlocks.Dto.Email;

namespace SeemplestBlocks.UnitTests.Email
{
    [TestClass]
    public class SmtpEmailSenderTest
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<IConfigurationReader, FakeConfigurationReader>();
        }

        [TestMethod]
        public void SendEmailWorksAsExpected()
        {
            // --- Arrange
            var sender = new SmtpEmailSender();

            // --- Act
            sender.SendEmail("dotneteer@hotmail.com", "Novák István", new[] { "inovak@grepton.hu" }, "Test Email",
                "This is a test email");

            // --- Assert
        }

        [TestMethod]
        public void SendEmailWithAppointmentWorksAsExpected()
        {
            // --- Arrange
            var sender = new SmtpEmailSender();

            // --- Act
            sender.SendEmail("dotneteer@hotmail.com", "Novák István", new[] { "inovak@grepton.hu" }, "Test Email",
                "This is a test email",
                new AppointmentDefinitionDto
                {
                    OrganizerEmail = "onovak@grpton.hu",
                    OrganizerName = "István Novák",
                    Date = new DateTime(2014, 8, 22),
                    StartHour = 8,
                    StartMinute = 30,
                    EndHour = 9,
                    EndMinute = 45,
                    Subject = "Test megbeszélés",
                    Description = "Megbeszéljuk a tesztet",
                    Location = "Nálunk",
                    Recepients = new[] {
                        "inovak@grepton.hu",
                        "vsxguy@gmail.com"
                    },
                    ReminderMinutesBefore = 15,
                    UseReminder = true
                });

            // --- Assert
        }
    }

    sealed class FakeConfigurationReader : IConfigurationReader
    {
        public bool GetConfigurationValue(string category, string key, out string value)
        {
            if (category == "SmtpConfig")
            {
                switch (key)
                {
                    case "Enabled":
                        value = "true";
                        return true;
                    case "ThrowException":
                        value = "false";
                        return true;
                    case "SmtpServer":
                        value = "smtp.gmail.com";
                        return true;
                    case "PortNumber":
                        value = "587";
                        return true;
                    case "UserName":
                        value = "seemplestcloud";
                        return true;
                    case "Password":
                        value = "@2014Seemplest";
                        return true;
                    case "UseSsl":
                        value = "true";
                        return true;
                    case "SmtpAuth":
                        value = "true";
                        return true;
                    case "MaxRetry":
                        value = "1";
                        return true;
                    case "RetryMinutes":
                        value = "5";
                        return true;
                    case "SendInterval":
                        value = "1000";
                        return true;
                    default:
                        value = null;
                        return false;
                }
            }
            value = null;
            return false;
        }
    }
}
