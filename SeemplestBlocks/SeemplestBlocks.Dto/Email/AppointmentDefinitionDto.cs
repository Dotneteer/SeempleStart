using System;

namespace SeemplestBlocks.Dto.Email
{
    /// <summary>
    /// Ez az osztály egy találkozó szervezéséhez szükséges adatokat ír le.
    /// </summary>
    public class AppointmentDefinitionDto
    {
        /// <summary>
        /// A szervező email címe
        /// </summary>
        public string OrganizerEmail { get; set; }

        /// <summary>
        /// A szervező neve
        /// </summary>
        public string OrganizerName { get; set; }

        /// <summary>
        /// A meghívottak email címei
        /// </summary>
        public string[] Recepients { get; set; }

        /// <summary>
        /// A találkozó tárgya
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// A találkozó leírása
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A találkozó helyszíne
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// A megbeszélés dátuma
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Az esemény kezdete (óra)
        /// </summary>
        public int StartHour { get; set; }

        /// <summary>
        /// Az esemény kezdete (perc)
        /// </summary>
        public int StartMinute { get; set; }

        /// <summary>
        /// Az esemény vége (óra)
        /// </summary>
        public int EndHour { get; set; }

        /// <summary>
        /// Az esemény vége (perc)
        /// </summary>
        public int EndMinute { get; set; }

        /// <summary>
        /// Kell emlékeztető?
        /// </summary>
        public bool UseReminder { get; set; }

        /// <summary>
        /// The number of minutes a reminder should be shown
        /// </summary>
        public int ReminderMinutesBefore { get; set; }
    }
}