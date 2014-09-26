using System.Data;

namespace Seemplest.Core.DataAccess
{
    /// <summary>
    /// This enum defines how version handling should be used in conjunction with
    /// the Update family of methods.
    /// </summary>
    public enum SqlRowVersionHandling
    {
        /// <summary>Versions should not be used with Update methods</summary>
        DoNotUseVersions,

        /// <summary>Use rowversion columns, but ignore concurrency issues</summary>
        IgnoreConcurrencyIssues,

        /// <summary>
        /// Use rowversion columns, and raise a <see cref="DBConcurrencyException"/> 
        /// in case of concurrency issues
        /// </summary>
        RaiseException
    }
}