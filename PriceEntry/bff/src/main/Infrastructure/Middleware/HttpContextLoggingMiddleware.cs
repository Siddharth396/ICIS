namespace Infrastructure.Middleware
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Infrastructure.Logging.SerilogEnrichers;

    using Microsoft.AspNetCore.Http;

    using Serilog.Context;

    using Subscriber.Auth;

    [ExcludeFromCodeCoverage]
    public class HttpContextLoggingMiddleware
    {
        private readonly RequestDelegate next;

        public HttpContextLoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext, IUserContext userContext)
        {
            using (LogContext.Push(
                       new UserIdEnricher(httpContext, userContext),
                       new CorrelationIdEnricher(httpContext),
                       new ExceptionEnricher()))
            {
                await next(httpContext);
            }
        }
    }
}
