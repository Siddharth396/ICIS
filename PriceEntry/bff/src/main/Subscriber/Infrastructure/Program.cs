namespace Subscriber.Infrastructure
{
    using System.Diagnostics.CodeAnalysis;

    using global::Infrastructure.Extensions;

    using Microsoft.AspNetCore.Builder;

    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplication.CreateBuilder(args)
               .LoadConfigurations()
               .AddSerilog()
               .ConfigureServices()
               .Build()
               .ConfigureApplication()
               .RunApplication();
        }
    }
}
