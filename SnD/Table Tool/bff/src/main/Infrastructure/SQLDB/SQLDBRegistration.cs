namespace Infrastructure.SQLDB
{
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.SQLDB.Repositories;    

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public static class SQLDBRegistration
    {
        public static IServiceCollection AddSqlDb(this IServiceCollection services,IConfiguration configuration)
        {
            
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnectionString")));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}
