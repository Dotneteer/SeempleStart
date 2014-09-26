namespace Seemplest.Core.DataAccess
{
    /// <summary>
    /// This enum signs if the Execute family of methods can be used or not
    /// </summary>
    public enum SqlDirectExecuteMode
    {
        /// <summary>Do not allow Execute methods</summary>
        Disable = 0,

        /// <summary>Allow using Execute methods</summary>
        Enable = 1
    }
}