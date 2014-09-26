namespace Seemplest.Core.DataAccess.Mapping
{
    /// <summary>
    /// This interface defines the behavior of an object that converts a source data record
    /// property type to a data field type.
    /// </summary>
    public interface ITargetConverter : IDataConverter
    {
        /// <summary>
        /// Converts the specified CLR type to the expected data type.
        /// </summary>
        /// <param name="clrObject">CLR object</param>
        /// <returns>Converted data object</returns>
        object ConvertToDataType(object clrObject);
    }
}