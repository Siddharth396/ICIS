namespace Authoring.Infrastructure
{
    using System.Diagnostics.CodeAnalysis;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using global::Infrastructure.Extensions;
    using global::Infrastructure.MongoDB.Serializers;

    using Microsoft.AspNetCore.Builder;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Conventions;

    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            // Register this as early as possible
            BsonSerializer.RegisterDiscriminatorConvention(
                typeof(BasePriceItem),
                new ScalarDiscriminatorConvention("series_item_type_code"));
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalToBsonConverter());

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
