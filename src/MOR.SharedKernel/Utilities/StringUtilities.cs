using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringUtilities
    {
        public const string HTML_NEWLINE = "<br/>";
        private const string DEF_CSV_SEPARATOR = ",";


        public static bool AreEqualCaseInsensitive(string value1, string value2)
        {
            var compr = StringComparer.OrdinalIgnoreCase;
            var ret = compr.Compare(value1, value2) == 0;
            return ret;
        }

        public static string[]? SplitCsv(string? data, string separators = ",", bool removeEmptyEntries = true)
        {
            var ret = default(string[]);

            if (data != null)
            {
                if (string.IsNullOrEmpty(separators))
                {
                    separators = ",";
                }

                var seps = separators.ToCharArray();
                ret = data.Split(seps, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            }

            return ret;
        }

        public static int[]? SplitCsvToInt32(string? data, string separators = ",", bool removeEmptyEntries = true)
        {
            var splits = SplitCsv(data, separators, removeEmptyEntries);
            var ret = StringArrayToIntArray(splits);

            return ret;
        }

        public static IEnumerable<KeyValuePair<string?, string?>>? SplitStringToKeyPairs(string? data, string separators = ",", bool removeEmptyEntries = true)
        {
            var splits = SplitCsv(data, separators, removeEmptyEntries);

            if (splits == null)
            {
                return null;
            }

            var ret = new List<KeyValuePair<string?, string?>>();

            foreach (var str in splits)
            {
                var value = SplitCsv(str, ":", removeEmptyEntries);

                if (value != null)
                {
                    ret.Add(new KeyValuePair<string?, string?>(value[0], value[1]));
                }
            }

            return ret;
        }

        public static int[]? StringArrayToIntArray(string[]? input)
        {
            if (input == null)
            {
                return null;
            }

            var length = input.Length;
            var ret = new int[length];

            for (int i = 0; i < length; i++)
            {
                ret[i] = Int32.Parse(input[i]);
            }

            return ret;
        }

        public static string EscapeXMLString(string target)
        {
            string ret = target;

            if (!string.IsNullOrWhiteSpace(target))
            {
                ret = SecurityElement.Escape(target);
                ret = ret.Replace("&apos;", "'");
            }

            return ret;
        }

        public static string UrlEncodeBase64Value(string base64Value)
        {
            var ret = base64Value
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return ret;
        }

        public static string FormatNewLinesToHtml(string target)
        {
            var ret = target;
            if (!string.IsNullOrWhiteSpace(ret))
            {
                ret = ret.Replace(Environment.NewLine, HTML_NEWLINE);
                ret = ret.Replace("\r\n", HTML_NEWLINE);
                ret = ret.Replace("\r", HTML_NEWLINE);
                ret = ret.Replace("\n", HTML_NEWLINE);
            }
            return ret;
        }

        public static string ReplaceNewLinesWith(string text, string replaceWith = " ")
        {
            var ret = text;
            if (!string.IsNullOrWhiteSpace(ret))
            {
                ret = ret.Replace("\r\n", replaceWith);
                ret = ret.Replace("\r", replaceWith);
                ret = ret.Replace("\n", replaceWith);
            }
            return ret;
        }

        public static StringBuilder ReplaceNewLinesWith(StringBuilder sb, string replaceWith = " ")
        {
            sb.Replace("\r\n", replaceWith);
            sb.Replace("\r", replaceWith);
            sb.Replace("\n", replaceWith);

            return sb;
        }

        public static string WordSearchRegexExperssion(string searchKeyword)
        {
            searchKeyword = searchKeyword.Trim();

            List<string> lstKeywords = new List<string>();

            if (searchKeyword.Contains(","))
            {
                var subList = searchKeyword.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
                subList = subList.Select(t => Regex.Escape(t)).ToList();

                lstKeywords.AddRange(subList);
            }
            else
            {
                lstKeywords.Add(Regex.Escape(searchKeyword));
            }

            var pattern = string.Concat(@"\b(", string.Join("|", lstKeywords), @")\b");
            return pattern;
        }

        public static string? ToCsv<T>(IEnumerable<T>? items, string separator = DEF_CSV_SEPARATOR)
        {
            if (items != null)
            {
                var ret = string.Join(separator, items);
                return ret;
            }

            return null;
        }

        public static string? Coalesce(params string[] values)
        {
            foreach (var val in values)
            {
                if (val != null)
                {
                    return val;
                }
            }

            return null;
        }


        public static string Slice(ref string text, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex + 1;
            var ret = text.Substring(startIndex, length);
            return ret;
        }

        public static string Slice(this string text, int startIndex, int endIndex)
        {
            return Slice(ref text, startIndex, endIndex);
        }

        public static string FindAndReplaceFirst(string text, string what, string until, string replaceWith, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            var lastIx = text.Length - 1;
            var startIx = text.IndexOf(what, comparisonType);
            var endIx = lastIx;

            if (startIx < 0)
            {
                return text;
            }

            if (!string.IsNullOrWhiteSpace(until))
            {
                if (startIx < endIx)
                {
                    var tix = text.IndexOf(until, startIx + 1, comparisonType);

                    if (tix >= 0)
                    {
                        endIx = tix;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(text.Substring(0, startIx));
            sb.Append(replaceWith);

            if (endIx < lastIx)
            {
                sb.Append(text.Substring(endIx));
            }

            var ret = sb.ToString();
            return ret;
        }

        public static string? StringBetween(string source, string start, string end, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var ret = default(string);

            if (!string.IsNullOrWhiteSpace(source))
            {
                var len = source.Length;
                var lastIx = len - 1;
                var ixStart = 0;

                if (!string.IsNullOrWhiteSpace(start))
                {
                    ixStart = source.IndexOf(start, comparison);

                    if (ixStart < 0)
                    {
                        return ret;
                    }

                    ixStart = ixStart + start.Length;
                }

                if (ixStart > lastIx)
                {
                    return ret;
                }
                else if (ixStart == lastIx)
                {
                    return source[lastIx].ToString();
                }

                var ixEnd = !string.IsNullOrWhiteSpace(end) ? source.IndexOf(end, ixStart, comparison) : len;

                if (ixEnd < 0)
                {
                    ixEnd = len;
                }

                ret = source.Substring(ixStart, ixEnd - ixStart);
            }

            return ret;
        }

        public static string ToCamelCase(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return word;
            }

            var code = (int)word[0];

            if (code >= 65 && code <= 90)
            {
                code += 32;

                word = ((char)code).ToString() + word.Remove(0, 1);
            }

            return word;
        }

        public static string ReplaceCaseInsensitive(string input, string replace, string with)
        {
            var ret = Regex.Replace(input, replace, with, RegexOptions.IgnoreCase);
            return ret;
        }

        public static string Base64Decode(this string value, bool throwOnFormatError = false)
        {
            if (value.NotNullOrWhiteSpace())
            {
                try
                {
                    var base64EncodedBytes = Convert.FromBase64String(value);
                    value = Encoding.UTF8.GetString(base64EncodedBytes);
                }
                catch (FormatException)
                {
                    if (throwOnFormatError)
                    {
                        throw;
                    }
                }
            }

            return value;
        }

        public static string Base64Encode(this string value)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64UrlDecode(this string value, bool throwOnFormatError = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            try
            {
                // Convert Base64URL to Base64
                string base64 = value
                    .Replace('-', '+')
                    .Replace('_', '/');

                // Restore padding
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                var bytes = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                if (throwOnFormatError)
                    throw;

                return value;
            }
        }

    }
}
