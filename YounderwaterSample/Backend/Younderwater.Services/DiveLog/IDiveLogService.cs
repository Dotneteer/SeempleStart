using System.Collections.Generic;
using System.Threading.Tasks;
using Seemplest.Core.ServiceObjects;
using Younderwater.Dto.DiveLog;

namespace Younderwater.Services.DiveLog
{
    /// <summary>
    /// This interface defines operations for managing the dive log.
    /// </summary>
    public interface IDiveLogService: IServiceObject
    {
        /// <summary>
        /// Gets all dives of the current user
        /// </summary>
        /// <returns>All dives of the user</returns>
        Task<List<DiveLogEntryDto>> GetAllDivesOfUserAsync();

        /// <summary>
        /// Gets a specific dive record
        /// </summary>
        /// <param name="diveId">Dive log entry ID</param>
        /// <returns>Dive log entry, if exists; otherwise, null</returns>
        Task<DiveLogEntryDto> GetDiveByIdAsync(int diveId);

        /// <summary>
        /// Registers a new dive log entry
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        Task<int> RegisterDiveLogEntryAsync(DiveLogEntryDto dive);

        /// <summary>
        /// Modifies a dive log entry
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        Task ModifyDiveLogEntryAsync(DiveLogEntryDto dive);

        /// <summary>
        /// Removes the specified dive log entry
        /// </summary>
        /// <param name="diveId">Dive log entry ID</param>
        Task RemoveDiveLogEntryAsync(int diveId);
    }
}