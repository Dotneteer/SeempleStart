using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Seemplest.Core.Common;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.Mapping;
using Seemplest.Core.Exceptions;

namespace Seemplest.Core.DataAccess.DataRecords
{
    /// <summary>
    /// This class is responsible for managing <see cref="DataRecord{TRecord}"/> related metadata
    /// </summary>
    public static class RecordMetadataManager
    {
        private static readonly ConcurrentDictionary<Type, DataRecordDescriptor> s_DataRecords =
            new ConcurrentDictionary<Type, DataRecordDescriptor>();

        /// <summary>
        /// Clears the cache of data records.
        /// </summary>
        public static void Reset()
        {
            s_DataRecords.Clear();
        }

        /// <summary>
        /// Gets the metadata for the specified record type
        /// </summary>
        /// <param name="recordType">Type to obtain metadata for</param>
        /// <returns>Record metadata</returns>
        public static DataRecordDescriptor GetMetadata(Type recordType)
        {
            if (recordType == null) throw new ArgumentNullException("recordType");

            // --- Check the cache
            DataRecordDescriptor metadata;
            if (s_DataRecords.TryGetValue(recordType, out metadata)) return metadata;

            // --- Obtain the metadata from reflection
            metadata = ScanMetadata(recordType);
            s_DataRecords[recordType] = metadata;
            return metadata;
        }

        /// <summary>
        /// Gets the metadata for the specified record type
        /// </summary>
        /// <typeparam name="T">Type to obtain metadata for</typeparam>
        /// <returns>Record metadata</returns>
        public static DataRecordDescriptor GetMetadata<T>()
        {
            return GetMetadata(typeof (T));
        }

        /// <summary>
        /// Scans the metadata of the specified type.
        /// </summary>
        /// <param name="recordType">Type to scan for metadata</param>
        /// <returns>Metadata of the input type</returns>
        private static DataRecordDescriptor ScanMetadata(Type recordType)
        {
            return typeof (IDataRecord).IsAssignableFrom(recordType) 
                ? ScanDataRecord(recordType) 
                : ScanPoco(recordType);
        }

        /// <summary>
        /// Scans the metadata of a traditional POCO object
        /// </summary>
        /// <param name="recordType">Type of POCO</param>
        /// <returns>POCO metadata information</returns>
        private static DataRecordDescriptor ScanPoco(Type recordType)
        {
            var tableAttr = recordType.GetCustomAttribute<TableNameAttribute>();
            var schemaAttr = recordType.GetCustomAttribute<SchemaNameAttribute>();
            var columns = new List<DataColumnDescriptor>();

            // --- Go through all public instance read-write properties of the POCO
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var property in recordType
            // ReSharper restore LoopCanBeConvertedToQuery
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetAccessors().Length == 2))
            {
                // --- Still check the ColumnName attribute
                var attrSet = new AttributeSet(property);
                var colAttr = attrSet.Optional<ColumnNameAttribute>();
                var sourceConverters = GetSourceConverters(property, attrSet);
                columns.Add(new DataColumnDescriptor(
                    property,
                    property.Name,
                    colAttr == null ? property.Name : colAttr.Value,
                    property.PropertyType,
                    null, false, false,false,
                    sourceConverters));
            }

            // --- Retrieve the metadata object
            return new DataRecordDescriptor(true, false, recordType, 
                schemaAttr == null ? null : schemaAttr.Value, 
                tableAttr == null ? null : tableAttr.Value, 
                columns);
        }

        /// <summary>
        /// Scans the metadata of a <see cref="IDataRecord"/> object
        /// </summary>
        /// <param name="recordType"></param>
        /// <returns>Record metadata</returns>
        private static DataRecordDescriptor ScanDataRecord(Type recordType)
        {
            var columns = new List<DataColumnDescriptor>();

            // --- Obtain record level attributes
            var immutableAttr = recordType.GetCustomAttribute<ImmutableRecordAttribute>();
            var schemaAttr = recordType.GetCustomAttribute<SchemaNameAttribute>();
            var tableAttr = recordType.GetCustomAttribute<TableNameAttribute>();
            if (tableAttr == null)
            {
                throw new MissingTableNameException(recordType);
            }

            // --- Go through all public instance read-write properties of the POCO
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var property in recordType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetAccessors().Length == 2))
            // ReSharper restore LoopCanBeConvertedToQuery
            {
                var attrSet = new AttributeSet(property);
                if (attrSet.Optional<IgnorePropertyAttribute>() != null) continue;

                // --- Check property level attributes
                var colAttr = attrSet.Optional<ColumnNameAttribute>();
                var pkAttr = attrSet.Optional<PrimaryKeyAttribute>();
                var versionAttr = attrSet.Optional<VersionColumnAttribute>();
                var autoGenAttr = attrSet.Optional<AutoGeneratedAttribute>();
                var calcAttr = attrSet.Optional<CalculatedAttribute>();
                var sourceConverters = GetSourceConverters(property, attrSet);
                var targetConverter = GetTargetConverter(attrSet);
                columns.Add(new DataColumnDescriptor(
                    property,
                    property.Name,
                    colAttr == null ? property.Name : colAttr.Value,
                    property.PropertyType,
                    pkAttr == null ? (int?)null : pkAttr.Order,
                    autoGenAttr != null,
                    calcAttr != null,
                    versionAttr != null,
                    sourceConverters,
                    targetConverter));
            }

            // --- Retrieve the metadata object
            return new DataRecordDescriptor(false, immutableAttr != null, recordType,
                schemaAttr == null ? null : schemaAttr.Value, tableAttr.Value, columns);
        }

        /// <summary>
        /// Gets the source converters defined by the specified attribute set.
        /// </summary>
        /// <param name="property">Property information</param>
        /// <param name="attrSet">Attribute set</param>
        /// <returns>List of source converters</returns>
        private static List<ISourceConverter> GetSourceConverters(PropertyInfo property, AttributeSet attrSet)
        {
            var sourceConvertersAttrs = attrSet.All<SourceConverterAttribute>();
            var sourceConverters = new List<ISourceConverter>();
            foreach (var attr in sourceConvertersAttrs)
            {
                if (attr.SourceType == null || attr.ConverterType == null)
                {
                    throw new NullReferenceException(
                        "The Source and ConverterType properties of SourceConverterAttribute cannot be null.");
                }
                if (!typeof (ISourceConverter).IsAssignableFrom(attr.ConverterType))
                    throw new InvalidCastException(
                        string.Format("{0} cannot be used as the value for the ConverterType property " +
                                      "of SourceConverterAttribute, because it cannot be cast to IDataConverter.",
                                      attr.ConverterType));
                var converter = (ISourceConverter)Activator.CreateInstance(attr.ConverterType);
                if (!property.PropertyType.IsAssignableFrom(converter.GetClrType()))
                {
                    throw new InvalidCastException(
                        string.Format("The {0} converter does not support conversion to type {1}.",
                                      attr.ConverterType, property.PropertyType));
                }
                sourceConverters.Add(converter);
            }
            return sourceConverters;
        }

        /// <summary>
        /// Gets the target converter defined by the attribute set
        /// </summary>
        /// <param name="attrSet">Attribute set</param>
        /// <returns>Target converter instance</returns>
        private static ITargetConverter GetTargetConverter(AttributeSet attrSet)
        {
            var attr = attrSet.Optional<TargetConverterAttribute>();
            if (attr == null) return null;

            if (attr.ConverterType == null)
            {
                throw new NullReferenceException(
                    "The ConverterType property of TargetConverterAttribute cannot be null.");
            }
            if (!typeof(ITargetConverter).IsAssignableFrom(attr.ConverterType))
                throw new InvalidCastException(
                    string.Format("{0} cannot be used as the value for the ConverterType property " +
                                  "of TargetConverterAttribute, because it cannot be cast to ITargetConverter.",
                                  attr.ConverterType));
            return (ITargetConverter)Activator.CreateInstance(attr.ConverterType);
        }
    }
}