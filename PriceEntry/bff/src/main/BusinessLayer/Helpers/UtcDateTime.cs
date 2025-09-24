namespace BusinessLayer.Helpers
{
    using System;

    public static class UtcDateTime
    {
        public static DateTime GetUtcDateTime(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static DateTime? GetUtcDateTime(DateOnly? dateOnly)
        {
            return dateOnly?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }
    }
}
