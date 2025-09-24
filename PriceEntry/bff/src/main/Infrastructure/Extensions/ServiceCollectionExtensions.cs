namespace Infrastructure.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.Configuration;
    using Infrastructure.Configuration.Model;
    using Infrastructure.EventDispatcher;
    using Infrastructure.GraphQL;
    using Infrastructure.HealthChecks;
    using Infrastructure.MongoDB;
    using Infrastructure.Services;
    using Infrastructure.Services.Version;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Internal;

    using Prometheus;

    using Serilog;

    using Subscriber.Auth;

    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IHealthChecksBuilder>? registerOwnHealthChecks = null)
        {
            var mongoDbOptions = configuration.GetOptions<MongoDbOptions>();

            var healthChecksBuilder = services.AddHealthChecks()
               .AddDefaultHealthCheck()
               .AddMongoDbCheck(mongoDbOptions)
               .ForwardToPrometheus();

            registerOwnHealthChecks?.Invoke(healthChecksBuilder);

            services.AddHttpClient();

            services.AddHttpContextAccessor();

            services.AddControllers();
            services.Configure<ForwardedHeadersOptions>(
                options =>
                {
                    options.ForwardLimit = null;
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });

            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.WithOrigins(configuration.GetCorsOrigins())
                               .SetIsOriginAllowedToAllowWildcardSubdomains()
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .SetPreflightMaxAge(TimeSpan.FromDays(1));
                        });
                });

            services.AddSingleton<VersionService>()
               .AddMongoDb()
               .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
               .AddScoped<IUserContext, UserContext>()
               .AddScoped<IErrorReporter, GraphQLErrorReporter>()
               .AddScoped<IEventDispatcher, EventDispatcher>();

            services.AddTransient<ISystemClock, Clock>();

            services.AddSingleton(Log.Logger);
        }
    }
}
