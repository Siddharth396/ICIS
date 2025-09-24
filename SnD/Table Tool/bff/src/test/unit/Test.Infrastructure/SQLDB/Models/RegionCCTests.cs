namespace Test.Infrastructure.SQLDB.Models
{
    using global::Infrastructure.SQLDB.Models;

    using Xunit;

    public class RegionCCTests
    {
        [Fact]
        public void EnsureAllPropertiesAreTested()
        {
            //make a list of all attributes in the class
            var expectedProperties = new[]
            {
                nameof(RegionCC.Id),
                nameof(RegionCC.Code),
                nameof(RegionCC.Description)
            };

            // get all properties in the class
            var actualProperties = typeof(RegionCC).GetProperties().Select(p => p.Name);

            // compare the two lists
            Assert.Equal(expectedProperties, actualProperties);
        }

        [Fact]
        public void AllAttributesPopulated_ChecksIfAttributesAssigned()
        {
            // Arrange
            var regionCC = new RegionCC
            {
                Id = 1,
                Code = "Code 1",
                Description = "Description 1"
            };

            //Act & Assert
            Assert.Equal(1, regionCC.Id);
            Assert.Equal("Code 1", regionCC.Code);
            Assert.Equal("Description 1", regionCC.Description);
        }
    }
}
