namespace Subscriber.Tests
{
    using FluentAssertions;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    using Subscriber.Infrastructure;

    using Xunit;

    public class ServiceCollectionShould
    {
        [Fact]
        public void Have_All_Types_Registered_With_The_Correct_Scope()
        {
            Action act = () => new Program("subscriber.appsettings.json", "subscriber.env")
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
