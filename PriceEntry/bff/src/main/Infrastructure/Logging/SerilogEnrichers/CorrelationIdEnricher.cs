namespace Infrastructure.Logging.SerilogEnrichers
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Http;

    using Serilog.Core;
    using Serilog.Events;

    [ExcludeFromCodeCoverage]
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";

        private const string CorrelationIdHeaderName = "CorrelationId";

        private readonly HttpContext httpContext;

        public CorrelationIdEnricher(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var correlationId = GetCorrelationId();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(CorrelationIdPropertyName, correlationId));
            }
        }

        private string GetCorrelationId()
        {
            try
            {
                if (httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var value))
                {
                    return value;
                }
            }
            catch
            {
                // ignore any exception to not break the pipeline
            }

            return Guid.NewGuid().ToString();
        }
    }
}