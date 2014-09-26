using System;

namespace Seemplest.Core.DataAccess.Mapping
{
    /// <summary>
    /// This abstract class is intended to be the base class of all source converters.
    /// </summary>
    /// <typeparam name="TData">Source type</typeparam>
    /// <typeparam name="TClr">Target type</typeparam>
    public abstract class DualConverterBase<TData, TClr> : ISourceConverter, ITargetConverter
    {
        /// <summary>
        /// Converts the specified source object to the destination type.
        /// </summary>
        /// <param name="dataType">Source value</param>
        /// <returns>The target value</returns>
        object ISourceConverter.ConvertFromDataType(object dataType)
        {
            return ConvertFromDataType((TData) dataType);
        }

        /// <summary>
        /// Converts the specified source object to the destination type.
        /// </summary>
        /// <param name="dataType">Source value</param>
        /// <returns>The target value</returns>
        public abstract TClr ConvertFromDataType(TData dataType);

        /// <summary>
        /// Converts the specified CLR type to the expected data type.
        /// </summary>
        /// <param name="clrObject">CLR object</param>
        /// <returns>Converted data object</returns>
        object ITargetConverter.ConvertToDataType(object clrObject)
        {
            return ConvertToDataType((TClr) clrObject);
        }

        /// <summary>
        /// Converts the specified CLR type to the expected data type.
        /// </summary>
        /// <param name="clrObject">CLR object</param>
        /// <returns>Converted data object</returns>
        public abstract TData ConvertToDataType(TClr clrObject);

        /// <summary>
        /// Gets the source type.
        /// </summary>
        /// <returns></returns>
        public Type GetDataType()
        {
            return typeof (TData);
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <returns></returns>
        public Type GetClrType()
        {
            return typeof (TClr);
        }
    }
}