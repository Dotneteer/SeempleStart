namespace Seemplest.Core.DataAccess.DataRecords
{
    /// <summary>
    /// This interface defines the behaviour of a cloneable data record
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    public interface ICloneableRecord<out TRecord>
    {
        /// <summary>
        /// Clones the data record into a new instance
        /// </summary>
        /// <returns>The clone of the data record</returns>
        TRecord Clone();
    }
}