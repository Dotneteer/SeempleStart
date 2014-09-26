using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Seemplest.Core.Exceptions;

namespace Seemplest.Core.DataAccess.DataRecords
{
    /// <summary>
    /// This generic abstract class represents a data record within the database.
    /// </summary>
    public abstract class DataRecord<TRecord> : IDataRecord, ICloneableRecord<TRecord> 
        where TRecord: DataRecord<TRecord>, new()
    {
        // --- Cache for the clone methods
        // ReSharper disable StaticFieldInGenericType
        private static readonly Dictionary<Type, Delegate> s_CloneMethods = 
            new Dictionary<Type, Delegate>();
        // ReSharper restore StaticFieldInGenericType

        private readonly HashSet<string> _modifiedColumns = new HashSet<string>();
        private bool _isLoaded;
        private bool _isImmutable;

        /// <summary>
        /// Gets the type of the record
        /// </summary>
        /// <returns>Type of the record</returns>
        public Type GetRecordType()
        {
            return typeof (TRecord);
        }

        /// <summary>
        /// Sets the content of this data record as immutable.
        /// </summary>
        void IDataRecord.SetImmutable()
        {
            _isImmutable = true;
        }

        /// <summary>
        /// Signs that this record has been loaded from the database.
        /// </summary>
        void IDataRecord.SignLoaded()
        {
            _isLoaded = true;
        }

        /// <summary>
        /// Indicates if the specified property has been modified or not.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <returns>True, if the property has been modified; otherwise, false</returns>
        bool IDataRecord.IsModified(string name)
        {
            return _modifiedColumns.Contains(name);
        }

        /// <summary>
        /// Gets the list of modified columns
        /// </summary>
        /// <returns>List of modified columns</returns>
        public List<string> GetModifiedColumns()
        {
            return _modifiedColumns.ToList();
        }

        /// <summary>
        /// Clones this data record
        /// </summary>
        /// <returns>Clone of the data record</returns>
        public IDataRecord Clone()
        {
            return ((ICloneableRecord<TRecord>)this).Clone();
        }

        /// <summary>
        /// Checks and administers column modification.
        /// </summary>
        /// <typeparam name="T">Column type</typeparam>
        /// <param name="value">New column value</param>
        /// <param name="columnName">Modified column's name</param>
        /// <returns></returns>
        protected T Modify<T>(T value, string columnName)
        {
            if (_isLoaded)
            {
                if (_isImmutable)
                    throw new ImmutableRecordChangedException(columnName);
                _modifiedColumns.Add(columnName);
            }
            return value;
        }

        /// <summary>
        /// Clones the data record into a new instance
        /// </summary>
        /// <returns>The clone of the data record</returns>
        TRecord ICloneableRecord<TRecord>.Clone()
        {
            // --- Obtain the clone method
            var typeKey = typeof (TRecord);
            Delegate cloneDelegate;
            if (!s_CloneMethods.TryGetValue(typeKey, out cloneDelegate))
            {
                cloneDelegate = CreateCloneDelegate();
                s_CloneMethods[typeKey] = cloneDelegate;
            }
            var cloneFunc = (Func<TRecord, TRecord>) cloneDelegate;
            return cloneFunc((TRecord)this);
        }

        /// <summary>
        /// Creates a delegate to clone the specified 
        /// </summary>
        /// <returns>Function to clone this data record</returns>
        private static Func<TRecord, TRecord> CreateCloneDelegate()
        {
            // --- Create a dynamic method
            var cloneMethod = new DynamicMethod("CloneFactory" + s_CloneMethods.Count,
                typeof(TRecord),
                new[] { typeof(TRecord) },
                true);
            var il = cloneMethod.GetILGenerator();
            var metadata = RecordMetadataManager.GetMetadata<TRecord>();
            var cloneObject = il.DeclareLocal(typeof(TRecord));
            
            // --- Create an empty clone of TRecord and store it
            il.Emit(OpCodes.Newobj, typeof(TRecord).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null));
            il.Emit(OpCodes.Stloc, cloneObject);

            // --- Copy each property value
            foreach (var dataColumn in metadata.DataColumns)
            {
                il.Emit(OpCodes.Ldloc, cloneObject);    // cloneObject
                il.Emit(OpCodes.Ldarg_0);               // cloneObject, origObject
                il.Emit(OpCodes.Call, dataColumn.PropertyInfo.GetGetMethod());  // cloneObject, origObject.dataColumn.Value
                il.Emit(OpCodes.Call, dataColumn.PropertyInfo.GetSetMethod());
            }

            il.Emit(OpCodes.Ldloc, cloneObject);
            il.Emit(OpCodes.Ret);

            // --- Create the delegate
            var cloneDelegate = cloneMethod.CreateDelegate(Expression.GetFuncType(typeof(TRecord), typeof(TRecord)));
            return (Func<TRecord, TRecord>)cloneDelegate;
        }

        /// <summary>
        /// Merges the changed properties from another object
        /// </summary>
        /// <param name="other">Other data record object</param>
        public void MergeChangesFrom(TRecord other)
        {
            // --- Let's assume this record has already been loaded
            _isLoaded = true;
            var metadata = RecordMetadataManager.GetMetadata<TRecord>();
            foreach (var dataColumn in metadata.DataColumns)
            {
                var propInfo = typeof (TRecord).GetProperty(dataColumn.Name);
                var oldVal = propInfo.GetValue(this);
                var newVal = propInfo.GetValue(other);
                if (oldVal == null && newVal != null || oldVal != null && !oldVal.Equals(newVal))
                {
                    propInfo.SetValue(this, newVal);
                }
            }
        }
    }
}