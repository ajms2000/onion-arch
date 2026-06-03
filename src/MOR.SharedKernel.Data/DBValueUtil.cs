namespace System.Data
{
    public static class DBValueUtil
    {
        public static T GetValue<T>(object value, T defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            else
                return (T)value;
        }

        public static T GetNumber<T>(object value)
            where T : struct
        {
            if (value == null || value == DBNull.Value)
            {
                object o = 0.0;
                return (T)o;
            }
            else
                return (T)value;
        }

        public static Guid GetGuid(object value)
        {
            if (value == null || value == DBNull.Value)
                return Guid.Empty;
            else
                return (Guid)value;
        }

        public static string? GetString(object? value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            else
                return (string)value;
        }

        public static string GetEmptyString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            else
                return (string)value;
        }

        public static bool GetBooleanFromString(object obj)
        {
            bool ret = false;
            if (obj != DBNull.Value && obj != null)
            {
                var strObj = (string)obj;
                ret = StringUtilities.AreEqualCaseInsensitive(strObj, bool.TrueString);
            }
            return ret;
        }


        public static T SafeGetValue<T>(DataRow row, string columnName, T defaultValue)
        {
            object? val = null;

            try
            {
                val = row[columnName];
            }
            catch
            {
                // Ignore if col doesn't exist
            }

            if (val == null || val == DBNull.Value)
                return defaultValue;
            else
                return (T)val;
        }
    }
}
