namespace Infrastructure.Middleware
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Builder;

    [ExcludeFromCodeCoverage]
    public static class HttpContextLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSerilogEnrichers(this IApplicationBuilder app)
        {
            app.UseMiddleware<HttpContextLoggingMiddleware>();
            return app;
        }
    }
}