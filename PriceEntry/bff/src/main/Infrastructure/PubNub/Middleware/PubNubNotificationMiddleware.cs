namespace Infrastructure.PubNub.Middleware
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    [ExcludeFromCodeCoverage]
    public class PubNubNotificationMiddleware
    {
        private readonly RequestDelegate next;

        public PubNubNotificationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IPubNubNotificationService pubNubNotificationService)
        {
            try
            {
                await next(httpContext);
                await pubNubNotificationService.SendPubNubNotifications();
            }
            catch
            {
                throw;
            }
        }
    }
}
