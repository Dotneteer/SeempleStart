using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Seemplest.Core.Common;
using Seemplest.Core.DataAccess.DataRecords;

namespace Seemplest.Core.DataAccess.Tracking
{
    /// <summary>
    /// This class represents a primary key value
    /// </summary>
    public class PrimaryKeyValue
    {
        /// <summary>
        /// Gets the elements of the primary key
        /// </summary>
        public readonly IReadOnlyList<object> KeyElement;

        /// <summary>
        /// Gets the string representation of the primary key
        /// </summary>
        public readonly string KeyString;

        /// <summary>
        /// Initializes the primary key value with the specified elements
        /// </summary>
        public PrimaryKeyValue(IDataRecord dataRecord, DataRecordDescriptor descriptor)
            : this(descriptor
                .PrimaryKeyColumns
                .Select(c => c.PropertyInfo.GetValue(dataRecord)))
        {
        }

        /// <summary>
        /// Initializes the primary key value with the specified elements
        /// </summary>
        public PrimaryKeyValue(IEnumerable<object> pkValues)
        {
            // --- Obtain the primary key elements
            KeyElement = new ReadOnlyCollection<object>(
                pkValues.ToList());

            // --- Create the primary key string
            var sb = new StringBuilder();
            foreach (var element in KeyElement)
            {
                var byteArrElement = element as byte[];
                sb.AppendFormat("[{0}]", byteArrElement == null
                              ? element.ToString()
                              : TypeConversionHelper.ByteArrayToString(byteArrElement));
            }
            KeyString = sb.ToString();
        }

        /// <summary>
        /// Compares this element with an other one
        /// </summary>
        /// <param name="other">Other element</param>
        /// <returns>True, if the two elemens are equal; otherwise, false</returns>
        protected bool Equals(PrimaryKeyValue other)
        {
            return string.Equals(KeyString, other.KeyString);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PrimaryKeyValue) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return KeyString.GetHashCode();
        }
    }
}