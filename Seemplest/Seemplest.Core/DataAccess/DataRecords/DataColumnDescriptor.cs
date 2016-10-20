﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Seemplest.Core.DataAccess.Mapping;

namespace Seemplest.Core.DataAccess.DataRecords
{
    /// <summary>
    /// This class describes a column of a data table
    /// </summary>
    public sealed class DataColumnDescriptor
    {
        /// <summary>
        /// Property information about the data column
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the name of underlying data column
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Gets the ClrType of the property representing the data column
        /// </summary>
        public Type ClrType { get; private set; }

        /// <summary>
        /// Gets the primary key oreder of the data column 
        /// (null, if not included in the primary key).
        /// </summary>
        public int? PkOrder { get; private set; }

        /// <summary>
        /// Gets the flag indicating if this column is automatically generated.
        /// </summary>
        public bool IsAutoGenerated { get; private set; }

        /// <summary>
        /// Gets the flag indicating if this column is calculated.
        /// </summary>
        public bool IsCalculated { get; private set; }

        /// <summary>
        /// Gets the flag indicating if this column is a version column.
        /// </summary>
        public bool IsVersionColumn { get; private set; }

        /// <summary>
        /// Gets the maximum length of the column.
        /// </summary>
        public int? MaxLength { get; private set; }

        /// <summary>
        /// Gets the flag indicating if the column is a blob;
        /// </summary>
        public bool IsBlob { get; private set; }

        /// <summary>
        /// Gets the list of converters that can convert different sources to this 
        /// data column's property type.
        /// </summary>
        public IReadOnlyList<ISourceConverter> SourceConverters { get; private set; }

        /// <summary>
        /// Gets the converter that converts this data record column to the expected data type
        /// to different target types.
        /// </summary>
        public ITargetConverter TargetConverter { get; private set; }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="propertyInfo">Property information</param>
        /// <param name="name">Property name</param>
        /// <param name="columnName">Column name</param>
        /// <param name="clrType">Underlying CLR type</param>
        /// <param name="pkOrder">Order in the primary key</param>
        /// <param name="isAutoGenerated">Is automatically generated?</param>
        /// <param name="isCalculated">Is Calculated?</param>
        /// <param name="isVersionColumn">Is a version column?</param>
        /// <param name="maxLength">Maximum column length (optional)</param>
        /// <param name="isBlob">Is a blob column?</param>
        /// <param name="sourceConverters">Optional list of source converters</param>
        /// <param name="targetConverter">Optional target converter</param>
        public DataColumnDescriptor(PropertyInfo propertyInfo, string name, string columnName, Type clrType, 
            int? pkOrder, bool isAutoGenerated, bool isCalculated, bool isVersionColumn, 
            int? maxLength, bool isBlob,
            IList<ISourceConverter> sourceConverters = null,
            ITargetConverter targetConverter = null)
        {
            PropertyInfo = propertyInfo;
            Name = name;
            ColumnName = columnName;
            ClrType = clrType;
            PkOrder = pkOrder;
            IsAutoGenerated = isAutoGenerated;
            IsCalculated = isCalculated;
            IsVersionColumn = isVersionColumn;
            MaxLength = maxLength;
            IsBlob = isBlob;
            if (sourceConverters != null)
            {
                SourceConverters = new ReadOnlyCollection<ISourceConverter>(sourceConverters);
            }
            TargetConverter = targetConverter;
        }
    }
}