namespace Authoring.Tests.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Infrastructure.Services.Workflow;
    using global::Infrastructure.Services.Workflow.Dtos;

    using Microsoft.Extensions.Internal;

    using NSubstitute;

    using Serilog;

    using Subscriber.Auth;

    using Test.Infrastructure.Mocks;
    using Test.Infrastructure.Stubs;

    using Xunit;

    public class DataPackageWorkflowServiceShould
    {
        private readonly ISystemClock clock = new TestClock();

        private readonly IUserContext userContext = new TestUserContext();

        private readonly ILogger logger = Substitute.For<ILogger>();

        private readonly WorkflowSettings workflowSettings = new()
        {
            BaseUrl = "http://localhost",
            StartWorkflowEndpoint = "/start",
            StateTransitionEndpoint = "/complete",
            NextActionsEndpoint = "/next-actions/{businessKey}",
            Timeout = new TimeSpan(0, 0, 0),
            MaxRetries = 1,
            WorkspaceId = "pricing",
            ProcessDefinitionKey = "pricing",
            VersionEndpoint = "/version",
            WorkflowCorrectionToggle = true,
            ShowRepublishButtonToggle = true,
            Versions = new WorkflowVersionSettings
            {
                Default = "advance",
                Overrides = new Dictionary<string, string>
                {
                    { "LNG", "simple" },
                    { "MELAMINE", "advance" },
                    { "STYRENE", "simple" }
                }
            }
        };

        [Fact]
        public async Task Return_False_When_PerformAction_With_Invalid_BusinessKey()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.PerformAction(businessKey, null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Return_True_When_PerformAction_With_Valid_BusinessKey()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");

            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.PerformAction(businessKey, "SOME_ACTION");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Return_None_WorkflowId_When_Starting_The_Workflow_With_Invalid_BusinessKey()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");

            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.StartWorkflow(
                             businessKey,
                             new WorkflowVersion("advance"),
                             new Dictionary<string, string> { ["Var1"] = "Value" },
                             OperationType.None,
                             DateTime.UtcNow,
                             ReviewPageUrl.Create("/dummy/path"));

            // Assert
            result.Should().Be(WorkflowId.None);
        }

        [Fact]
        public async Task StartWorkflow_WithValidBusinessKey_ReturnsWorkflowId()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");

            var expectedWorkflowId = new WorkflowId("456");

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Id\":\"456\"}")
            };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.StartWorkflow(
                businessKey,
                new WorkflowVersion(string.Empty),
                new Dictionary<string, string>(),
                OperationType.None,
                DateTime.UtcNow,
                ReviewPageUrl.Create("/dummy/path"));

            // Assert
            result.Should().Be(expectedWorkflowId);
        }

        [Theory]
        [InlineData("Default_Commodity", "advance")]
        [InlineData("LNG", "simple")]
        [InlineData("Melamine", "advance")]
        [InlineData("Styrene", "simple")]
        public void Return_WorkflowVersion_When_Getting_WorkflowVersion(string commodity, string expected)
        {
            // Arrange
            var workflowService = new DataPackageWorkflowService(
                Substitute.For<HttpClient>(),
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = workflowService.GetWorkflowVersion(commodity);

            // Assert
            result.Value.Should().Be(expected);
        }

        [Fact]
        public async Task Return_EmptyList_When_GetNextActions_Returns_NoContent()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");

            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.GetNextActions(businessKey);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Return_EmptyList_When_GetNextActions_Returns_UnsuccessfulResponse()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");

            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.GetNextActions(businessKey);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Return_ListOfActions_When_GetNextActions_Returns_SuccessfulResponse()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");

            var expectedActions = new List<WorkflowAction>
            {
                new() { Key = "Action1", Value = "Value1" }, new() { Key = "Action2", Value = "Value2" }
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new NextActionsResponse { NextActions = expectedActions }))
            };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.GetNextActions(businessKey);

            // Assert
            result.Should().BeEquivalentTo(expectedActions);
        }

        [Fact]
        public async Task StartWorkflow_WithValidBusinessKey_For_Advanced_Correction_ReturnsWorkflowId()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");

            var expectedWorkflowId = new WorkflowId("456");

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Id\":\"456\"}")
            };

            var httpClient = HttpClientMock.GetHttpClientWithResponse(workflowSettings.BaseUrl, httpResponse);

            var workflowService = new DataPackageWorkflowService(
                httpClient,
                workflowSettings,
                userContext,
                logger);

            // Act
            var result = await workflowService.StartWorkflow(
                businessKey,
                new WorkflowVersion("advance"),
                new Dictionary<string, string>(),
                OperationType.Correction,
                DateTime.UtcNow,
                ReviewPageUrl.Create("/dummy/path"));

            // Assert
            result.Should().Be(expectedWorkflowId);
        }

        [Fact]
        public async Task Throw_Exception_When_Starting_Workflow_With_Empty_ReviewPageUrl()
        {
            // Arrange
            var businessKey = new WorkflowBusinessKey("123");
            var workflowService = new DataPackageWorkflowService(
                Substitute.For<HttpClient>(),
                workflowSettings,
                userContext,
                logger);

            // Act
            Func<Task> act = async () => await workflowService.StartWorkflow(
                businessKey,
                new WorkflowVersion("advance"),
                new Dictionary<string, string>(),
                OperationType.None,
                DateTime.UtcNow,
                ReviewPageUrl.Empty);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Review page url is required to start a workflow.");
        }
    }
}
