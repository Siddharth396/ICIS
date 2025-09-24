namespace Authoring.Tests
{
    using Authoring.Infrastructure;

    using FluentAssertions;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    using Xunit;

    public class ServiceCollectionShould
    {
        [Fact]
        public void Have_All_Types_Registered_With_The_Correct_Scope()
        {
            Action act = () => new Program("authoring.appsettings.json", "authoring.env")
               .CreateHostBuilder(new string[] { })
               .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseDefaultServiceProvider(
                                (_, options) =>
                                {
                                    options.ValidateOnBuild = true;
                                    options.ValidateScopes = true;
                                })
                           .UseStartup<Startup>();
                    })
               .Build();

            act.Should().NotThrow("dependencies should be registered correctly");
        }
    }
}
