namespace Infrastructure.PubNub
{
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.PubNub.Middleware;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using PubnubApi;

    [ExcludeFromCodeCoverage]
    public static class PubNubRegistration
    {
        public static IServiceCollection RegisterPubNub(this IServiceCollection services)
        {
            services.AddOptions<PubNubOptions>()
               .BindConfiguration(PubNubOptions.ConfigKey)
               .ValidateDataAnnotations()
               .ValidateOnStart();

            services.AddSingleton(
                p =>
                {
                    var options = p.GetService<IOptions<PubNubOptions>>()!.Value;
                    var pubNubConfig = new PNConfiguration(new UserId(options?.Uuid))
                    {
                        SubscribeKey = options?.SubscribeKey,
                        PublishKey = options?.PublishKey
                    };
                    return new Pubnub(pubNubConfig);
                });
            services.AddPubNubNotificationService();
            return services;
        }

        public static IApplicationBuilder UsePubNubNotificationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<PubNubNotificationMiddleware>();

            return app;
        }

        /// <summary>
        /// Adds the PubNubNotificationService to the service collection.
        /// This should be scoped to ensure that the PubNubNotificationService is created once per request.
        /// </summary>
        /// <param name="services"></param>
        private static void AddPubNubNotificationService(this IServiceCollection services)
        {
            services.AddScoped<IPubNubNotificationService, PubNubNotificationService>();
        }
    }
}
