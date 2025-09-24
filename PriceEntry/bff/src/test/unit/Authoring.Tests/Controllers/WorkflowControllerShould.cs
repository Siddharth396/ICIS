namespace Authoring.Tests.Controllers
{
    using System.Threading.Tasks;

    using Authoring.Api.Controllers;

    using Camunda.Worker.Variables;

    using global::BusinessLayer.DataPackage.Models;
    using global::BusinessLayer.DataPackage.Services;

    using global::Infrastructure.Services.Workflow;

    using Icis.Workflow.Model;

    using NSubstitute;

    using Serilog;

    using Snapshooter.Xunit;

    using Xunit;

    public class WorkflowControllerShould
    {
        private readonly IDataPackageService dataPackageService;

        private readonly WorkflowController workflowController;

        public WorkflowControllerShould()
        {
            dataPackageService = Substitute.For<IDataPackageService>();
            var logger = Substitute.For<ILogger>();
            workflowController = new WorkflowController(dataPackageService, logger);
        }

        [Fact]
        public async Task Return_Failure_With_Error_For_DataPackageStatusUpdated_When_Status_Update_Fails()
        {
            // Arrange
            var input = new APIEventPayload { BusinessKey = "12345" };
            AddRequiredInputVariables(input);

            dataPackageService
               .OnDataPackageTransitionedToState(Arg.Any<WorkflowBusinessKey>(), Arg.Any<WorkflowStatus>(), Arg.Any<string>())
               .Returns(DataPackageStatusChangeResult.InvalidStatus);

            // Act
            var result = await workflowController.DataPackageStatusUpdated(input);

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task
            Return_Invalid_Status_For_PublishDataPackage_When_Input_Data_Package_Is_Not_In_Correct_Status()
        {
            // Arrange
            var input = new APIEventPayload { BusinessKey = "12345" };
            AddRequiredInputVariables(input);
            dataPackageService
               .OnDataPackagePublished(Arg.Any<WorkflowBusinessKey>(), Arg.Any<string>(), Arg.Any<string>())
               .Returns(DataPackageStatusChangeResult.InvalidStatus);

            // Act
            var result = await workflowController.DataPackagePublished(input);

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Not_Found_For_PublishDataPackage_When_Input_Data_Package_Is_Not_Found()
        {
            // Arrange
            var input = new APIEventPayload { BusinessKey = "12345" };
            AddRequiredInputVariables(input);
            dataPackageService
               .OnDataPackagePublished(Arg.Any<WorkflowBusinessKey>(), Arg.Any<string>(), Arg.Any<string>())
               .Returns(DataPackageStatusChangeResult.NotFound);

            // Act
            var result = await workflowController.DataPackagePublished(input);

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task
            Return_Status_Not_Found_For_DataPackageStatusUpdated_When_Status_Variable_Not_Found_In_Payload()
        {
            // Arrange
            var input = new APIEventPayload { BusinessKey = "12345" };
            AddRequiredInputVariables(input);
            input.Variables.Remove("status");

            // Act
            var result = await workflowController.DataPackageStatusUpdated(input);

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Success_For_PublishDataPackage_When_Data_Package_Is_Published_With_Success()
        {
            // Arrange
            var input = new APIEventPayload { BusinessKey = "12345" };
            AddRequiredInputVariables(input);

            dataPackageService
               .OnDataPackagePublished(Arg.Any<WorkflowBusinessKey>(), Arg.Any<string>(), Arg.Any<string>())
               .Returns(DataPackageStatusChangeResult.Success);

            // Act
            var result = await workflowController.DataPackagePublished(input);

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task
         Return_Status_Not_Found_For_DataPackagePublished_When_Status_Variable_Not_Found_In_Payload()
        {
            // Arrange
            var input = new APIEventPayload { BusinessKey = "12345" };
            AddRequiredInputVariables(input);
            input.Variables.Remove("status");

            // Act
            var result = await workflowController.DataPackagePublished(input);

            // Assert
            result.MatchSnapshot();
        }

        private static void AddRequiredInputVariables(APIEventPayload input)
        {
            input.Variables.Add("ActionPerformedByUser", new StringVariable("test_user"));
            input.Variables.Add("status", new StringVariable("some_status"));
        }
    }
}
