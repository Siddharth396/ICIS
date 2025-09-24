namespace BusinessLayer.Tests.Validation
{
    using BusinessLayer.DTO;
    using BusinessLayer.DTOs;
    using BusinessLayer.Validation;

    using Common.Constants;

    using Infrastructure.SQLDB.Models;

    using Moq;

    using Xunit;
    using Xunit.Sdk;

    public class ValidationServiceTests
    {
        Mock<SaveContentBlockRequest> mockSaveContentBlockRequest;
        Mock<ContentBlockRequest> mockContentBlockRequest;
        ContentBlockValidationService contentBlockValidationService;
        public ValidationServiceTests()
        {
            mockSaveContentBlockRequest = new Mock<SaveContentBlockRequest>();
            mockContentBlockRequest = new Mock<ContentBlockRequest>();
            contentBlockValidationService = new ContentBlockValidationService();
        }

        [Fact]
        public void Should_ReturnInvalidResponse_When_ValidatingContentBlockInputWithInvalidInput()
        {
            // Arrange
            var StatusCode = "400";
            var errorMessage = "Incorrect request parameter. Please provide all required parameters.";
            var contentBlockInput = new SaveContentBlockRequest
            {
                ContentBlockId = " ",
                Filter = "{\"key\":\"value\"}"
            };

            // Act
            var response = contentBlockValidationService.ValidateContentBlockInput(contentBlockInput);


            // Assert
            Assert.False(response.Status);
            Assert.Equal(StatusCode, response.StatusCode);
            Assert.Equal(errorMessage, response.ValidationMessage);

        }




        [Fact]
        public void Should_ReturnValidResponse_When_ValidatingContentBlockInputWithValidInput()
        {
            // Arrange
            var StatusCode = string.Empty;
            var message = string.Empty;
            var contentBlockInput = new SaveContentBlockRequest
            {
                ContentBlockId = "123",
                Filter = "{\"region\": \"Australia\", \"product\": \"Benzene\", \"tableType\": \"Outage\"}"
            };

            //Act
            var response = contentBlockValidationService.ValidateContentBlockInput(contentBlockInput);


            // Assert
            Assert.True(response.Status);
            Assert.Equal(StatusCode, response.StatusCode);
            Assert.Equal(message, response.ValidationMessage);

        }


        [Fact]
        public void Should_ReturnInvalidResponse_When_ValidatingContentBlockRequestWithInvalidRequest()
        {
            // Arrange
            var StatusCode = "400";
            var errorMessage = "Incorrect request parameter. Please provide all required parameters.";
            var contentBlockInput = new ContentBlockRequest
            {
                ContentBlockId = string.Empty,
                Version = string.Empty
            };

            // Act
            var response = contentBlockValidationService.ValidateContentBlockRequest(contentBlockInput);


            // Assert
            Assert.False(response.Status);
            Assert.Equal(StatusCode, response.StatusCode);
            Assert.Equal(errorMessage, response.ValidationMessage);

        }


        [Fact]
        public void Should_ReturnValidResponse_When_ValidatingCContentBlockRequestWithValidRequest()
        {
            // Arrange
            var StatusCode = string.Empty;
            var message = string.Empty;
            var contentBlockInput = new ContentBlockRequest
            {
                ContentBlockId = "123",
                Version = "2.1"
            };

            //Act
            var response = contentBlockValidationService.ValidateContentBlockRequest(contentBlockInput);


            // Assert
            Assert.True(response.Status);
            Assert.Equal(StatusCode, response.StatusCode);
            Assert.Equal(message, response.ValidationMessage);

        }

    }
}


