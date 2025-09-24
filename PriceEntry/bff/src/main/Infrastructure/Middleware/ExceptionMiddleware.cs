namespace Infrastructure.Middleware
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    using Newtonsoft.Json;

    using Serilog;

    [ExcludeFromCodeCoverage]

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger logger;

        public ExceptionMiddleware(RequestDelegate next)
        {
            logger = Log.ForContext<ExceptionMiddleware>();
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                LogException(ex, httpContext);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //// Custom exception handling can be added here if required
            //// switch (exception)
            //// {
            ////     case CustomException customException: set custom error message or status code
            //// }

            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error"
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private void LogException(Exception ex, HttpContext httpContext)
        {
            logger.Error(
                ex,
                "Exception occurred while executing {RequestUrl} with {QueryStringParams}",
                httpContext.Request.Path,
                httpContext.Request.QueryString.Value);
        }
    }
}