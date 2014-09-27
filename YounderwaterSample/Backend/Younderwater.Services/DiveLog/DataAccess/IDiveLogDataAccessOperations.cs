using System.Collections.Generic;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;

namespace Younderwater.Services.DiveLog.DataAccess
{
    /// <summary>
    /// This interface defines dive log entry data operations
    /// </summary>
    public interface IDiveLogDataAccessOperations: IDataAccessOperation
    {
        /// <summary>
        /// Gets all dives of the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>All dives of the user</returns>
        Task<List<DiveLogRecord>> GetAllDiveLogForUserIdAsync(string userId);

        /// <summary>
        /// Gets a specific dive record
        /// </summary>
        /// <param name="id">Dive log entry ID</param>
        /// <returns>Dive log entry, if exists; otherwise, null</returns>
        Task<DiveLogRecord> GetDiveLogByIdAsync(int id);

        /// <summary>
        /// Inserts a new dive log entry into the database
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        Task InsertDiveLogEntryAsync(DiveLogRecord dive);

        /// <summary>
        /// Updates a dive log entry in the database
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        Task UpdateDiveLogEntryAsync(DiveLogRecord dive);

        /// <summary>
        /// Removes the specified dive log entry from the database
        /// </summary>
        /// <param name="diveId">Dive log entry ID</param>
        Task DeleteDiveLogEntryAsync(int diveId);
    }
}