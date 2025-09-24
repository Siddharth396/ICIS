namespace Authoring.Tests.Infrastructure.Services
{
    using System;

    using FluentAssertions;

    using global::Infrastructure.Services.Workflow;

    using Xunit;

    public class ReviewPageUrlShould
    {
        [Theory]
        [InlineData("/valid/path")]
        [InlineData("valid/path")]
        public void Be_Created_When_Path_Is_Valid(string path)
        {
            // Act
            var result = ReviewPageUrl.Create(path);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be(path);
        }

        [Fact]
        public void Throw_Argument_Exception_When_Path_Is_Empty()
        {
            // Act
            Action act = () => ReviewPageUrl.Create(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Path cannot be null or empty.*");
        }

        [Fact]
        public void Throw_Argument_Exception_When_Path_Is_Not_Relative()
        {
            // Arrange
            var path = "http://absolute/path";

            // Act
            Action act = () => ReviewPageUrl.Create(path);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Path must be a relative path.*");
        }

        [Fact]
        public void Throw_Argument_Exception_When_Path_Is_Null()
        {
            // Act
            Action act = () => ReviewPageUrl.Create(null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Path cannot be null or empty.*");
        }

        [Fact]
        public void Have_An_Empty_Field()
        {
            // Act
            var result = ReviewPageUrl.Empty;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be(string.Empty);
        }
    }
}
