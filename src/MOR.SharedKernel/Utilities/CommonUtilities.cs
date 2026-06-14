using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace System
{
    public static class CommonUtilities
    {
        private static string[] ROMAN_SYMBOLS = { "MMM", "MM", "M", "CM", "DCCC", "DCC", "DC", "D", "CD", "CCC", "CC", "C", "XC", "LXXX", "LXX", "LX", "L", "XL", "XXX", "XX", "X", "IX", "VIII", "VII", "VI", "V", "IV", "III", "II", "I" };
        private static int[] ROMAN_INV_VALUES = { 3000, 2000, 1000, 900, 800, 700, 600, 500, 400, 300, 200, 100, 90, 80, 70, 60, 50, 40, 30, 20, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        private static uint ALPHA_A_CAP = 'A';
        private static uint ALPHA_A_LOW = 'a';

        private static char[] ALPHA_CAPITAL_CHARS = new char[26];
        private static char[] ALPHA_SMALL_CHARS = new char[26];
        private static char[] NUMERIC_CHARS = new char[10];
        private static char[] ALPHA_NUMERIC_CHARS = new char[62];

        private static string[] ALPHA_CAPITAL_STRS = new string[26];
        private static string[] ALPHA_SMALL_STRS = new string[26];
        private static string[] NUMERIC_STRS = new string[10];
        private static string[] ALPHA_NUMERIC_STRS = new string[62];


        public const char SLASH_B = '\\';
        public const char SLASH_F = '/';

        public static readonly char[] CHARS_SLASHES = new[] { SLASH_B, SLASH_F };
        public static readonly char[] CHARS_SLASH_B = new[] { SLASH_B };
        public static readonly char[] CHARS_SLASH_F = new[] { SLASH_F };


        static CommonUtilities()
        {
            BuildAlphaNumericTable();
        }


        public static byte[] StreamToBytes(Stream stream)
        {
            using (var ms = stream.ToMemoryStream())
            {
                var ret = ms.ToArray();
                return ret;
            }
        }

        public async static Task<byte[]> StreamToBytesAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            using (var ms = await stream.ToMemoryStreamAsync(cancellationToken))
            {
                var ret = ms.ToArray();
                return ret;
            }
        }

        public async static Task<MemoryStream> ToMemoryStreamAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            var ms = new MemoryStream();

            await stream.CopyToAsync(ms, cancellationToken);
            ms.Seek(0L, SeekOrigin.Begin);

            return ms;
        }

        public static MemoryStream ToMemoryStream(this Stream stream)
        {
            if (stream.CanRead)
            {
                var ms = new MemoryStream();

                stream.CopyTo(ms);
                ms.SeekBegin();

                return ms;
            }

            throw new NotSupportedException("Stream does not support read.");
        }

        public static void SeekBegin(this Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
        }


        public static void DisposeSafe(this IDisposable? obj)
        {
            if (obj != null)
            {
                try
                {
                    obj.Dispose();
                }
                catch
                {
                    // Do nothing
                }
            }
        }


        public static byte[] ReadAllBytes(this BinaryReader br, int bufferSize = 4096)
        {
            var list = new List<byte>();

            do
            {
                var bytes = br.ReadBytes(bufferSize);

                if (!bytes.Any())
                {
                    break;
                }

                list.AddRange(bytes);
            }
            while (true);

            var ret = list.ToArray();
            return ret;
        }


        public static string ToHex(byte[] bytes, string separator = "", string prefix = "")
        {
            var hex = BitConverter.ToString(bytes);
            hex = prefix + hex.Replace("-", separator);
            return hex;
        }

        public static XmlWriterSettings GetBasicXmlWriterSettings(bool omitXmlDeclaration)
        {
            XmlWriterSettings ret = new XmlWriterSettings();
            ret.NewLineChars = Environment.NewLine;
            ret.Indent = true;
            ret.OmitXmlDeclaration = omitXmlDeclaration;
            return ret;
        }

        public static bool EnumHasUnderlyingValue<TEnum>(int value, params TEnum[] exclude)
            where TEnum : struct
        {
            var array = (TEnum[])Enum.GetValues(typeof(TEnum));
            array = array.Where(t => !exclude.Contains(t)).ToArray();
            var values = array.Cast<int>();
            var ret = values.Contains(value);
            return ret;
        }

        public static TEnum[] GetSelectedValuesFromFlagEnumValue<TEnum>(TEnum value, bool considerZero = false)
            where TEnum : struct
        {
            var type = typeof(TEnum);

            var allValues = Enum.GetValues(type).Cast<int>().ToArray();
            var checkVal = Convert.ToInt32(value);

            if (!considerZero)
            {
                allValues = allValues.Where(t => t != 0).ToArray();
            }

            var ret = allValues
                .Where(t => (checkVal & t) == t)
                .Cast<TEnum>()
                .ToArray();

            return ret;
        }

        public static TEnum ParseEnum<TEnum>(string value, bool ignoreCase = true)
            where TEnum : struct
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
            {
                throw new AppException("Generic type is not an enum.");
            }

            var ret = (TEnum)Enum.Parse(type, value, ignoreCase);
            return ret;
        }

        public static TEnum ParseFlagsEnum<TEnum>(string value, bool ignoreCase = true)
            where TEnum : struct
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
            {
                throw new AppException("Generic type is not an enum.");
            }

            var splits = value
                .Split("|".WrapInArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToArray();

            var intEnumVals = splits.Select(t => (int)Enum.Parse(type, t)).ToArray();
            var tempRet = 0;

            for (int i = 0; i < intEnumVals.Length; i++)
            {
                tempRet = tempRet | intEnumVals[i];
            }

            var ret = (TEnum)(object)tempRet;
            return ret;
        }

        public static TEnum[] SanitizeEnumValues<TEnum>(IEnumerable<TEnum> values)
            where TEnum : struct, Enum
        {
            var allVals = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
            var intersection = allVals.Intersect(values).ToArray();

            return intersection;
        }

        public static bool HasAllEnumValues<TEnum>(IEnumerable<TEnum> values)
            where TEnum : struct, Enum
        {
            var allVals = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
            var intersection = allVals.Intersect(values).ToArray();

            var ret = intersection.Length == allVals.Length;
            return ret;
        }

        public static KeyValuePair<string, string?> GetCustomPrimaryAndSecondaryAssemblyVersions(Assembly asm)
        {
            var custAttribs = asm.GetCustomAttributesData().ToList();

            var ca = custAttribs
                .Where(t => t.AttributeType == typeof(AssemblyMetadataAttribute) && t.ConstructorArguments.Any(f => object.Equals(f.Value, "VER")) == true)
                .FirstOrDefault();

            var asmVer = asm.GetName().Version;
            var primaryVersion = string.Format("{0}.{1}", asmVer?.Major ?? 0, (asmVer?.Minor ?? 0).ToString("00"));
            var secondaryVersion = (string?)ca?.ConstructorArguments.Last().Value;

            var ret = new KeyValuePair<string, string?>(primaryVersion, secondaryVersion);
            return ret;
        }

        /// <param name="index">1 based column index</param>
        public static string GetExcelColNameFromIndex(this int index)
        {
            if (index < 1)
            {
                var msg = string.Format("Index cannot be less than 1. Current value: {0}", index);
                throw new ArgumentOutOfRangeException(msg);
            }

            var ret = string.Empty;
            var bal = index;

            while (true)
            {
                var remainder = bal % 26;

                if (remainder == 0)
                {
                    ret = ALPHA_CAPITAL_STRS[25] + ret;

                    bal -= 26;
                    bal = bal / 26;
                }
                else
                {
                    ret = ALPHA_CAPITAL_STRS[remainder - 1] + ret;
                    bal = bal - remainder;
                    bal = bal / 26;
                }

                if (bal == 0)
                {
                    break;
                }
            }

            return ret;
        }

        public static string DeepGetExceptionMessages(Exception ex)
        {
            var sb = new StringBuilder();
            var tex = ex;

            while (tex != null)
            {
                sb.AppendLine(tex.Message.Trim());
                sb.AppendLine();

                tex = tex.InnerException;
            }

            var ret = sb.ToString();
            return ret;
        }


        public static NetworkCredential? GetBasicCredentials(string? header)
        {
            var ret = default(NetworkCredential);

            if ((header != null) && header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {

                var encodedUP = StringUtilities.SplitCsv(header, " ");

                if (encodedUP != null && encodedUP.Length > 1)
                {
                    var encodedUPBytes = Convert.FromBase64String(encodedUP[1].Trim());
                    var decodedUP = Encoding.UTF8.GetString(encodedUPBytes);
                    var decodedUPSplit = decodedUP.Split(':');

                    var u = decodedUPSplit.Length > 0 ? decodedUPSplit[0] : string.Empty;
                    var p = decodedUPSplit.Length > 1 ? decodedUPSplit[1] : string.Empty;

                    ret = new NetworkCredential(u, p);
                }
            }

            return ret;
        }


        public static void ReleaseMemory(int delayFirst = 2000, int delaySecond = 10000)
        {
            //Task.Delay(delayFirst).Wait();
            System.Threading.Thread.Sleep(delayFirst);

            GC.Collect();

            //Task.Delay(delaySecond).ContinueWith(t =>
            //{
            //	GC.Collect();
            //});
        }

        public static KeyValuePair<int, T>[,] DistributeObjectsVerticallyInMatrix<T>(List<T> items, int columns, T defaultValue)
        {
            var totalCount = items.Count;
            var remainder = totalCount % columns;
            var isPerfect = remainder == 0;
            var highestDivisibleTotal = (totalCount - remainder);
            var perfectRows = highestDivisibleTotal / columns;
            var rows = isPerfect ? perfectRows : (perfectRows + 1);
            var lastRowIx = rows - 1;

            var lastRowColFlags = new bool[columns];

            for (int i = 0; i < columns; i++)
            {
                if (isPerfect)
                {
                    lastRowColFlags[i] = true;
                }
                else
                {
                    lastRowColFlags[i] = i < remainder;
                }
            }

            var ret = new KeyValuePair<int, T>[rows, columns];
            var ix = 0;

            for (int c = 0; c < columns; c++)
            {
                var lrColFLag = lastRowColFlags[c];

                for (int r = 0; r < rows; r++)
                {
                    var isLastRow = r == lastRowIx;

                    if (isLastRow && !lrColFLag)
                    {
                        ret[r, c] = new KeyValuePair<int, T>(int.MinValue, defaultValue);
                    }
                    else
                    {
                        ret[r, c] = new KeyValuePair<int, T>(ix, items[ix]);
                        ix++;
                    }
                }
            }

            return ret;
        }


        public static int GetIntUniqueID()
        {
            var ret = Math.Abs(Guid.NewGuid().GetHashCode());
            return ret;
        }

        public static string GetStringUniqueIDIncremental(bool lowercase = false)
        {
            System.Threading.Thread.Sleep(10);

            Task.Delay(10).Wait();

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var arr = timestamp.ToArray();

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (char)((int)arr[i] + 17);
            }

            var ret = string.Concat(arr);

            if (lowercase)
            {
                ret = ret.ToLower();
            }

            return ret;
        }

        public static byte[] GenerateRandomBytes(int length, bool nonZero = true)
        {
            var ret = new byte[length];
            var rng = RandomNumberGenerator.Create();

            if (nonZero)
            {
                rng.GetNonZeroBytes(ret);
            }
            else
            {
                rng.GetBytes(ret);
            }

            return ret;
        }

        public static string GenerateRandomAlphaKey(int length)
        {
            var tableLength = ALPHA_CAPITAL_STRS.Length + ALPHA_SMALL_STRS.Length;
            var halfLength = tableLength / 2;

            var rBytes = GenerateRandomBytes(length);
            var retBytes = new string[length];

            for (int i = 0; i < length; i++)
            {
                var ascii = (int)rBytes[i];
                var remainder = ascii % tableLength;

                if (remainder < halfLength)
                {
                    retBytes[i] = ALPHA_CAPITAL_STRS[remainder];
                }
                else
                {
                    retBytes[i] = ALPHA_SMALL_STRS[remainder - halfLength];
                }
            }

            var ret = string.Join(string.Empty, retBytes);
            return ret;
        }

        public static string GenerateRandomAlphaNumericKey(int length, bool mustHaveAnAlpha = false, bool mustHaveANumeric = false)
        {
            var ret = default(string);

            var tableLength = ALPHA_NUMERIC_STRS.Length;
            var rBytes = GenerateRandomBytes(length);
            var retBytes = new string[length];

            var hasAlpha = false;
            var hasNumeric = false;

            for (int i = 0; i < length; i++)
            {
                var ascii = (int)rBytes[i];
                var remainder = ascii % tableLength;

                retBytes[i] = ALPHA_NUMERIC_STRS[remainder];

                if (remainder < 10)
                {
                    hasNumeric = true;
                }
                else
                {
                    hasAlpha = true;
                }
            }

            if (mustHaveAnAlpha && mustHaveANumeric && !(hasAlpha && hasNumeric))
            {
                ret = GenerateRandomAlphaNumericKey(length, mustHaveAnAlpha, mustHaveANumeric);
            }
            else if (mustHaveAnAlpha && !hasAlpha)
            {
                ret = GenerateRandomAlphaNumericKey(length, mustHaveAnAlpha, mustHaveANumeric);
            }
            else if (mustHaveANumeric && !hasNumeric)
            {
                ret = GenerateRandomAlphaNumericKey(length, mustHaveAnAlpha, mustHaveANumeric);
            }
            else
            {
                ret = string.Join(string.Empty, retBytes);
            }

            return ret;
        }


        public static string ToRoman(this int number, bool isLower = false)
        {
            return ToRoman((uint)number, isLower);
        }

        public static string ToRoman(this uint number, bool isLower = false)
        {
            if (number <= 0)
            {
                throw new IndexOutOfRangeException($"Invalid number {number}. Must be greater that 0.");
            }

            var sb = new StringBuilder();
            var n = (int)number;
            var index = 0;

            while (n != 0)
            {
                if (n >= ROMAN_INV_VALUES[index])
                {
                    n -= ROMAN_INV_VALUES[index];

                    var symb = isLower ? ROMAN_SYMBOLS[index].ToLower() : ROMAN_SYMBOLS[index];

                    sb.Append(symb);
                }
                else
                {
                    index++;
                }
            }

            return sb.ToString();
        }

        public static string ToAlphaSequence(this int number, bool isLower = false)
        {
            return ToAlphaSequence((uint)number, isLower);
        }

        public static string ToAlphaSequence(this uint number, bool isLower = false)
        {
            if (number <= 0 || number > 26)
            {
                throw new IndexOutOfRangeException($"Invalid number {number}. Must be greater that 0 and less than 26.");
            }

            var baseval = isLower ? ALPHA_A_LOW : ALPHA_A_CAP;
            var ret = (char)(baseval + number - 1);

            return ret.ToString();
        }


        private static void BuildAlphaNumericTable()
        {
            var iZero = (int)'0';
            var iACap = (int)ALPHA_A_CAP;
            var iASml = (int)ALPHA_A_LOW;

            for (int i = 0; i < 10; i++)
            {
                NUMERIC_CHARS[i] = (char)(iZero + i);
                ALPHA_NUMERIC_CHARS[i] = NUMERIC_CHARS[i];

                NUMERIC_STRS[i] = NUMERIC_CHARS[i].ToString();
                ALPHA_NUMERIC_STRS[i] = NUMERIC_STRS[i];
            }

            for (int i = 0; i < 26; i++)
            {
                var ix1 = 10 + i;
                var ix2 = 10 + 26 + i;

                ALPHA_CAPITAL_CHARS[i] = (char)(iACap + i);
                ALPHA_SMALL_CHARS[i] = (char)(iASml + i);

                ALPHA_NUMERIC_CHARS[ix1] = ALPHA_CAPITAL_CHARS[i];
                ALPHA_NUMERIC_CHARS[ix2] = ALPHA_SMALL_CHARS[i];

                ALPHA_CAPITAL_STRS[i] = ALPHA_CAPITAL_CHARS[i].ToString();
                ALPHA_SMALL_STRS[i] = ALPHA_SMALL_CHARS[i].ToString();

                ALPHA_NUMERIC_STRS[ix1] = ALPHA_CAPITAL_STRS[i];
                ALPHA_NUMERIC_STRS[ix2] = ALPHA_SMALL_STRS[i];
            }
        }


        public static string CSVfy<T>(IEnumerable<T> items, string delimiter = ",", string itemWrapChar = "")
        {
            var sb = new StringBuilder();

            var props = typeof(T).GetProperties();
            var headers = props.Select(t => $"{itemWrapChar}{t.Name}{itemWrapChar}").ToArray();

            sb.AppendLine(string.Join(delimiter, headers));

            foreach (var item in items)
            {
                var vals = props.Select(t =>
                {
                    var obj = t.GetValue(item);
                    var str = $"{itemWrapChar}{obj}{itemWrapChar}";
                    return str;
                }).ToArray();

                sb.AppendLine(string.Join(delimiter, vals));
            }

            var ret = sb.ToString();
            return ret;
        }

        public static int GetInt(string value, int defaultValue = int.MinValue)
        {
            var ret = defaultValue;

            if (!int.TryParse(value, out ret))
            {
                throw new Exception("Unable to parse provided value to int.");
            }

            return ret;
        }
    }
}
