namespace Test.Infrastructure.SQLDB.Models
{
    using global::Infrastructure.SQLDB.Models;

    using Xunit;

    public class CapacityDevleopmentTests
    {
        [Fact]
        public void EnsureAllPropertiesAreTested()
        {
            //make a list of all attributes in the class
            var expectedProperties = new[]
            {
                nameof(CapacityDevelopment.Country),
                nameof(CapacityDevelopment.Company),
                nameof(CapacityDevelopment.Site),
                nameof(CapacityDevelopment.PlantNo),
                nameof(CapacityDevelopment.Type),
                nameof(CapacityDevelopment.EstimatedStart),
                nameof(CapacityDevelopment.NewAnnualCapacity),
                nameof(CapacityDevelopment.CapacityChange),
                nameof(CapacityDevelopment.PercentChange),
                nameof(CapacityDevelopment.LastUpdated)
            };

            // get all properties in the class
            var actualProperties = typeof(CapacityDevelopment).GetProperties().Select(p => p.Name);

            Assert.Equal(expectedProperties, actualProperties);
        }

        [Fact]
        public void AllAttributesPopulated_ChecksIfAttributesAssigned()
        {
            // Arrange
            var capacityDevelopment = new CapacityDevelopment
            {
                Country = "Country 1",
                Company = "Company 1",
                Site = "Site 1",
                PlantNo = 1,
                Type = "Type 1",
                EstimatedStart = "25 Aug 2024 (Market information)",
                NewAnnualCapacity = 220,
                CapacityChange = 100,
                PercentChange = "45.5",
                LastUpdated = "25 Jan 2024"
            };

            //Act & Assert
            Assert.Equal("Country 1", capacityDevelopment.Country);
            Assert.Equal("Company 1", capacityDevelopment.Company);
            Assert.Equal("Site 1", capacityDevelopment.Site);
            Assert.Equal(1, capacityDevelopment.PlantNo);
            Assert.Equal("Type 1", capacityDevelopment.Type);
            Assert.Equal("25 Aug 2024 (Market information)", capacityDevelopment.EstimatedStart);
            Assert.Equal(220, capacityDevelopment.NewAnnualCapacity);
            Assert.Equal(100, capacityDevelopment.CapacityChange);
            Assert.Equal("45.5", capacityDevelopment.PercentChange);
            Assert.Equal("25 Jan 2024", capacityDevelopment.LastUpdated);            
        }
    }
}
