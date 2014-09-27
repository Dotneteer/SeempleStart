using System;

namespace Younderwater.Services.DiveLog.Exceptions
{
    /// <summary>
    /// Signs that a dive log entry cannot be found
    /// </summary>
    public class DiveNotFoundException : Exception
    {
        public DiveNotFoundException(int diveId) :
            base(String.Format("The dive log entry with ID {0} cannot be found.", diveId))
        {
        }
    }
}