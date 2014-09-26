using System;
using Seemplest.Core.DataAccess.Tracking;

namespace Seemplest.Core.DataAccess
{
    /// <summary>
    /// This class represents the event arguments related to tracking events
    /// </summary>
    public class TrackingInfoEventArgs: EventArgs
    {
        /// <summary>
        /// Gets the change set belonging to this event
        /// </summary>
        public SqlDatabaseChangeSet ChangeSet { get; private set; }

        /// <summary>
        /// Sets the exception instance if tracking is to be aborted.
        /// </summary>
        public Exception TrackingException { get; set; }

        /// <summary>
        /// Initializes the event args with the specified change set
        /// </summary>
        /// <param name="changeSet"></param>
        public TrackingInfoEventArgs(SqlDatabaseChangeSet changeSet)
        {
            ChangeSet = changeSet;
        }
    }
}