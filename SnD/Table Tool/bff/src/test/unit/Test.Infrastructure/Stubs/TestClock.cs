namespace Test.Infrastructure.Stubs
{
    using System;

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
    }
}