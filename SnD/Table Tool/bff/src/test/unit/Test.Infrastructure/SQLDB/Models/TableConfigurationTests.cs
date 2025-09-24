namespace Test.Infrastructure.SQLDB.Models
{
    using global::Infrastructure.SQLDB.Models;

    using Xunit;

    public class TableConfigurationTests
    {
        [Fact]

        public void Should_Ensure_All_Properties_Are_Tested()
        {
            var expectedProperties = new[]
           {
                nameof(TableConfiguration.Id),
                nameof(TableConfiguration.ContentBlockId),
                nameof(TableConfiguration.MinorVersion),
                nameof(TableConfiguration.MajorVersion),
                nameof(TableConfiguration.Filter),
                nameof(TableConfiguration.CreatedOn),
            };

            // get all properties in the class
            var actualProperties = typeof(TableConfiguration).GetProperties().Select(p => p.Name);

            Assert.Equal(expectedProperties, actualProperties);
        }



        [Fact]
        public void Should_Ensure_All_Attributes_Are_Populated()
        {
            // Arrange
            var createdOn = new DateTime(2015, 12, 25);
            var filters = "{\"region\": \"Asia Minor\", \"product\": \"Styrene\", \"TableType\": \"Outage\"}";
            var contentBlockId = "2232-999";
            var tableTool = new TableConfiguration
            {
                Id = 1,
                ContentBlockId = "2232-999",
                MajorVersion = 0,
                MinorVersion = 1,
                Filter = filters,
                CreatedOn = createdOn
            };

            //Act & Assert
            Assert.Equal(1, tableTool.Id);
            Assert.Equal(contentBlockId, tableTool.ContentBlockId);
            Assert.Equal(0, tableTool.MajorVersion);
            Assert.Equal(1, tableTool.MinorVersion);
            Assert.Equal(filters, tableTool.Filter);
            Assert.Equal(createdOn, tableTool.CreatedOn);
        }
    }
}
