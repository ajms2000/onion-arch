using System.Text.Json.Serialization;

namespace System.Text.Json
{
    public class JsonConverterDateTime : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var ret = reader.GetDateTime();
            return ret;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("s"));
        }
    }

    public class JsonConverterDateTimeOffset : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var ret = reader.GetDateTimeOffset();
            return ret;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("s") + value.ToString("%K"));
        }
    }
}
