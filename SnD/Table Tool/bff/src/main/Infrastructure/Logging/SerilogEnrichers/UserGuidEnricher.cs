namespace Infrastructure.Logging.SerilogEnrichers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;

    using Serilog.Core;
    using Serilog.Events;

    [ExcludeFromCodeCoverage]
    public class UserGuidEnricher : ILogEventEnricher
    {
        private const string UserGuidPropertyName = "UserGuid";

        private const string Unknown = "Unknown";

        private readonly HttpContext httpContext;

        public UserGuidEnricher(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var property = propertyFactory.CreateProperty(UserGuidPropertyName, GetUserGuid());
            logEvent.AddPropertyIfAbsent(property);
        }

        private string GetUserGuid()
        {
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                return httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Unknown;
            }

            return Unknown;
        }
    }
}