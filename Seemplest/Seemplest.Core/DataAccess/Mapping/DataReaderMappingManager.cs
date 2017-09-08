using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Seemplest.Core.DataAccess.DataRecords;
using IDataRecord = Seemplest.Core.DataAccess.DataRecords.IDataRecord;

namespace Seemplest.Core.DataAccess.Mapping
{
    /// <summary>
    /// This class is responsible for mapping the content of a data reader to a data record
    /// </summary>
    public static class DataReaderMappingManager
    {
        private static readonly ConcurrentDictionary<Tuple<Type, string>, Delegate> s_Mappers =
            new ConcurrentDictionary<Tuple<Type, string>, Delegate>();
        private static readonly List<Func<object, object>> s_Converters = 
            new List<Func<object, object>>();

        private static readonly EnumMapper s_EnumMapper = new EnumMapper();

        // --- Method information used for dynamic code generation
        static readonly MethodInfo s_FnIsDbNull = 
            typeof(System.Data.IDataRecord).GetMethod("IsDBNull");
        static readonly MethodInfo s_FnGetValue = 
            typeof(System.Data.IDataRecord).GetMethod("GetValue", new[] { typeof(int) });
        static readonly FieldInfo s_FldConverters = 
            typeof(DataReaderMappingManager).GetField("s_Converters", 
            BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);
        static readonly MethodInfo s_FnListGetItem = 
            typeof(List<Func<object, object>>).GetProperty("Item").GetGetMethod();
        static readonly MethodInfo s_FnInvoke = typeof(Func<object, object>).GetMethod("Invoke");
        static readonly MethodInfo s_FnSignLoaded = typeof(IDataRecord).GetMethod("SignLoaded");

        /// <summary>
        /// Clears the cache of data records.
        /// </summary>
        public static void Reset()
        {
            s_Mappers.Clear();
            s_Converters.Clear();
            s_EnumMapper.Clear();
        }

        /// <summary>
        /// Retrieves the count of mappers
        /// </summary>
        /// <returns></returns>
        public static int GetMapperCount()
        {
            return s_Mappers.Count;
        }

        /// <summary>
        /// Retrieves the count of converters
        /// </summary>
        /// <returns></returns>
        public static int GetConverterCount()
        {
            return s_Converters.Count;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="reader">Data reader instance</param>
        /// <param name="instance">Optional instance to map the data reader into.</param>
        /// <returns></returns>
        public static Func<IDataReader, T, T> GetMapperFor<T>(IDataReader reader, T instance = default(T))
        {
            if (reader == null) throw new ArgumentNullException("reader");

            // --- Check the cache
            // ReSharper disable CompareNonConstrainedGenericWithNull
            var useDefault = (instance == null || instance.Equals(default(T)));
            // ReSharper restore CompareNonConstrainedGenericWithNull
            var mapperKey = CreateMapperKey<T>(reader, useDefault);
            Delegate mapper;
            if (!s_Mappers.TryGetValue(mapperKey, out mapper))
            {
                mapper = CreateMapper<T>(reader, instance);
                s_Mappers[mapperKey] = mapper;
            }
            return (Func<IDataReader, T, T>)mapper;
        }

        /// <summary>
        /// Creates a key for the specified mapper.
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="reader">IDataReader instance</param>
        /// <param name="useDefault">Use default instance?</param>
        /// <returns></returns>
        private static Tuple<Type, string> CreateMapperKey<T>(IDataReader reader, bool useDefault)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                sb.AppendFormat("{0}:{1}|", reader.GetName(i), reader.GetFieldType(i).TypeHandle.Value);
            }
            sb.Append(useDefault);
            return new Tuple<Type, string>(typeof(T), sb.ToString());
        }

        /// <summary>
        /// Creates a mapper for the specified type and data reader.
        /// </summary>
        /// <typeparam name="T">Data record type</typeparam>
        /// <param name="reader">IDataReader instance</param>
        /// <param name="instance">Optional instance to map the data reader into.</param>
        /// <returns>Mapper function</returns>
        public static Func<IDataReader, T, T> CreateMapper<T>(IDataReader reader, object instance = null)
        {
            // --- Create a dynamic method
            var mapperMethod = new DynamicMethod("FetchFactory" + s_Mappers.Count,
                typeof(T),
                new[] { typeof(IDataReader), typeof(T) },
                true);
            var il = mapperMethod.GetILGenerator();
            var metadata = RecordMetadataManager.GetMetadata<T>();
            var dataObject = il.DeclareLocal(typeof(T));

            if (instance == null)
            {
                // --- Create a new instance of T
                il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, Type.EmptyTypes, null));
            }
            else
            {
                // --- Use the default instance
                il.Emit(OpCodes.Ldarg_1);
            }
            il.Emit(OpCodes.Stloc, dataObject);

            // --- Go through all data reader fields
            for (var i = 0; i < reader.FieldCount; i++)
            {
                // --- Skip columns not in the data record
                var columnInfo = metadata[reader.GetName(i)];
                if (columnInfo == null) continue;

                // --- Check if the current field is a DBNull value
                il.Emit(OpCodes.Ldarg_0);                   // reader
                il.Emit(OpCodes.Ldc_I4, i);                 // reader, i
                il.Emit(OpCodes.Callvirt, s_FnIsDbNull);    // bool
                var lblNext = il.DefineLabel();
                il.Emit(OpCodes.Brtrue_S, lblNext);

                // --- Push the data object to the stack
                il.Emit(OpCodes.Ldloc, dataObject);         // dataobject

                // --- Check for custom source converter
                Func<object, object> converter = null;
                if (columnInfo.SourceConverters != null)
                {
                    foreach (var sourceConverter in columnInfo.SourceConverters)
                    {
                        if (sourceConverter.GetDataType() != reader.GetFieldType(i)) continue;
                        converter = sourceConverter.ConvertFromDataType;
                        break;
                    }
                }
                if (converter == null)
                {
                    converter = GetConverter(reader.GetFieldType(i), columnInfo.ClrType);
                }

                // --- If there is a converter, push it to the stack
                if (converter != null)
                {
                    var converterIndex = s_Converters.Count;
                    s_Converters.Add(converter);

                    // --- Push the converter onto the stack
                    il.Emit(OpCodes.Ldsfld, s_FldConverters);
                    il.Emit(OpCodes.Ldc_I4, converterIndex);
                    il.Emit(OpCodes.Callvirt, s_FnListGetItem);
                }

                // --- Get the field value 
                il.Emit(OpCodes.Ldarg_0);	                // dataobject, (converter),  reader
                il.Emit(OpCodes.Ldc_I4, i);                 // dataobject, (converter),  reader, i
                il.Emit(OpCodes.Callvirt, s_FnGetValue);    // datobject, (converter),  fieldvalue

                // --- Use the converter
                if (converter != null)
                {
                    il.Emit(OpCodes.Callvirt, s_FnInvoke);  // dataobject, converted_fieldvalue
                }

                // --- Store the value to the appropriate field
                il.Emit(OpCodes.Unbox_Any, columnInfo.ClrType);
                il.Emit(OpCodes.Callvirt, columnInfo.PropertyInfo.GetSetMethod(true));

                // --- Continue with the next field
                il.MarkLabel(lblNext);
            }

            // --- Allow modification tracking
            if (!metadata.IsSimplePoco)
            {
                il.Emit(OpCodes.Ldloc, dataObject);
                il.Emit(OpCodes.Callvirt, s_FnSignLoaded);
            }

            // --- Return the data record object
            il.Emit(OpCodes.Ldloc, dataObject);
            il.Emit(OpCodes.Ret);

            // --- Create the delegate
            var mapperDelegate = mapperMethod.CreateDelegate(Expression.GetFuncType(typeof(IDataReader), typeof(T), typeof(T)));
            return (Func<IDataReader, T, T>)mapperDelegate;
            // return DoMapping<T>;
        }

        // --- This method is generated dynamically by CreatMapper<T>
        /*
        private static T DoMapping<T>(IDataReader reader, T instance)
        {
            // --- Create an instance of T
            var dataRecord = instance != null ? instance : Activator.CreateInstance<T>();
            var metadata = RecordMetadataManager.GetMetadata<T>();

            // --- Map properties one-by-one
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnInfo = metadata[reader.GetName(i)];
                if (columnInfo == null) continue;

                // --- Null means that the default value of the data record property should not be changed
                if (reader.IsDBNull(i)) continue;

                // --- Check for custom source converter
                Func<object, object> converter = null;
                if (columnInfo.SourceConverters != null)
                {
                    foreach (var sourceConverter in columnInfo.SourceConverters)
                    {
                        if (sourceConverter.GetDataType() != reader.GetFieldType(i)) continue;
                        converter = sourceConverter.ConvertFromDataType;
                        break;
                    }
                }

                // --- Check mapping
                if (converter == null)
                {
                    converter = GetConverter(reader.GetFieldType(i), columnInfo.ClrType);
                }
                var columnValue = reader.GetValue(i);
                if (converter != null) columnValue = converter(columnValue);

                // --- Set property value
                columnInfo.PropertyInfo.SetValue(dataRecord, columnValue);
            }
            return dataRecord;
        }
        */

        /// <summary>
        /// Gets the converter method that can convert a source type to a destination type
        /// </summary>
        /// <param name="sourceType">Source type</param>
        /// <param name="destinationType">Destination type</param>
        /// <returns>Converter function if it can be used; otherwise, null</returns>
        private static Func<object, object> GetConverter(Type sourceType, Type destinationType)
        {
            Func<object, object> converter = null;
            if (destinationType.IsEnum && IsIntegralType(sourceType))
            {
                if (sourceType != typeof (int))
                {
                    converter = src => Convert.ChangeType(src, typeof (int), null);
                }
            }
            else if (!destinationType.IsAssignableFrom(sourceType))
            {
                if (destinationType.IsEnum && sourceType == typeof (string))
                {
                    converter = src => s_EnumMapper.EnumFromString(destinationType, (string) src);
                }
                else
                {
                    converter = src => Convert.ChangeType(src, destinationType, null);
                }
            }
            return converter;
        }

        /// <summary>
        /// Checks whether the specified type is an integral type.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        private static bool IsIntegralType(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode >= TypeCode.SByte && typeCode <= TypeCode.UInt64;
        }

        private class EnumMapper
        {
            private readonly ConcurrentDictionary<Type, Dictionary<string, object>> _stringsToEnums =
                new ConcurrentDictionary<Type, Dictionary<string, object>>();

            /// <summary>
            /// Resets the mapper
            /// </summary>
            public void Clear()
            {
                _stringsToEnums.Clear();
            }

            /// <summary>
            /// Convers an enum value from a string
            /// </summary>
            /// <param name="type">Enumerable type</param>
            /// <param name="value"></param>
            /// <returns></returns>
            public object EnumFromString(Type type, string value)
            {
                PopulateIfNotPresent(type);
                return _stringsToEnums[type][value];
            }

            private void PopulateIfNotPresent(Type type)
            {
                if (!_stringsToEnums.ContainsKey(type))
                {
                        Populate(type);
                }
            }

            private void Populate(Type type)
            {
                var values = Enum.GetValues(type);
                _stringsToEnums[type] = new Dictionary<string, object>(values.Length);

                for (var i = 0; i < values.Length; i++)
                {
                    var value = values.GetValue(i);
                    _stringsToEnums[type].Add(value.ToString(), value);
                }
            }
        }
    }
}