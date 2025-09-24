namespace Test.Infrastructure.SQLDB.Models
{
    using global::Infrastructure.SQLDB.Models;

    using Xunit;

    public class OutageTests
    {
        [Fact]
        public void EnsureAllPropertiesAreTested()
        {
            //make a list of all attributes in the class
            var expectedProperties = new[]
            {
                nameof(Outage.OutageStart),
                nameof(Outage.OutageEnd),
                nameof(Outage.Country),
                nameof(Outage.Company),
                nameof(Outage.Site),
                nameof(Outage.PlantNo),
                nameof(Outage.Cause),
                nameof(Outage.CapacityLoss),
                nameof(Outage.TotalAnnualCapacity),
                nameof(Outage.LastUpdated),
                nameof(Outage.Comments)
            };

            // get all properties in the class
            var actualProperties = typeof(Outage).GetProperties().Select(p => p.Name);

            // compare the two lists
            Assert.Equal(expectedProperties, actualProperties);
        }

        [Fact]
        public void AllAttributesPopulated_ChecksIfAttributesAssigned()
        {
            // Arrange
            var outage = new Outage
            {
                OutageStart = "25 Aug 2024 (Market information)",
                OutageEnd = "12 Nov 2024 (Market information)",
                Country = "Country 1",
                Company = "Company 1",
                Site = "Site 1",
                PlantNo = 1,
                Cause = "Scheduled",
                CapacityLoss = "100% (est. 48.2kt)",
                TotalAnnualCapacity = 220.00m,
                LastUpdated = "25 Jan 2024",
                Comments = "Comments 1"
            };

            //Act & Assert
            Assert.Equal("25 Aug 2024 (Market information)", outage.OutageStart);
            Assert.Equal("12 Nov 2024 (Market information)", outage.OutageEnd);
            Assert.Equal("Country 1", outage.Country);
            Assert.Equal("Company 1", outage.Company);
            Assert.Equal("Site 1", outage.Site);
            Assert.Equal(1, outage.PlantNo);
            Assert.Equal("Scheduled", outage.Cause);
            Assert.Equal("100% (est. 48.2kt)", outage.CapacityLoss);
            Assert.Equal(220.00m, outage.TotalAnnualCapacity);
            Assert.Equal("25 Jan 2024", outage.LastUpdated);
            Assert.Equal("Comments 1", outage.Comments);
        }
    }
}
