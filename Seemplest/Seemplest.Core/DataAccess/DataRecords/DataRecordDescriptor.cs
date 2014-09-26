using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Seemplest.Core.DataAccess.DataRecords
{
    /// <summary>
    /// This class describes the metadata of a <see cref="DataRecord{TRecord}"/>
    /// </summary>
    public sealed class DataRecordDescriptor
    {
        private readonly DataColumnsKeyedCollection _dataColumns = new DataColumnsKeyedCollection();

        /// <summary>
        /// Gets the flag indicating if this instance describes a simple POCO
        /// </summary>
        public bool IsSimplePoco { get; private set; }

        /// <summary>
        /// Gets the flag indicating that this record is immutable.
        /// </summary>
        public bool IsImmutable { get; private set; }

        /// <summary>
        /// Get the type of the data record
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the schema name of the underlying datatable
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the name of the underlying data table
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the collection of primary key columns
        /// </summary>
        /// <remarks>Cache the value of this property when using it!</remarks>
        public IReadOnlyList<DataColumnDescriptor> PrimaryKeyColumns
        {
            get
            {
                // ReSharper disable PossibleInvalidOperationException
                return new ReadOnlyCollection<DataColumnDescriptor>(
                    _dataColumns.Where(c => c.PkOrder.HasValue)
                        .OrderBy(c => c.PkOrder.Value).ToList());
                // ReSharper restore PossibleInvalidOperationException
            }
        }

        /// <summary>
        /// Gets the collection of data columns
        /// </summary>
        /// <remarks>Cache the value of this property when using it!</remarks>
        public IReadOnlyList<DataColumnDescriptor> DataColumns
        {
            get
            {
                return new ReadOnlyCollection<DataColumnDescriptor>(_dataColumns);
            }
        }

        /// <summary>
        /// Creates a new instance of this class with the specified attributes.
        /// </summary>
        /// <param name="isSimplePoco">Indicates that the metadata describes a simple POCO</param>
        /// <param name="isImmutable">Indicates that this object is immutable</param>
        /// <param name="type">DataRecord type</param>
        /// <param name="schemaName">Name of the underlying data table schema</param>
        /// <param name="tableName">Name of the underlying data table</param>
        /// <param name="columns">Columns of the table</param>
        public DataRecordDescriptor(bool isSimplePoco, bool isImmutable, Type type, string schemaName, string tableName, 
            IEnumerable<DataColumnDescriptor> columns)
        {
            IsSimplePoco = isSimplePoco;
            IsImmutable = isImmutable;
            Type = type;
            SchemaName = schemaName;
            TableName = tableName;
            foreach (var column in columns) _dataColumns.Add(column);
        }

        /// <summary>
        /// Checks if there is a data column with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsColumn(string name)
        {
            return _dataColumns.Contains(name);
        }

        /// <summary>
        /// Gets the data column with the specified index.
        /// </summary>
        /// <param name="index">Data column index</param>
        /// <returns>Data column descriptor</returns>
        public DataColumnDescriptor this[int index]
        {
            get { return _dataColumns[index]; }
        }

        /// <summary>
        /// Gets the data column with the specified name.
        /// </summary>
        /// <param name="name">Data column name</param>
        /// <returns>Data column descriptor</returns>
        public DataColumnDescriptor this[string name]
        {
            get { return _dataColumns.Contains(name) ? _dataColumns[name] : null; }
        }

        /// <summary>
        /// A collection for data column to retrieve them by index and by column name.
        /// </summary>
        private class DataColumnsKeyedCollection : KeyedCollection<string, DataColumnDescriptor>
        {
            /// <summary>
            /// Creates a new instance of this class.
            /// </summary>
            public DataColumnsKeyedCollection() : base(StringComparer.InvariantCultureIgnoreCase, 10)
            {
            }

            /// <summary>
            /// When implemented in a derived class, extracts the key from the specified element.
            /// </summary>
            /// <returns>
            /// The key for the specified element.
            /// </returns>
            /// <param name="item">The element from which to extract the key.</param>
            protected override string GetKeyForItem(DataColumnDescriptor item)
            {
                return item.ColumnName;
            }
        }
    }
}