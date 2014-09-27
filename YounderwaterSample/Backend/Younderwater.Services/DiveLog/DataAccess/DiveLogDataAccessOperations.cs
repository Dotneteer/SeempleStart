using System.Collections.Generic;
using System.Threading.Tasks;
using SeemplestBlocks.Core.ServiceInfrastructure;

namespace Younderwater.Services.DiveLog.DataAccess
{
    /// <summary>
    /// This class implements dive log entry data operations
    /// </summary>
    public class DiveLogDataAccessOperations : CoreSqlDataAccessOperationBase, IDiveLogDataAccessOperations
    {
        /// <summary>
        /// Initializes this object with the specified connection information
        /// </summary>
        /// <param name="connectionOrName">Connection information</param>
        public DiveLogDataAccessOperations(string connectionOrName)
            : base(connectionOrName)
        {
        }

        /// <summary>
        /// Gets all dives of the specified user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>All dives of the user</returns>
        public async Task<List<DiveLogRecord>> GetAllDiveLogForUserIdAsync(string userId)
        {
            return await OperationAsync(ctx => ctx.FetchAsync<DiveLogRecord>(
                "where [UserId]=@0", userId));
        }

        /// <summary>
        /// Gets a specific dive record
        /// </summary>
        /// <param name="id">Dive log entry ID</param>
        /// <returns>Dive log entry, if exists; otherwise, null</returns>
        public async Task<DiveLogRecord> GetDiveLogByIdAsync(int id)
        {
            return await OperationAsync(ctx => ctx.FirstOrDefaultAsync<DiveLogRecord>(
                "where [Id]=@0", id));
        }

        /// <summary>
        /// Inserts a new dive log entry into the database
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        public async Task InsertDiveLogEntryAsync(DiveLogRecord dive)
        {
            await OperationAsync(ctx => ctx.InsertAsync(dive));
        }

        /// <summary>
        /// Updates a dive log entry in the database
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        public async Task UpdateDiveLogEntryAsync(DiveLogRecord dive)
        {
            await OperationAsync(ctx => ctx.UpdateAsync(dive));
        }

        /// <summary>
        /// Removes the specified dive log entry from the database
        /// </summary>
        /// <param name="diveId">Dive log entry ID</param>
        public async Task DeleteDiveLogEntryAsync(int diveId)
        {
            await OperationAsync(ctx => ctx.DeleteByIdAsync<DiveLogRecord>(diveId));
        }
    }
}