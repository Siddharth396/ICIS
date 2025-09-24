namespace Authoring.Tests.Infrastructure.Extensions
{
    using System.ComponentModel.DataAnnotations;

    using FluentAssertions;

    using global::Infrastructure.Extensions;

    using Xunit;

    public class EnumExtensionsTests
    {
        public enum TestEnum
        {
            [Display(Name = "Value 1")]
            Value1,
            [Display(Name = "Value 2")]
            Value2,
            Value3
        }

        [Fact]
        public void GetDisplayName_WithDisplayAttribute_ShouldReturnDisplayName()
        {
            // Arrange
            var enumValue = TestEnum.Value1;

            // Act
            var displayName = enumValue.GetDisplayName();

            // Assert
            displayName.Should().Be("Value 1");
        }

        [Fact]
        public void GetDisplayName_WithoutDisplayAttribute_ShouldReturnEmptyString()
        {
            // Arrange
            var enumValue = TestEnum.Value3;

            // Act
            var displayName = enumValue.GetDisplayName();

            // Assert
            displayName.Should().BeEmpty();
        }
    }
}
