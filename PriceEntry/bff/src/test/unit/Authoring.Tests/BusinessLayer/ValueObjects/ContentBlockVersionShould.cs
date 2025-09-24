namespace Authoring.Tests.BusinessLayer.ValueObjects
{
    using System;

    using FluentAssertions;

    using Xunit;

    using Version = global::BusinessLayer.ContentBlock.ValueObjects.Version;

    public class ContentBlockVersionShould
    {
        [Fact]
        public void Throw_ArgumentOutOfRange_Exception_For_Negative_Values()
        {
            // Arrange
            var value = -1;

            // Act
            var action = new Action(() => new Version(value));

            // Assert
            action.Should()
               .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Increment_To_Next_Version()
        {
            // Arrange
            var version = new Version(5);

            // Act
            var incrementedVersion = version.Increment();

            // Assert
            incrementedVersion.Value.Should().Be(version.Value + 1);
        }
    }
}
