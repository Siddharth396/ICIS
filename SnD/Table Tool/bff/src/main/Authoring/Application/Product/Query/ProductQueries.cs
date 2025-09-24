namespace Authoring.Application.Product.Query
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Product;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;

    using HotChocolate;
    using HotChocolate.Data;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType("Query")]
    public class ProductQueries
    {
        private readonly IErrorReporter errorReporter;

        public ProductQueries(IErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Product>> GetProducts(IResolverContext resolverContext, [Service] IProductService service)
        {
            var productData = await service.GetProducts();
            if (productData.IsFailure)
            {
                errorReporter.ReportCustomError(
                resolverContext,
                productData.Error.Code,
                productData.Error.Message);
            }

            return productData.Data!;
        }
    }
}