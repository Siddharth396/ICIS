namespace Infrastructure.Middleware
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Infrastructure.Logging.SerilogEnrichers;

    using Microsoft.AspNetCore.Http;

    using Serilog.Context;

    [ExcludeFromCodeCoverage]
    public class HttpContextLoggingMiddleware
    {
        private readonly RequestDelegate next;

        public HttpContextLoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using (LogContext.Push(
                new UserGuidEnricher(httpContext),
                new CorrelationIdEnricher(httpContext),
                new ExceptionEnricher()))
            {
                await next(httpContext);
            }
        }
    }
}