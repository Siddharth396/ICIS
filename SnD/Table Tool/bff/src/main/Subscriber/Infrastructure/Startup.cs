namespace Subscriber.Infrastructure
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using BusinessLayer.Repositories;
    using BusinessLayer.Services;

    using global::Infrastructure.Configuration;
    using global::Infrastructure.Extensions;
    using global::Infrastructure.GraphQL;
    using global::Infrastructure.Services;

    using global::Infrastructure.SQLDB;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Subscriber.Infrastructure.Services;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration configuration;

        [ActivatorUtilitiesConstructor]
        public Startup(IConfiguration configuration)
            : this(configuration, null)
        {
        }

        public Startup(IConfiguration configuration, Action<IServiceCollection>? registerOverrides)
        {
            this.configuration = configuration.ReplacePlaceholders();
            RegisterOverrides = registerOverrides;
        }

        // Hack to enable some implementations to be overriden in tests
        public Action<IServiceCollection>? RegisterOverrides { private get; set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.ConfigureCommonMiddilewares(configuration, loggerFactory);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddSingleton(configuration);
            services.AddSqlDb(configuration);
            services.AddSingleton<IAuthService, AuthService>();
            services.RegisterServices();
            services.RegisterRepositories();
            services.AddGraphQL(
                assembly,
                configuration.IsDevMode(),
                configuration.IsMutationEnabled(),
                configuration.IsTraceEnabled());
            services.ConfigureServices(configuration, RegisterOverrides);
        }
    }
}
