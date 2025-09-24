namespace Infrastructure.HealthChecks
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class HealthCheckTag
    {
        public const string Liveness = "liveness";

        public const string Readiness = "readiness";
    }
}
