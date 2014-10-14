using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SeemplestBlocks.Core.ServiceInfrastructure
{
    /// <summary>
    /// This class converts UTC DateTime values to local DateTime values
    /// </summary>
    public class UtcToLocConverter : DateTimeConverterBase
    {
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            return DateTime.Parse(reader.Value.ToString());
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                writer.WriteValue(value);
                return;
            }
            var dateTimeValue = (DateTime)value;
            dateTimeValue = DateTime.SpecifyKind(dateTimeValue, DateTimeKind.Local);
            writer.WriteValue((dateTimeValue.ToString("yyyy-MM-ddTHH:mm:ss")));
        }
    }
}