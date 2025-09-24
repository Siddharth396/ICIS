namespace Infrastructure.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.Extensions.Internal;

    [ExcludeFromCodeCoverage]
    public class Clock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
