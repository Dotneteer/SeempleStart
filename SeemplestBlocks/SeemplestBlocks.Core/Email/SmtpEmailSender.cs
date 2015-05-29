using System;
using System.Net.Mail;
using System.Text;
using SeemplestBlocks.Core.Diagnostics;
using SeemplestBlocks.Dto.Email;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// This class uses SMTP to send out an email message
    /// </summary>
    public class SmtpEmailSender : IEmailSender
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
        public void SendEmail(string fromAddr, string fromName, string[] toAddrs, string subject, string message,
            AppointmentDefinitionDto appointment = null)
        {
            if (!SmtpConfig.Enabled) return;

            using (var smtp = new SmtpClient(SmtpConfig.SmtpServer, SmtpConfig.PortNumber))
            {
                smtp.EnableSsl = SmtpConfig.UseSsl;
                if (SmtpConfig.SmtpAuth)
                {
                    var smtpUserInfo = new System.Net.NetworkCredential(SmtpConfig.UserName, SmtpConfig.Password);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = smtpUserInfo;
                }

                var mail = new MailMessage
                {
                    Subject = subject,
                    Body = message,
                    From = new MailAddress(fromAddr, fromName),
                    IsBodyHtml = message.Contains("<html"),
                };

                foreach (var toAddr in toAddrs)
                {
                    if (toAddr.EndsWith("example") ||
                        toAddr.EndsWith("invalid") ||
                        toAddr.EndsWith("localhost") ||
                        toAddr.EndsWith("test") ||
                        toAddr.EndsWith("local"))
                    {
                        throw new InvalidOperationException("Domain name reserved by RFC 6761/6762, email cannot be sent.");
                    }

                    mail.To.Add(toAddr);
                }

                if (appointment != null)
                {
                    var contype = new System.Net.Mime.ContentType("text/calendar");
                    // ReSharper disable PossibleNullReferenceException
                    contype.Parameters.Add("method", "REQUEST");
                    contype.Parameters.Add("name", "Meeting.ics");
                    var avCal = AlternateView.CreateAlternateViewFromString(GetAppointmentCard(appointment), contype);
                    mail.AlternateViews.Add(avCal);
                    // ReSharper restore PossibleNullReferenceException
                }

                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    Tracer.LogError("SmtpEmailSender.SendEmail", "Sending an email failed", ex.ToString());
                    if (SmtpConfig.ThrowException)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a vCard for the appointment
        /// </summary>
        /// <param name="appointment">Appointment data</param>
        /// <returns>vCard representation</returns>
        private static string GetAppointmentCard(AppointmentDefinitionDto appointment)
        {
            var str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//MyCompany//NONSGML myProduct//EN");
            str.AppendLine("TZ:+1");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:REQUEST");
            str.AppendLine("BEGIN:VEVENT");
            str.AppendLine("CLASS:PUBLIC");
            str.AppendLine(string.Format("CREATED:{0:yyyyMMddTHHmmss}", DateTime.Now));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmss}", DateTime.Now));
            str.AppendLine(string.Format("DTSTART:{0}", GetFormattedDateTime(appointment.Date, appointment.StartHour, appointment.StartMinute)));
            str.AppendLine(string.Format("DTEND:{0}", GetFormattedDateTime(appointment.Date, appointment.EndHour, appointment.EndMinute)));
            str.AppendLine(string.Format("LOCATION:{0}", appointment.Location));
            str.AppendLine(string.Format("SUMMARY;LANGUAGE=hu:{0}", appointment.Subject));
            str.AppendLine(string.Format("DESCRIPTION:{0}", appointment.Description));
            str.AppendLine("TRANSP:OPAQUE");
            str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", appointment.OrganizerEmail));

            foreach (var recepient in appointment.Recepients)
            {
                str.AppendLine(string.Format("ATTENDEE;RSVP=TRUE:mailto:{0}", recepient));
            }

            if (appointment.UseReminder)
            {
                str.AppendLine("BEGIN:VALARM");
                str.AppendLine(string.Format("TRIGGER:-PT{0}M", appointment.ReminderMinutesBefore));
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine(string.Format("DESCRIPTION:{0}", appointment.Subject));
                str.AppendLine("END:VALARM");
            }

            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            return str.ToString();
        }

        private static string GetFormattedDateTime(DateTime date, int hour, int minute)
        {
            return String.Format("{0}{1:D2}{2:D2}T{3:D2}{4:D2}00",
                date.Year,
                date.Month,
                date.Day,
                hour,
                minute);
        }
    }
}