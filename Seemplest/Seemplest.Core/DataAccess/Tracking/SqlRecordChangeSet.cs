using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Seemplest.Core.DataAccess.Tracking
{
    /// <summary>
    /// Represents the changes of a record
    /// </summary>
    public class SqlRecordChangeSet: ReadOnlyDictionary<string, SqlFieldChange>
    {
        private readonly IReadOnlyList<TrackingIssue> _issueList; 

        /// <summary>
        /// Gets the issue list behind this instance
        /// </summary>
        public readonly List<TrackingIssue> InternalIssueList;

        /// <summary>
        /// Gets the dictionary behind this change set -- for internal use
        /// </summary>
        internal IDictionary<string, SqlFieldChange> ChangeSet;

        /// <summary>
        /// Gets the list of issues related to the tracking of the record instance
        /// </summary>
        public IReadOnlyList<TrackingIssue> IssueList
        {
            get { return _issueList; }
        }

        /// <summary>
        /// Gets the state of this record in the current context
        /// </summary>
        public ChangedRecordState State { get; internal set; }

        /// <summary>
        /// Gets the timestamp of the change
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Initializes an instance using the specified dictionary
        /// </summary>
        public SqlRecordChangeSet() : base (new Dictionary<string, SqlFieldChange>())
        {
            ChangeSet = Dictionary;
            InternalIssueList = new List<TrackingIssue>();
            _issueList = new ReadOnlyCollection<TrackingIssue>(InternalIssueList);
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Eliminates all fields that are non-changed
        /// </summary>
        internal void EliminateNonChangedFields()
        {
            var anyChange = false;
            var newDictionary = new Dictionary<string, SqlFieldChange>();
            foreach (var dataField in Keys)
            {
                var fieldChange = this[dataField];
                if (fieldChange.PreviousValue == null && fieldChange.NewValue != null ||
                    fieldChange.PreviousValue != null && fieldChange.NewValue == null ||
                    fieldChange.PreviousValue != null && fieldChange.NewValue != null &&
                    !fieldChange.PreviousValue.Equals(fieldChange.NewValue))
                {
                    newDictionary.Add(dataField, fieldChange);
                    continue;
                }
                anyChange = true;
            }
            if (!anyChange) return;
            Dictionary.Clear();
            foreach (var item in newDictionary) Dictionary.Add(item);
        }
    }
}