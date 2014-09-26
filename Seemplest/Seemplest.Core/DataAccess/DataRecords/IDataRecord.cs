using System;
using System.Collections.Generic;

namespace Seemplest.Core.DataAccess.DataRecords
{
    /// <summary>
    /// This interface describes a data record
    /// </summary>
    public interface IDataRecord
    {
        /// <summary>
        /// Gets the type of the record
        /// </summary>
        /// <returns>Type of the record</returns>
        Type GetRecordType();

        /// <summary>
        /// Sets the content of this data record as immutable.
        /// </summary>
        void SetImmutable();

        /// <summary>
        /// Signs that this record has been loaded from the database.
        /// </summary>
        void SignLoaded();

        /// <summary>
        /// Indicates if the specified property has been modified or not.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <returns>True, if the property has been modified; otherwise, false</returns>
        bool IsModified(string name);

        /// <summary>
        /// Gets the list of modified columns
        /// </summary>
        /// <returns>List of modified columns</returns>
        List<string> GetModifiedColumns();

        /// <summary>
        /// Clones this data record
        /// </summary>
        /// <returns>Clone of the data record</returns>
        IDataRecord Clone();
    }
}