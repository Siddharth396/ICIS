namespace BusinessLayer.Tests.Services.Product
{
    using BusinessLayer.Services.Product;

    using Serilog;

    using Infrastructure.SQLDB.Models;
    using Infrastructure.SQLDB.Repositories;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using Xunit;

    public class ProductServiceTests
    {
        Mock<IGenericRepository<Product>> mockGenericeRepository;
        Mock<ILogger> mockLogger;
        Mock<IConfiguration> mockConfiguration;
        ProductService productService;
        public ProductServiceTests()
        {
            mockGenericeRepository = new Mock<IGenericRepository<Product>>();
            mockLogger = new Mock<ILogger>();
            mockConfiguration = new Mock<IConfiguration>();
            productService = new ProductService(mockGenericeRepository.Object, mockLogger.Object, mockConfiguration.Object);
        }
        [Theory]
        [InlineData("Styrene")]
        [InlineData("Styrene,Melamine")]
        public async Task GetProducts_Should_Get_Success_When_EnabledProduct_Have_Value(string product)
        {
            //Arrange
            Product product1=new Product();
            product1.Id = 1;
            product1.Code = "Styrene";
            product1.Description = "Styrene";
            Product product2 = new Product();
            product2.Id = 2;
            product2.Code = "Melamine";
            product2.Description = "Melamine";
            List<Product> prod = new List<Product>() { product1,product2 };

            var enabledProducts = mockConfiguration.Setup(s => s.GetSection("enabledProducts").Value).Returns(product);
            mockGenericeRepository.Setup(x =>x.GetQueryable()).Returns(prod.AsQueryable());
            //Act
            var actualResponse = await productService.GetProducts();

            //Assert
            Assert.True(actualResponse.IsSuccess);
            Assert.NotNull(actualResponse.Data);
            Assert.Equal(product.Split(',').Length,actualResponse.Data.Count());
            Assert.IsAssignableFrom<IEnumerable<Product>>(actualResponse.Data);
        }
        [Fact]
        public async Task GetProducts_Should_Get_Fail_When_EnabledProduct_Not_Have_Value()
        {
            //Arrange
            var enabledProducts = mockConfiguration.Setup(s => s.GetSection("enabledProducts").Value).Returns("");
            //Act
            var actualResponse = await productService.GetProducts();

            //Assert
            Assert.True(actualResponse.IsFailure);
            Assert.Null(actualResponse.Data);
            Assert.NotNull(actualResponse.Error);
            Assert.Equal("500", actualResponse.Error.Code);
            Assert.Equal("There is no products in enabled list", actualResponse.Error.Message);

        }
    }
}
