namespace Infrastructure.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.Configuration;
    using Infrastructure.GraphQL;
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
            Action<IServiceCollection>? registerOverrides)
        {

            services.AddHttpClient();

            services.AddHttpContextAccessor();
            services.AddHealthCheckExtension(configuration.GetConnectionString("ConnectionString"));
            services.AddControllers().AddNewtonsoftJson();
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
               .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
               .AddScoped<IUserContext, UserContext>()
               .RegisterConfigurationSections(configuration)
               .AddScoped<IErrorReporter, GraphQLErrorReporter>();

            services.AddTransient<ISystemClock, Clock>();
            services.AddSingleton(Log.Logger);

            registerOverrides?.Invoke(services);
        }
    }
}
