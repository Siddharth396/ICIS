namespace Test.Infrastructure.SQLDB.Models
{
    using global::Infrastructure.SQLDB.Models;

    using Xunit;

    public class ProductTests
    {
        [Fact]
        public void EnsureAllPropertiesAreTested()
        {
            //make a list of all attributes in the class
            var expectedProperties = new[]
            {
                nameof(Product.Id),
                nameof(Product.Code),
                nameof(Product.Description)
            };

            // get all properties in the class
            var actualProperties = typeof(Product).GetProperties().Select(p => p.Name);

            // compare the two lists
            Assert.Equal(expectedProperties, actualProperties);
        }

        [Fact]
        public void AllAttributesPopulated_ChecksIfAttributesAssigned()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Code = "Code 1",
                Description = "Description 1"
            };

            // Act & Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Code 1", product.Code);
            Assert.Equal("Description 1", product.Description);
        }
    }
}
