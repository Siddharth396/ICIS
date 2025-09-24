namespace Authoring.Tests.BusinessLayer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::BusinessLayer.Commentary.Services;
    using global::BusinessLayer.ContentBlock.DTOs.Output;
    using global::BusinessLayer.ContentBlock.Services;
    using global::BusinessLayer.DataPackage.Models;
    using global::BusinessLayer.DataPackage.Repositories;
    using global::BusinessLayer.DataPackage.Repositories.Models;
    using global::BusinessLayer.DataPackage.Services;
    using global::BusinessLayer.PriceEntry.Services;
    using global::BusinessLayer.PriceEntry.Services.Factories;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using global::Infrastructure.EventDispatcher;
    using global::Infrastructure.MongoDB.Models;
    using global::Infrastructure.Services.AuditInfoService;
    using global::Infrastructure.Services.Workflow;

    using Microsoft.Extensions.Internal;

    using NSubstitute;

    using Serilog;

    using Test.Infrastructure.TestData;

    using Xunit;

    using IPriceSeriesService = global::BusinessLayer.PriceSeriesSelection.Services.IPriceSeriesService;
    using Version = global::BusinessLayer.ContentBlock.ValueObjects.Version;

    public class DataPackageServiceShould
    {
        private readonly DataPackageService dataPackageService;

        private readonly IDataPackageWorkflowService workflowService;

        private readonly IPriceSeriesService priceSeriesService;

        private readonly WorkflowSettings workflowSettings;

        private readonly IDataPackageDomainService dataPackageDomainService;

        public DataPackageServiceShould()
        {
            var seriesItemTypeServiceFactory = Substitute.For<ISeriesItemTypeServiceFactory>();
            var contentBlockService = Substitute.For<IContentBlockService>();
            var halfMonthPriceSeriesItemService = Substitute.For<IHalfMonthPriceSeriesItemService>();
            workflowService = Substitute.For<IDataPackageWorkflowService>();
            Substitute.For<IDataPackageRepository>();
            var logger = Substitute.For<ILogger>();
            var auditInfoService = Substitute.For<IAuditInfoService>();
            var publicationScheduleService = Substitute.For<IPublicationScheduleService>();
            var clock = Substitute.For<ISystemClock>();
            priceSeriesService = Substitute.For<IPriceSeriesService>();

            auditInfoService.GetAuditInfoForUser(Arg.Is<string>(s => s == TestData.UserGuid.ToString()))
               .Returns(new AuditInfo { User = TestData.UserGuid.ToString(), Timestamp = TestData.Now });

            var eventDispatcher = Substitute.For<IEventDispatcher>();
            workflowSettings = new WorkflowSettings
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
                    { "STYRENE", "advance" }
                }
                }
            };

            dataPackageDomainService = Substitute.For<IDataPackageDomainService>();
            var dataPackageMetadataService = Substitute.For<IDataPackageMetadataDomainService>();
            var priceSeriesItemsDomainService = Substitute.For<IPriceSeriesItemsDomainService>();
            var contentBlockDomainService = Substitute.For<IContentBlockDomainService>();
            var commentaryDomainService = Substitute.For<ICommentaryDomainService>();

            dataPackageService = new DataPackageService(
                seriesItemTypeServiceFactory,
                contentBlockService,
                workflowService,
                auditInfoService,
                priceSeriesService,
                halfMonthPriceSeriesItemService,
                eventDispatcher,
                logger,
                workflowSettings,
                publicationScheduleService,
                clock,
                dataPackageMetadataService,
                dataPackageDomainService,
                priceSeriesItemsDomainService,
                contentBlockDomainService,
                commentaryDomainService);
        }

        [Fact]
        public async Task Not_Publish_Data_Package_When_Data_Package_Is_Not_Found()
        {
            // Arrange
            var dataPackageId = new DataPackageId("test-key");
            var businessKey = new WorkflowBusinessKey(dataPackageId.Value);
            dataPackageDomainService.GetDataPackage(businessKey).Returns((DataPackage?)null);

            // Act
            var result = await dataPackageService.OnDataPackagePublished(businessKey, TestData.UserGuid.ToString(), UserActions.Publish);

            // Assert
            result.Should().Be(DataPackageStatusChangeResult.NotFound);
        }

        [Fact]
        public async Task Return_Empty_Action_When_Price_Series_Id_List_Is_Empty_And_Data_Package_Status_Is_Published()
        {
            // Arrange
            var dataPackageId = new DataPackageId("test-key");
            var contentBlockDefinition = new ContentBlockDefinition()
            {
                ContentBlockId = "TestContentBlock",
                Version = 1,
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid
                    {
                        Id = "TestGrid_One",
                        PriceSeriesIds = null,
                        SeriesItemTypeCode = null,
                        Title = "TestGrid Title One"
                    },

                    new PriceSeriesGrid
                    {
                        Id = "TestGrid_Two",
                        PriceSeriesIds = null,
                        SeriesItemTypeCode = null,
                        Title = "TestGrid Title Two"
                    }
                ]
            };

            var dataPackage = new DataPackage()
            {
                Id = "TestId",
                ContentBlock = new ContentBlock() { Id = "TestId", Version = 1 },
                AssessedDateTime = TestData.Now,
                PriceSeriesItemGroups =
                [
                    new PriceSeriesItemGroup
                    {
                        SeriesItemTypeCode = "pi-single-with-ref",
                        PriceSeriesItemIds = ["Series1", "Series2"]
                    },

                    new PriceSeriesItemGroup
                    {
                        SeriesItemTypeCode = "pi-single",
                        PriceSeriesItemIds = ["Series3", "Series4"]
                    }
                ],
                Status = "PUBLISHED",
                LastModified = new AuditInfo() { User = TestData.UserGuid.ToString(), Timestamp = TestData.Now },
                Created = new AuditInfo() { User = TestData.UserGuid.ToString(), Timestamp = TestData.Now },
                SequenceId = Guid.NewGuid().ToString(),
            };

            var dataPackageKey = new DataPackageKey(
                contentBlockDefinition.ContentBlockId,
                new Version(contentBlockDefinition.Version),
                TestData.Now);
            dataPackageDomainService.GetDataPackage(dataPackageKey).Returns(dataPackage);

            // Act
            var result = await dataPackageService.GetNextActions(
                             contentBlockDefinition,
                             TestData.Now,
                             false);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Not_Return_Republish_Action_When_Toggle_Is_Off()
        {
            // Arrange
            workflowSettings.ShowRepublishButtonToggle = false;
            var dataPackageId = new DataPackageId("test-key");
            var contentBlockDefinition = new ContentBlockDefinition()
            {
                ContentBlockId = "TestContentBlock",
                Version = 1,
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid
                    {
                        Id = "TestGrid_One",
                        PriceSeriesIds = ["Series1", "Series2"],
                        SeriesItemTypeCode = "pi-single-with-ref",
                        Title = "TestGrid Title One"
                    },

                    new PriceSeriesGrid
                    {
                        Id = "TestGrid_Two",
                        PriceSeriesIds = ["Series3", "Series4"],
                        SeriesItemTypeCode = "pi-single",
                        Title = "TestGrid Title Two"
                    }
                ]
            };

            var dataPackage = new DataPackage()
            {
                Id = "TestId",
                ContentBlock = new ContentBlock() { Id = "TestId", Version = 1 },
                AssessedDateTime = TestData.Now,
                PriceSeriesItemGroups =
                [
                    new PriceSeriesItemGroup
                    {
                        SeriesItemTypeCode = "pi-single-with-ref",
                        PriceSeriesItemIds = ["Series1", "Series2"]
                    },

                    new PriceSeriesItemGroup
                    {
                        SeriesItemTypeCode = "pi-single",
                        PriceSeriesItemIds = ["Series3", "Series4"]
                    }
                ],
                Status = "PUBLISHED",
                LastModified = new AuditInfo() { User = TestData.UserGuid.ToString(), Timestamp = TestData.Now },
                Created = new AuditInfo() { User = TestData.UserGuid.ToString(), Timestamp = TestData.Now },
                SequenceId = Guid.NewGuid().ToString()
            };

            var priceSeries = new PriceSeries()
            {
                Id = "Series1",
                Commodity = new Commodity
                {
                    Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"),
                    Name = new Name()
                    {
                        English = "LNG"
                    },
                }
            };

            priceSeriesService.GetPriceSeriesById(Arg.Any<string>()).Returns(priceSeries);

            workflowService.GetWorkflowVersion("LNG").Returns(WorkflowVersion.Simple);

            var dataPackageKey = new DataPackageKey(
                contentBlockDefinition.ContentBlockId,
                new Version(contentBlockDefinition.Version),
                TestData.Now);
            dataPackageDomainService.GetDataPackage(dataPackageKey).Returns(dataPackage);

            // Act
            var result = await dataPackageService.GetNextActions(
                             contentBlockDefinition,
                             TestData.Now,
                             false);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
