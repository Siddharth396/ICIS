namespace Authoring.Infrastructure
{
    using System.Reflection;
    using System.Text;
    using System.Text.Json;

    using Authoring.Infrastructure.Services;

    using BusinessLayer.PriceDisplay.Registration;
    using BusinessLayer.PriceEntry.AutoMapper;
    using BusinessLayer.Registration;

    using dotenv.net;

    using global::Infrastructure;
    using global::Infrastructure.Configuration;
    using global::Infrastructure.Extensions;
    using global::Infrastructure.GraphQL;
    using global::Infrastructure.Services;

    using Icis.Workflow.Extensions;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Serilog;
    using Serilog.Formatting.Json;

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
            builder.Services.RegisterInfrastructure();
            builder.Services.RegisterServicesForDisplay();
            builder.Services.RegisterServices();
            builder.Services.RegisterRepositoriesForDisplay();
            builder.Services.RegisterRepositories();
            builder.Services.RegisterMappers();
            builder.Services.AddGraphQL(
                assembly,
                builder.Configuration.IsDevMode(),
                builder.Configuration.IsMutationEnabled(),
                builder.Configuration.IsTraceEnabled());
            builder.Services.AddAutoMapper(typeof(PriceEntryMappingProfile));
            builder.Services.ConfigureServices(builder.Configuration, HealthChecks.RegisterHealthChecks);
            builder.Services.AddControllers()
               .AddJsonOptions(options => { options.JsonSerializerOptions.SetJsonSerializerOptions(); });
            return builder;
        }

        public static WebApplicationBuilder LoadConfigurations(this WebApplicationBuilder builder)
        {
            DotEnv.Load(new DotEnvOptions(true, new[] { ".env" }, Encoding.UTF8));

            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.BuildAndReplacePlaceholders();
            return builder;
        }

        /// <summary>
        /// The GetJsonSerializerOptions method is implemented in Icis.Workflow nuget package,
        /// and it is actually changing things on the input, and also returning it.
        /// This method has been added because we can't rename methods in nuget packages.
        /// </summary>
        public static void SetJsonSerializerOptions(this JsonSerializerOptions options)
        {
            options.GetJsonSerializerOptions();
        }
    }
}
