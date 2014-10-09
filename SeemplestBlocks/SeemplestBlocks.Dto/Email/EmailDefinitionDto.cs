using System;
using System.Collections.Generic;

namespace SeemplestBlocks.Dto.Email
{
    /// <summary>
    /// Egy elküldendő email definícióját írja le
    /// </summary>
    public class EmailDefinitionDto
    {
        /// <summary>
        /// A küldő email címe
        /// </summary>
        public string FromAddr { get; set; }

        /// <summary>
        /// A küldő neve
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// A címzett email címe
        /// </summary>
        public string ToAddr { get; set; }

        /// <summary>
        /// A levélsablon típusa
        /// </summary>
        public string TemplateType { get; set; }

        /// <summary>
        /// A levél paraméterei
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// A kiküldés legkorábbi időpontja
        /// </summary>
        public DateTimeOffset? SendAfterUtc { get; set; }

        /// <summary>
        /// Az opcionális találkozó leírása
        /// </summary>
        public AppointmentDefinitionDto Appointment { get; set; }
    }
}