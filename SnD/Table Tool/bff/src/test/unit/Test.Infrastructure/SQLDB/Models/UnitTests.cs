namespace Test.Infrastructure.SQLDB.Models
{
    using global::Infrastructure.SQLDB.Models;

    using Xunit;

    public class UnitTests
    {
        [Fact]
        public void EnsureAllPropertiesAreTested()
        {
            //make a list of all attributes in the class
            var expectedProperties = new[]
            {
                nameof(Unit.Id),
                nameof(Unit.Code),
                nameof(Unit.Description)
            };

            // get all properties in the class
            var actualProperties = typeof(Unit).GetProperties().Select(p => p.Name);

            // compare the two lists
            Assert.Equal(expectedProperties, actualProperties);
        }

        [Fact]
        public void AllAttributesPopulated_ChecksIfAttributesAssigned()
        {
            // Arrange
            var unit = new Unit
            {
                Id = 1,
                Code = "Code 1",
                Description = "Description 1"
            };

            // Act & Assert
            Assert.Equal(1, unit.Id);
            Assert.Equal("Code 1", unit.Code);
            Assert.Equal("Description 1", unit.Description);
        }
    }
}
