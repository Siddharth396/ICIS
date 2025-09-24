namespace Infrastructure.Services.OAuth
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;

    public static class OAuthRegistration
    {
        public static IServiceCollection RegisterOAuthServices(this IServiceCollection services)
        {
            services.AddOptions<OAuthOptions>()
               .BindConfiguration(OAuthOptions.ConfigurationSectionName)
               .ValidateDataAnnotations()
               .ValidateOnStart();

            services.AddSingleton<OAuthOptions>(
                provider => provider.GetRequiredService<IOptions<OAuthOptions>>().Value);

            services.AddSingleton<IConfidentialClientApplication>(
                provider =>
                {
                    var options = provider.GetRequiredService<OAuthOptions>();

                    var app = ConfidentialClientApplicationBuilder.Create(options.ClientId)
                       .WithClientSecret(options.ClientSecret)
                       .WithOidcAuthority(options.Authority)
                       .Build();

                    return app;
                });
            return services;
        }
    }
}
