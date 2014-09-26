namespace Seemplest.Core.Common
{
    /// <summary>
    /// This interface defines the behavior of an object that is able to map names 
    /// (suc as event log, file, etc.) to related names.
    /// </summary>
    public interface INameMapper
    {
        /// <summary>
        /// Maps the specified name to another name.
        /// </summary>
        /// <param name="name">Source name</param>
        /// <returns>Mapped name</returns>
        string Map(string name);
    }
}