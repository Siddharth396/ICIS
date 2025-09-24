namespace BusinessLayer.Helpers
{
    using System;

    public static class UnixTimestampConverter
    {
        public static DateTime FromUnixTimeMilliseconds(this long unixTimestamp)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp);
            return dateTimeOffset.UtcDateTime;
        }

        public static long ToUnixTimeMilliseconds(this DateTime utcDateTime)
        {
            var dateTimeOffset = new DateTimeOffset(UtcDateTime.GetUtcDateTime(utcDateTime));
            return dateTimeOffset.ToUnixTimeMilliseconds();
        }
    }
}
