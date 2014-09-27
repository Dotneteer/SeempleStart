using System;

namespace Younderwater.Dto.DiveLog
{
    /// <summary>
    /// This class respresents a dive log entry
    /// </summary>
    public class DiveLogEntryDto
    {
        /// <summary>
        /// Entry ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dive date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Diving site
        /// </summary>
        public string DiveSite { get; set; }

        /// <summary>
        /// Location of dive site
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Maximum depth of the dive
        /// </summary>
        public decimal MaxDepth { get; set; }

        /// <summary>
        /// Dive time in minutes
        /// </summary>
        public int BottomTime { get; set; }

        /// <summary>
        /// Dive comments
        /// </summary>
        public string Comment { get; set; }
    }
}