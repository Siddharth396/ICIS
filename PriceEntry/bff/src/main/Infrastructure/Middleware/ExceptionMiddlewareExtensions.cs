namespace Infrastructure.Middleware
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Builder;

    [ExcludeFromCodeCoverage]
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            return app;
        }
    }
}