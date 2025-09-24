namespace Authoring.Tests.Application.Product.Query
{

    using Authoring.Application.Product.Query;

    using BusinessLayer.Services.Models;
    using BusinessLayer.Services.Product;

    using FluentAssertions;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;
    using global::Infrastructure.SQLDB.Repositories;

    using HotChocolate.Resolvers;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using Test.Infrastructure.GraphQL;

    using Xunit;


    [Collection("Authoring stub collection")]
    public class ProductQueriesShould
    {
        private readonly QueryExecutor execute;
        private readonly ProductQueries productQueries;
        private readonly Mock<IErrorReporter> errorReporter;
        private readonly Mock<IResolverContext> resolverContext;
        private readonly Mock<IProductService> productService;
        private readonly Mock<IConfiguration> mockConfiguration;
        private readonly Mock<IGenericRepository<Product>> mockGenericeRepository;

        public ProductQueriesShould(AuthoringStubFixture fixture)
        {
            execute = fixture.Executor;
            errorReporter = new Mock<IErrorReporter>();
            resolverContext = new Mock<IResolverContext>();
            productService = new Mock<IProductService>();
            productQueries = new ProductQueries(errorReporter.Object);
            mockConfiguration = new Mock<IConfiguration>();
            mockGenericeRepository = new Mock<IGenericRepository<Product>>();
        }

        [Fact]
        public async Task GetProducts_Successfully()
        {
            // Arrange
            Product product1 = new Product();
            product1.Id = 1;
            product1.Code = "Styrene";
            product1.Description = "Styrene";
            Product product2 = new Product();
            product2.Id = 2;
            product2.Code = "Melamine";
            product2.Description = "Melamine";
            IEnumerable<Product> prod = new List<Product>() { product1, product2 };

            var enabledProducts = mockConfiguration.Setup(s => s.GetSection("enabledProducts").Value).Returns("Styrene,Melamine");
            mockGenericeRepository.Setup(x => x.GetQueryable()).Returns(prod.AsQueryable());
            productService.Setup(x => x.GetProducts()).ReturnsAsync(Response<IEnumerable<Product>>.Success(prod));
            // Act
            var response = await productQueries.GetProducts(resolverContext.Object,productService.Object);

            // Assert

            Assert.NotNull(response);
            Assert.Equal(prod, response);
            response.Should().BeAssignableTo<IEnumerable<Product>>();

        }

        [Fact]
        public async Task GetProducts_On_Failure_Return_Null()
        {
            // Arrange
            productService.Setup(x => x.GetProducts()).ReturnsAsync(Response<IEnumerable<Product>>.Failure("There is no products in enabled list", "500"));

            // Act
            var response = await productQueries.GetProducts(resolverContext.Object, productService.Object);

            // Assert
            Assert.Null(response);

        }
    }
}
