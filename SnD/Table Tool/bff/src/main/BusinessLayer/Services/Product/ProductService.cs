namespace BusinessLayer.Services.Product
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Models;

    using Infrastructure.SQLDB.Models;
    using Infrastructure.SQLDB.Repositories;

    using Microsoft.Extensions.Configuration;

    using Serilog;

    public class ProductService : IProductService
    {
        IGenericRepository<Product> genericRepository;
        private ILogger logger;
        private IConfiguration configuration;
        public ProductService(IGenericRepository<Product> genericRepository, ILogger logger, IConfiguration configuration)
        {
            this.genericRepository = genericRepository;
            this.logger = logger;
            this.configuration = configuration;
        }
        public async Task<Response<IEnumerable<Product>>> GetProducts()
        {
            var enabledProducts = configuration.GetSection("enabledProducts").Value;
            if (string.IsNullOrWhiteSpace(enabledProducts))
            {
                return Response<IEnumerable<Product>>.Failure("There is no products in enabled list", "500");
            }
            var query = genericRepository.GetQueryable();
            var products = query.Where(x => enabledProducts.Contains(x.Description) && !string.IsNullOrWhiteSpace(x.Description)).ToList();
            return Response<IEnumerable<Product>>.Success(products);
        }
    }
}
