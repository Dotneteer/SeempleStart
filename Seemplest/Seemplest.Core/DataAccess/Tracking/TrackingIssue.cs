using Seemplest.Core.DataAccess.DataRecords;

namespace Seemplest.Core.DataAccess.Tracking
{
    /// <summary>
    /// This class represents and issue with a tracked record
    /// </summary>
    public class TrackingIssue
    {
        /// <summary>
        /// Gets the copy of the record with tracking issue
        /// </summary>
        public readonly IDataRecord RecordInstance;

        /// <summary>
        /// Gets the description of the issue
        /// </summary>
        public readonly string Description;

        public TrackingIssue(IDataRecord recordInstance, string description)
        {
            RecordInstance = recordInstance;
            Description = description;
        }
    }
}