namespace Infrastructure.Logging.SerilogEnrichers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;

    using Serilog.Core;
    using Serilog.Events;

    using Subscriber.Auth;

    [ExcludeFromCodeCoverage]
    public class UserIdEnricher : ILogEventEnricher
    {
        private const string Unknown = "Unknown";

        private const string UserGuidPropertyName = "UserId";

        private readonly HttpContext httpContext;

        private readonly IUserContext userContext;

        public UserIdEnricher(HttpContext httpContext, IUserContext userContext)
        {
            this.httpContext = httpContext;
            this.userContext = userContext;
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
                // Authoring BFF first as that should have the encrypted user id
                if (!string.IsNullOrWhiteSpace(userContext.UserIdEncrypted))
                {
                    return userContext.UserIdEncrypted;
                }

                // If the encrypted user id is not found, then most probably it is for Subscriber BFF
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    return userId;
                }

                return Unknown;
            }

            return Unknown;
        }
    }
}
