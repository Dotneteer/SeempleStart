using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Seemplest.Core.DataAccess.Tracking
{
    /// <summary>
    /// This class represents the change set during the lifecycle of a SqlDatabase object.
    /// </summary>
    public class SqlDatabaseChangeSet: ReadOnlyDictionary<string, SqlTableChangeSet>
    {
        /// <summary>
        /// Gets the dictionary behind this change set -- for internal use
        /// </summary>
        internal readonly IDictionary<string, SqlTableChangeSet> ChangeSet;

        /// <summary>
        /// Initializes an instance using the specified dictionary
        /// </summary>
        public SqlDatabaseChangeSet() : base(new Dictionary<string, SqlTableChangeSet>())
        {
            ChangeSet = Dictionary;
        }

        /// <summary>
        /// Eliminates all tables that are non-changed
        /// </summary>
        internal void EliminateUnchangedTables()
        {
            var anyChange = false;
            var newDictionary = new Dictionary<string, SqlTableChangeSet>();
            foreach (var table in Keys)
            {
                var tableChange = this[table];
                if (tableChange.ChangeSet.Count > 0)
                {
                    newDictionary.Add(table, tableChange);
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