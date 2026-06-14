using System.Collections.Specialized;

namespace System
{
    public static class UrlUtilities
    {
        public const string UriSchemeSftp = "sftp";

        private static Uri TempUri = new Uri("http://tempuri.org");


        public static string? MultiUrlSplitJoin(string? url)
        {
            var ret = default(string);

            if (!string.IsNullOrWhiteSpace(url))
            {
                ret = string.Join(", ", url.Split(new string[] { ", ", ",\r\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray());
            }

            return ret;
        }

        public static string SafeGetQueryString(NameValueCollection queryCollection, bool prefixQuestion = true)
        {
            var ret = string.Empty;
            var keys = queryCollection.AllKeys;

            if (keys.AnyAndNotNull())
            {
                var qParts = new List<string>();

                foreach (var key in keys)
                {
                    var value = queryCollection[key];
                    value = System.Net.WebUtility.UrlDecode(value);
                    value = System.Net.WebUtility.UrlEncode(value);

                    var qPart = string.Format("{0}={1}", key, value);

                    qParts.Add(qPart);
                }

                ret = string.Join("&", qParts);

                if (prefixQuestion)
                {
                    ret = "?" + ret;
                }
            }

            return ret;
        }

        public static string ConstructQueryString(Dictionary<string, object> items, bool prefixQuestion = false)
        {
            var kvps = items.ToArray();

            return ConstructQueryString(kvps, prefixQuestion);
        }

        public static string ConstructQueryString(IEnumerable<KeyValuePair<string, object>> items, bool prefixQuestion = false)
        {
            var parts = new List<string>();

            foreach (var item in items)
            {
                var name = item.Key;
                var val = item.Value != null ? item.Value.ToString() : null;

                if (val != null)
                {
                    val = System.Net.WebUtility.UrlEncode(val);
                }

                var part = $"{name}={val}";

                parts.Add(part);
            }

            var ret = string.Join("&", parts);

            if (prefixQuestion)
            {
                ret = "?" + ret;
            }

            return ret;
        }

        /// <summary>
        /// Send key/value pairs in the form: key1, value1, key2, value2...
        /// </summary>
        public static string ConstructQueryString(params object[] kvps)
        {
            var parts = new List<string>();
            var q = new Queue<object>(kvps);

            while (q.Any())
            {
                var key = q.Dequeue().ToString();

                var oVal = q.Dequeue();
                var val = oVal == null ? null : oVal.ToString();

                if (val != null)
                {
                    val = System.Net.WebUtility.UrlEncode(val);
                }

                var part = $"{key}={val}";

                parts.Add(part);
            }

            var ret = string.Join("&", parts);
            return ret;
        }


        public static string ForceToAbsoluteUrl(string url)
        {
            var u = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!u.IsAbsoluteUri)
            {
                u = new Uri(TempUri, url);
            }

            return u.OriginalString;
        }

        public static string ForceToAbsoluteUrl(string url, UriFormat format)
        {
            var u = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!u.IsAbsoluteUri)
            {
                u = new Uri(TempUri, url);
            }

            var ret = u.GetComponents(UriComponents.HttpRequestUrl, format);
            return ret;
        }


        public static string EscapeUrl(string rawUrl)
        {
            return EscapeUnescapeCore(rawUrl, true);
        }

        public static string UnescapeUrl(string rawUrl)
        {
            return EscapeUnescapeCore(rawUrl, false);
        }

        private static string EscapeUnescapeCore(string rawUrl, bool escape)
        {
            // Escaping process will be done by the current registered encoder (Default  or AntiXSS as registered)

            var url = new Uri(rawUrl, UriKind.RelativeOrAbsolute);

            if (url.IsAbsoluteUri)
            {
                var ret = url.GetComponents(UriComponents.HttpRequestUrl, escape ? UriFormat.UriEscaped : UriFormat.SafeUnescaped);
                return ret;
            }
            else
            {
                var hasTilde = rawUrl.StartsWith("~");

                if (hasTilde)
                {
                    rawUrl = rawUrl.TrimStart('~');
                }

                url = new Uri(TempUri, rawUrl);

                var ret = url.GetComponents(UriComponents.PathAndQuery, escape ? UriFormat.UriEscaped : UriFormat.SafeUnescaped);

                if (hasTilde)
                {
                    ret = "~" + ret;
                }

                return ret;
            }
        }


        public static void SetBaseAddress(this System.Net.Http.HttpClient client, string url, bool ensureEndSlash = false)
        {
            if (ensureEndSlash && !url.EndsWith("/"))
            {
                url = $"{url}/";
            }

            client.BaseAddress = new Uri(url);
        }
    }
}
