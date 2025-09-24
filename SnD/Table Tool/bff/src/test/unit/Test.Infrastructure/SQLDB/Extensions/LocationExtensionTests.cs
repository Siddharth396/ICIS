namespace Test.Infrastructure.SQLDB.Extensions
{
    using global::Infrastructure.SQLDB.Extensions;

    using Xunit;

    public class LocationExtensionTests
    {
        [Theory]
        [InlineData("TAIWAN, PROVINCE OF CHINA", "Taiwan")]
        [InlineData("HONG KONG SPECIAL ADMINISTRATIVE REGION", "Hong Kong")]
        [InlineData("INDIA", "INDIA")]
        public void TransformCountry_ShouldReturnCorrectCountry(string country, string expectedCountry)
        {
            // Act
            var result = country.TransformCountry();

            // Assert
            Assert.Equal(expectedCountry, result);
        }

        [Fact]
        public void TransformCountry_ShouldReturnEmptyString()
        {
            // Act
            var result = string.Empty.TransformCountry();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void TransformCountry_ShouldReturnNull()
        {
            // Arrange
            var input = null as string;

            // Act
            var result = input.TransformCountry();

            // Assert
            Assert.Null(result);
        }
    }
}
