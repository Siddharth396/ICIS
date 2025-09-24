namespace BusinessLayer.Tests.Services.Common
{
    using BusinessLayer.Services.Common;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using Xunit;

    public class SqlQueryReaderTests
    {
        [Fact]
        public void ReadQuery_FileExists_ReturnsQueryText()
        {
            // Arrange
            var scriptName = "TableTool1.sql";
            var expectedQueryText = "SELECT * FROM Outages";
            var scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns(scriptsPath);
            mockConfiguration.Setup(x => x.GetSection("scriptsPath")).Returns(mockSection.Object);

            var sqlQueryReader = new SqlQueryReader(mockConfiguration.Object);

            Directory.CreateDirectory(scriptsPath);
            var filePath = Path.Combine(scriptsPath, scriptName);
            File.WriteAllText(filePath, expectedQueryText);

            // Act
            var queryText = sqlQueryReader.ReadQuery(scriptName);

            // Assert
            Assert.Equal(expectedQueryText, queryText);

            // Cleanup
            File.Delete(filePath);
            Directory.Delete(scriptsPath);
        }

        [Fact]
        public void ReadQuery_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            var scriptName = "NonExistentScript.sql";
            var scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns(scriptsPath);
            mockConfiguration.Setup(x => x.GetSection("scriptsPath")).Returns(mockSection.Object);

            var sqlQueryReader = new SqlQueryReader(mockConfiguration.Object);

            // Act & Assert
            var exception = Assert.Throws<FileNotFoundException>(() => sqlQueryReader.ReadQuery(scriptName));
            Assert.Equal($"Script file '{scriptName}' not found at path '{Path.Combine(scriptsPath, scriptName)}'.", exception.Message);
        }
    }
}
