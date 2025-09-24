namespace Authoring.Tests.Application.PricingContentPackage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.ContentPackageGroup.Repositories;
    using global::BusinessLayer.ContentPackageGroup.Repositories.Models;
    using global::BusinessLayer.PricingContentPackage.Repositories;
    using global::BusinessLayer.PricingContentPackage.Repositories.Models;

    using Microsoft.Extensions.DependencyInjection;

    using Snapshooter;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Mongo.Repositories;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class PricingContentPackageShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private readonly CanvasApiServiceStub canvasApiServiceStub;

        private readonly GenericRepository genericRepository;

        public PricingContentPackageShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            canvasApiServiceStub = factory.Services.GetRequiredService<CanvasApiServiceStub>();
            var clock = factory.Services.GetRequiredService<TestClock>();
            clock.SetUtcNow(TestData.Now);
            canvasApiServiceStub.ClearRequests();
            genericRepository = GetService<GenericRepository>();
        }

        public static TheoryData<string, Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow>>
            WorkflowForCommentariesData =>
            new()
            {
                {
                    "No_Updates_Of_Commentary",
                    (businessFlow, now) =>
                        businessFlow
                           .SaveCommentary(now, "0.1")
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now)
                           .InitiateApproval(now)
                           .AcknowledgeApproval(now)
                           .AcknowledgePublished(now)
                },
                {
                    "Add_Commentary_In_Review_Only",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .SaveCommentary(now, "0.1")
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                },
                {
                    "Update_Commentary_In_Review_And_Publish",
                    (businessFlow, now) => businessFlow
                       .SaveCommentary(now, "0.1")
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .SaveCommentary(now, "0.2")
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                }

                // todo - add back test when properly handling commentary corrections
                // {
                //     "Update_Commentary_In_Correction",
                //     (businessFlow, now) =>
                //         businessFlow
                //            .SaveCommentary(now, "0.1")
                //            .InitiateSendForReview(now)
                //            .AcknowledgeSendForReview(now)
                //            .InitiateStartReview(now)
                //            .AcknowledgeStartReview(now)
                //            .InitiateApproval(now)
                //            .AcknowledgeApproval(now)
                //            .AcknowledgePublished(now)
                //            .InitiateCorrection(now)
                //            .SaveCommentary(now, "0.2")
                //            .InitiateSendForReview(now)
                //            .AcknowledgeSendForReview(now)
                //            .InitiateStartReview(now)
                //            .AcknowledgeStartReview(now)
                //            .InitiateApproval(now)
                //            .AcknowledgeApproval(now)
                //            .AcknowledgePublished(now)
                // }
            };

        [Fact]
        public async Task Notify_Canvas_When_Prices_Are_Published()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var seriesIds = new List<string> { TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2 };
            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, seriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, TestData.Now, seriesItem)
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, TestData.Now, seriesItem)
                   .SaveCommentary(TestData.Now)
                   .InitiatePublish(TestData.Now)
                   .AcknowledgePublished(TestData.Now);

            _ = await businessFlow.Execute();

            canvasApiServiceStub.Requests.MatchSnapshotForCanvasApiRequests();
        }

        [Fact]
        public async Task Notify_Canvas_When_Prices_Are_Corrected()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var seriesIds = new List<string> { TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2 };
            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, seriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, TestData.Now, seriesItem)
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, TestData.Now, seriesItem)
                   .SaveCommentary(TestData.Now)
                   .InitiatePublish(TestData.Now)
                   .AcknowledgePublished(TestData.Now)
                   .InitiateCorrection(TestData.Now);

            _ = await businessFlow.Execute();

            canvasApiServiceStub.Requests.MatchSnapshotForCanvasApiRequests();
        }

        [Theory]
        [MemberData(nameof(WorkflowForCommentariesData))]
        public async Task Send_Latest_Version_Of_Commentaries(
            string scenario,
            Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow> configureWorkflowUntilStatus)
        {
            var seriesItem = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var seriesIds = new List<string> { TestSeries.Melamine_Asia_SE_Spot };
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, seriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, TestData.Now, seriesItem);

            _ = await configureWorkflowUntilStatus.Invoke(businessFlow, TestData.Now).Execute();

            canvasApiServiceStub.Requests.MatchSnapshotForCanvasApiRequests(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Send_SequenceId_To_Canvas_Api()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var seriesIds = new List<string> { TestSeries.LNG_South_Korea_HM1, TestSeries.LNG_China_HM1 };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, seriesIds, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM1, TestData.Now, seriesItem)
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, TestData.Now, seriesItem)
                .SaveCommentary(TestData.Now)
                .PublishWithAdvanceWorkflow(TestData.Now);

            _ = await businessFlow.Execute();

            var contentPackageGroup = (await genericRepository.GetDocument<ContentPackageGroup>(
                ContentPackageGroupRepository.CollectionName,
                x => x.PriceSeriesIds.Count == seriesIds.Count &&
                     x.PriceSeriesIds.All(priceSeriesId => seriesIds.Contains(priceSeriesId)))).First();

            var pricingContentPackage = (await genericRepository.GetDocument<ContentPackage>(
                ContentPackageRepository.CollectionName,
                x => x.SequenceId == contentPackageGroup.SequenceId)).Single();

            var richTextSequenceId = $"{contentPackageGroup.SequenceId}-rchtxt";
            var priceEntrySequenceId = $"{contentPackageGroup.SequenceId}-prcent";

            canvasApiServiceStub.Requests
                .Should()
                .NotBeNullOrEmpty()
                .And
                .AllSatisfy(request =>
                {
                    request.ContentPackage.SequenceId.Should().Be(contentPackageGroup.SequenceId);
                    request.ContentPackage.Contents.ContentBlocks
                        .Should()
                        .NotBeNullOrEmpty()
                        .And
                        .SatisfyRespectively(
                            contentBlockOne =>
                            {
                                contentBlockOne.SequenceId.Should().Be(priceEntrySequenceId);
                            },
                            contentBlockTwo =>
                            {
                                contentBlockTwo.SequenceId.Should().Be(richTextSequenceId);
                            });
                });

            pricingContentPackage.Should().NotBeNull();
            pricingContentPackage.SequenceId.Should().Be(contentPackageGroup.SequenceId);
            pricingContentPackage.Contents.ContentBlocks
                .Should()
                .NotBeNullOrEmpty()
                .And
                .SatisfyRespectively(
                    contentBlockOne =>
                    {
                        contentBlockOne.SequenceId.Should().Be(priceEntrySequenceId);
                    },
                    contentBlockTwo =>
                    {
                        contentBlockTwo.SequenceId.Should().Be(richTextSequenceId);
                    });
        }
    }
}
