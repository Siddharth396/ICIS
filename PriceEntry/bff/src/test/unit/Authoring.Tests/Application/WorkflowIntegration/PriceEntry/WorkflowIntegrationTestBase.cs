namespace Authoring.Tests.Application.WorkflowIntegration.PriceEntry
{
    using System;
    using System.Threading.Tasks;

    using Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries;
    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    public abstract class WorkflowIntegrationTestBase : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        protected WorkflowIntegrationTestBase(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
        }

        protected abstract string PriceSeriesId { get; }

        protected abstract SeriesItemTypeCode SeriesItemTypeCode { get; }

        [Fact]
        public async Task Status_Should_Be_Ready_To_Start_When_Price_Item_Is_Not_Enter()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            _ = await new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .Execute();

            var response = await contentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Status_Should_Change_To_In_Draft_When_Save_The_Price()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            _ = await new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(PriceSeriesId, AssessedDateTime, SeriesItemTypeCode, BuildSeriesItem())
                   .Execute();

            var response = await contentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        protected abstract SeriesItem BuildSeriesItem();
    }
}