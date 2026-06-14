using System.Text.Json;
using System.Text.Json.Serialization;

namespace System
{
    [Flags]
    public enum JSFlags
    {
        None = 0,
        Indented = 1,
        EnumAsString = 2,
        CamelCaseProperties = 4,
        CaseInsensitive = 8,
    }

    public static partial class SharedJsonExtensions
    {
        public static string? ToJson(this object? obj, JSFlags flags = JSFlags.None)
        {
            if (obj == null)
            {
                return null;
            }

            var settings = GetSerializerOptions(flags);

            var ret = JsonSerializer.Serialize(obj, settings);
            return ret;
        }

        public static T? FromJson<T>(this string? data, JSFlags flags = JSFlags.None)
        {
            if (data == null)
            {
                return default(T);
            }

            var settings = GetSerializerOptions(flags);

            var ret = JsonSerializer.Deserialize<T>(data, settings);
            return ret;
        }


        internal static JsonSerializerOptions GetSerializerOptions(JSFlags flags)
        {
            var ret = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };

            if (HasFlag(flags, JSFlags.Indented))
            {
                ret.WriteIndented = true;
            }

            if (HasFlag(flags, JSFlags.EnumAsString))
            {
                var converter = new JsonStringEnumConverter();
                ret.Converters.Add(converter);
            }

            if (HasFlag(flags, JSFlags.CamelCaseProperties))
            {
                ret.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }

            ret.PropertyNameCaseInsensitive = HasFlag(flags, JSFlags.CaseInsensitive);

            return ret;
        }

        private static bool HasFlag(this JSFlags flags, JSFlags contains)
        {
            var ret = (flags & contains) == contains;
            return ret;
        }
    }
}
