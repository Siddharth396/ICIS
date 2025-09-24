namespace Test.Infrastructure.Stubs
{
    using System;

    using BusinessLayer.Helpers;

    using Microsoft.Extensions.Internal;

    public class TestClock : ISystemClock
    {
        private DateTimeOffset? utcNow;

        public DateTimeOffset UtcNow
        {
            get
            {
                if (utcNow == null)
                {
                    return DateTimeOffset.UtcNow;
                }

                return utcNow.Value;
            }
        }

        public void SetUtcNow(DateTimeOffset now)
        {
            utcNow = now;
        }

        public void SetUtcNow(DateTime now)
        {
            utcNow = new DateTimeOffset(UtcDateTime.GetUtcDateTime(now));
        }

        public void Reset()
        {
            utcNow = null;
        }
    }
}