namespace Subscriber.Infrastructure
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text;

    using BusinessLayer.PriceDisplay.Registration;

    using dotenv.net;

    using global::Infrastructure;
    using global::Infrastructure.Configuration;
    using global::Infrastructure.Extensions;
    using global::Infrastructure.GraphQL;
    using global::Infrastructure.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Serilog.Formatting.Json;

    using Subscriber.Infrastructure.Services;

    [ExcludeFromCodeCoverage]
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(builder.Configuration)
               .WriteTo.Console(new JsonFormatter(renderMessage: true))
               .Enrich.FromLogContext()
               .CreateLogger();

            return builder;
        }

        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();
            builder.WebHost.UseUrls(builder.Configuration.GetValue<string>("Hostings:Urls"));

            var assembly = Assembly.GetExecutingAssembly();
            builder.Services.AddSingleton(builder.Configuration);
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.RegisterInfrastructureForSubscriber();
            builder.Services.RegisterServicesForDisplay();
            builder.Services.RegisterRepositoriesForDisplay();
            builder.Services.AddGraphQL(
                assembly,
                builder.Configuration.IsDevMode(),
                builder.Configuration.IsMutationEnabled(),
                builder.Configuration.IsTraceEnabled());
            builder.Services.ConfigureServices(builder.Configuration);
            return builder;
        }

        public static WebApplicationBuilder LoadConfigurations(this WebApplicationBuilder builder)
        {
            DotEnv.Load(new DotEnvOptions(true, new[] { ".env" }, Encoding.UTF8));

            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.BuildAndReplacePlaceholders();
            return builder;
        }
    }
}
