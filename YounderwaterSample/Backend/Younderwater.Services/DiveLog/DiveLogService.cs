using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;
using SeemplestBlocks.Core.ServiceInfrastructure;
using Younderwater.Dto.DiveLog;
using Younderwater.Services.DiveLog.DataAccess;
using Younderwater.Services.DiveLog.Exceptions;

namespace Younderwater.Services.DiveLog
{
    /// <summary>
    /// This class implements operations for managing the dive log.
    /// </summary>
    public class DiveLogService : ServiceObjectWithUserBase, IDiveLogService
    {
        /// <summary>
        /// Gets all dives of the current user
        /// </summary>
        /// <returns>All dives of the user</returns>
        public async Task<List<DiveLogEntryDto>> GetAllDivesOfUserAsync()
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IDiveLogDataAccessOperations>())
            {
                return (await ctx.GetAllDiveLogForUserIdAsync(ServicedUserId))
                    .Select(MapToDiveLogEntryDto)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a specific dive record
        /// </summary>
        /// <param name="diveId">Dive log entry ID</param>
        /// <returns>Dive log entry, if exists; otherwise, null</returns>
        public async Task<DiveLogEntryDto> GetDiveByIdAsync(int diveId)
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IDiveLogDataAccessOperations>())
            {
                var dive = await ctx.GetDiveLogByIdAsync(diveId);
                if (dive == null)
                {
                    throw new DiveNotFoundException(diveId);
                }
                return MapToDiveLogEntryDto(dive);
            }
        }

        /// <summary>
        /// Registers a new dive log entry
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        public async Task<int> RegisterDiveLogEntryAsync(DiveLogEntryDto dive)
        {
            ValidateDiveLogArgument(dive);
            Verify.Require(dive.Id == 0, "dive.Id");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateContext<IDiveLogDataAccessOperations>())
            {
                var diveRecord = new DiveLogRecord
                {
                    UserId = ServicedUserId,
                    Date = dive.Date,
                    DiveSite = dive.DiveSite,
                    Location = dive.Location,
                    MaxDepth = dive.MaxDepth,
                    BottomTime = dive.BottomTime,
                    Comment = dive.Comment
                };
                await ctx.InsertDiveLogEntryAsync(diveRecord);
                return diveRecord.Id;
            }
        }

        /// <summary>
        /// Modifies a dive log entry
        /// </summary>
        /// <param name="dive">The dive log entry to save</param>
        public async Task ModifyDiveLogEntryAsync(DiveLogEntryDto dive)
        {
            ValidateDiveLogArgument(dive);
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateContext<IDiveLogDataAccessOperations>())
            {
                var diveRecord = await ctx.GetDiveLogByIdAsync(dive.Id);
                if (diveRecord == null)
                {
                    throw new DiveNotFoundException(dive.Id);
                }
                
                if (diveRecord.UserId != ServicedUserId)
                {
                    throw new NoPermissionToDiveLogException(diveRecord.Id, ServicedUserId, diveRecord.UserId);
                }
                
                diveRecord.Date = dive.Date;
                diveRecord.DiveSite = dive.DiveSite;
                diveRecord.Location = dive.Location;
                diveRecord.MaxDepth = dive.MaxDepth;
                diveRecord.BottomTime = dive.BottomTime;
                diveRecord.Comment = dive.Comment;
                await ctx.UpdateDiveLogEntryAsync(diveRecord);
            }
        }

        /// <summary>
        /// Removes the specified dive log entry
        /// </summary>
        /// <param name="diveId">Dive log entry ID</param>
        public async Task RemoveDiveLogEntryAsync(int diveId)
        {
            using (var ctx = DataAccessFactory.CreateContext<IDiveLogDataAccessOperations>())
            {
                var diveRecord = await ctx.GetDiveLogByIdAsync(diveId);
                if (diveRecord == null)
                {
                    throw new DiveNotFoundException(diveId);
                }

                if (diveRecord.UserId != ServicedUserId)
                {
                    throw new NoPermissionToDiveLogException(diveRecord.Id, ServicedUserId, diveRecord.UserId);
                }

                await ctx.DeleteDiveLogEntryAsync(diveId);
            }
        }

        /// <summary>
        /// Checks whether arguments are valid
        /// </summary>
        private void ValidateDiveLogArgument(DiveLogEntryDto dive)
        {
            Verify.NotNull(dive, "dive");
            Verify.RaiseWhenFailed();

            Verify.NotNullOrEmpty(dive.DiveSite, "dive.DiveSite");
            Verify.NotNullOrEmpty(dive.Location, "dive.Location");
            Verify.IsInRange(dive.MaxDepth, 0, 300, "dive.MaxDepth");
            Verify.IsInRange(dive.BottomTime, 0, 480, "dive.BottomTime");
        }

        /// <summary>
        /// Maps a DiveLogRecord instance to a DiveLogEntryDto instance
        /// </summary>
        private static DiveLogEntryDto MapToDiveLogEntryDto(DiveLogRecord r)
        {
            return new DiveLogEntryDto
            {
                Id = r.Id,
                Date = r.Date,
                DiveSite = r.DiveSite,
                Location = r.Location,
                MaxDepth = r.MaxDepth,
                BottomTime = r.BottomTime,
                Comment = r.Comment
            };
        }
    }
}