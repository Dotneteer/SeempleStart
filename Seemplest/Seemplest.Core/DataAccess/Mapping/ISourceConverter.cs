namespace Seemplest.Core.DataAccess.Mapping
{
    /// <summary>
    /// This interface defines the behavior of an object that converts a source data field to
    /// a data record property type.
    /// </summary>
    public interface ISourceConverter: IDataConverter
    {
        /// <summary>
        /// Converts the specified source object to the destination type.
        /// </summary>
        /// <param name="dataType">Source value</param>
        /// <returns>
        /// The target value
        /// </returns>
        object ConvertFromDataType(object dataType);
    }
}