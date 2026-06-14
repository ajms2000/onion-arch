namespace System
{
    public static class DateTimeUtilities
    {
        /// <summary>
        /// For 'Unspecified' kind of dates, only the Kind will be changed to UTC, not the number of ticks.
        /// </summary>
        public static DateTime ToGMT(this DateTime source)
        {
            if (source.Kind == DateTimeKind.Unspecified)
            {
                source = DateTime.SpecifyKind(source, DateTimeKind.Utc);
            }
            else if (source.Kind == DateTimeKind.Local)
            {
                source = source.ToUniversalTime();
            }

            return source;
        }

        /// <summary>
        /// Source must be of kind Unspecified or Universal
        /// </summary>
        public static DateTime UnspecifiedToUtc(this DateTime source, string sourceTimeZoneIdentifier)
        {
            if (sourceTimeZoneIdentifier.NullOrWhiteSpace())
            {
                throw new ArgumentNullException("sourceTimeZoneIdentifier");
            }

            if (source.Kind == DateTimeKind.Unspecified)
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneIdentifier);
                source = TimeZoneInfo.ConvertTimeToUtc(source, timeZone);
            }
            else if (source.Kind == DateTimeKind.Local)
            {
                throw new NotSupportedException();
            }

            return source;
        }

        public static DateTime ToTimeZone(this DateTime source, string sourceTimeZoneIdentifier)
        {
            if (sourceTimeZoneIdentifier.NullOrWhiteSpace())
            {
                throw new ArgumentNullException("sourceTimeZoneIdentifier");
            }

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneIdentifier);

            if (source.Kind == DateTimeKind.Unspecified)
            {
                source = source.ToGMT();
            }
            else if (source.Kind == DateTimeKind.Local)
            {
                source = source.ToUniversalTime();
            }

            source = TimeZoneInfo.ConvertTimeFromUtc(source, timeZone);

            return source;
        }

        public static DateTime ToUnspecifiedKind(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Unspecified)
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Unspecified);
            }

            return dateTime;
        }

        /// <param name="dateTime">This datetime will be assumed as Unspecified kind.</param>
        public static DateTime ConvertDateTimeZone(DateTime dateTime, string sourceTimeSourceId, string targetTimeSourceId)
        {
            var dt = dateTime.ToUnspecifiedKind();

            var tsSrc = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeSourceId);
            var tsTgt = TimeZoneInfo.FindSystemTimeZoneById(targetTimeSourceId);

            var ret = TimeZoneInfo.ConvertTime(dt, tsSrc, tsTgt);
            return ret;
        }
    }
}
