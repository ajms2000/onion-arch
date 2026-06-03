using System.Collections;

namespace System
{
    public static class SharedCommonExtensions
    {
        // ***************** Enum Based *****************

        public static TAttribue[]? GetCustomAttributes<TAttribue>(this Enum value)
            where TAttribue : Attribute
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var ret = fieldInfo?.GetCustomAttributes(typeof(TAttribue), false) as TAttribue[];

            return ret;
        }

        public static TAttribue? GetCustomAttribute<TAttribue>(this Enum value, bool throwOnNotFound = false, bool throwOnMultiple = false)
            where TAttribue : Attribute
        {
            var items = value.GetCustomAttributes<TAttribue>();
            var ret = items?.FirstOrDefault();

            if (throwOnNotFound == true && ret == null)
            {
                throw new AppException($"No attribute of type '{typeof(TAttribue).Name}' is found on '{value}'");
            }

            if (throwOnMultiple == true && items?.Length > 1)
            {
                throw new AppException($"Multiple attributes of type '{typeof(TAttribue).Name}' is found on '{value}'");
            }

            return ret;
        }


        // ***************** Collection Based *****************

        public static bool AnyAndNotNull<T>(this IEnumerable<T> items, Func<T, bool>? predicate = null)
        {
            var ret = false;

            if (items != null)
            {
                if (predicate != null)
                {
                    ret = items.Any(predicate);
                }
                else
                {
                    ret = items.Any();
                }
            }

            return ret;
        }

        public static bool TryAddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
                return true;
            }
            else
            {
                return dic.TryAdd(key, value);
            }
        }

        public static string?[] ToStringArray(this IEnumerable items)
        {
            var ret = new List<string?>();
            foreach (var item in items)
            {
                ret.Add(item?.ToString());
            }
            return ret.ToArray();
        }


        // ***************** String Based *****************

        public static string?[] WrapInArray(this string? value, bool includeEmpty = false)
        {
            var ret = new List<string?>();

            if (!includeEmpty)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    ret.Add(value);
                }
            }
            else
            {
                ret.Add(value);
            }

            return ret.ToArray();
        }

        public static bool Contains(this string? text, string search, StringComparison comparision)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            var ix = text.IndexOf(search, comparision);
            return ix >= 0;
        }

        public static bool NullOrWhiteSpace(this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool NotNullOrWhiteSpace(this string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string? EmptyToNull(this string? value, bool trim = false)
        {
            if (trim == true && value != null)
            {
                value = value.Trim();
            }

            if (value == string.Empty)
            {
                return null;
            }

            return value;
        }

        public static string? NullToEmpty(this string? value)
        {
            value = value.NullOrWhiteSpace() ? string.Empty : value;
            return value;
        }


        public static bool EqualsInvariant(this string? source, string target)
        {
            var ret = StringComparer.InvariantCulture.Equals(source, target);
            return ret;
        }

        public static bool EqualsInvariantIgnoreCase(this string? source, string target)
        {
            var ret = StringComparer.InvariantCultureIgnoreCase.Equals(source, target);
            return ret;
        }

        public static bool EqualsOrdinal(this string? source, string target)
        {
            var ret = StringComparer.Ordinal.Equals(source, target);
            return ret;
        }

        public static bool EqualsOrdinalIgnoreCase(this string? source, string target)
        {
            var ret = StringComparer.OrdinalIgnoreCase.Equals(source, target);
            return ret;
        }

        // ***************** Misc *****************

        public static T Default<T>(this T? value, T defaultValue = default(T))
            where T : struct
        {
            if (value == null)
            {
                return defaultValue;
            }

            return value.Value;
        }

        public static T[] WrapInArray<T>(this T value)
        {
            var ret = new List<T>() { value };
            return ret.ToArray();
        }

        public static string ToYesNo(this bool val, bool uppercase = false)
        {
            var ret = val ? "Yes" : "No";

            if (uppercase)
            {
                ret = ret.ToUpper();
            }

            return ret;
        }


        public static MemoryStream ToMemoryStream(this byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            ms.Seek(0L, SeekOrigin.Begin);
            return ms;
        }
    }
}
