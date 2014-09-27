using System;

namespace Younderwater.Services.DiveLog.Exceptions
{
    /// <summary>
    /// Signs that a user tries to modify an entry without having permission to modify it
    /// </summary>
    public class NoPermissionToDiveLogException : Exception
    {
        public NoPermissionToDiveLogException(int diveId, string userId, string diveUserId) :
            base(string.Format("Dive with ID {0} belongs to user '{1}', but is being modified by user '{2}",
                diveId, diveUserId, userId))
        {
        }
    }
}