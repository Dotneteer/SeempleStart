using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Seemplest.Core.DataAccess.Tracking
{
    /// <summary>
    /// This class represents the changes related to a SQL database table
    /// </summary>
    public class SqlTableChangeSet: ReadOnlyDictionary<PrimaryKeyValue, SqlRecordChangeSet>
    {
        /// <summary>
        /// Gets the dictionary behind this change set -- for internal use
        /// </summary>
        public readonly IDictionary<PrimaryKeyValue, SqlRecordChangeSet> ChangeSet;

        /// <summary>
        /// Initializes an instance using the specified dictionary
        /// </summary>
        public SqlTableChangeSet() : base(new Dictionary<PrimaryKeyValue, SqlRecordChangeSet>())
        {
            ChangeSet = Dictionary;
        }
    }
}