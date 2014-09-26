using System;

namespace Seemplest.Core.DataAccess.Mapping
{
    /// <summary>
    /// This markup interface defines the behavior of an object that converts a source data field to
    /// a data record property.
    /// </summary>
    /// <remarks>This interface must support converting the null value.</remarks>
    public interface IDataConverter
    {
        /// <summary>
        /// Gets the source type.
        /// </summary>
        /// <returns></returns>
        Type GetDataType();

        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <returns></returns>
        Type GetClrType();
    }
}