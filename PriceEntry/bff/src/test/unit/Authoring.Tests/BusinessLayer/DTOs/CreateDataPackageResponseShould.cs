namespace Authoring.Tests.BusinessLayer.DTOs
{
    using FluentAssertions;

    using global::BusinessLayer.DataPackage.DTOs.Output;

    using Xunit;

    public class CreateDataPackageResponseShould
    {
        [Fact]
        public void Set_IsSuccess_To_True_By_Default()
        {
            // Arrange
            var response = new CreateDataPackageResponse();

            // Assert
            response.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Initialize_ContentBlocksInError_By_Default()
        {
            // Arrange
            var response = new CreateDataPackageResponse();

            // Assert
            response.ContentBlocksInError.Should().NotBeNull();
            response.ContentBlocksInError.Should().BeEmpty();
        }

        [Fact]
        public void Set_IsSuccess_To_False_When_Error_Is_Added()
        {
            // Arrange
            var response = new CreateDataPackageResponse();

            // Act
            response.AddContentBlockError("block1", "Error message");

            // Assert
            response.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Store_Error_When_Added()
        {
            // Arrange
            var response = new CreateDataPackageResponse();

            // Act
            response.AddContentBlockError("block1", "Error message");

            // Assert
            response.ContentBlocksInError.Should().HaveCount(1);
            response.ContentBlocksInError[0].ContentBlockId.Should().Be("block1");
            response.ContentBlocksInError[0].Error.Should().Be("Error message");
        }

        [Fact]
        public void Not_Modify_Values_When_Merge_WithNull_()
        {
            // Arrange
            var response = new CreateDataPackageResponse();
            var other = (CreateDataPackageResponse)null;

            // Act
            response.Merge(other);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.ContentBlocksInError.Should().BeEmpty();
        }

        [Fact]
        public void Should_Merge_Properties_With_Another_Instance()
        {
            // Arrange
            var response = new CreateDataPackageResponse();
            var other = new CreateDataPackageResponse();
            other.AddContentBlockError("block1", "Error message");

            // Act
            response.Merge(other);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ContentBlocksInError.Should().HaveCount(1);
            response.ContentBlocksInError[0].ContentBlockId.Should().Be("block1");
            response.ContentBlocksInError[0].Error.Should().Be("Error message");
        }
    }
}
