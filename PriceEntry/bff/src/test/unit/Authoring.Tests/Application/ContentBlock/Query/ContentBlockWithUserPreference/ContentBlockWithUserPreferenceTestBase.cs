namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithUserPreference
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;
    using global::BusinessLayer.ContentBlock.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::BusinessLayer.UserPreference.DTOs.Input;
    using Snapshooter;
    using Snapshooter.Xunit;
    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;
    using Xunit;

    public abstract class ContentBlockWithUserPreferenceTestBase : WebApplicationTestBase
    {
        protected const string ContentBlockId = "contentBlockId";

        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        private readonly UserPreferenceGraphQLClient userPreferenceGraphQLClient;

        protected ContentBlockWithUserPreferenceTestBase(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            userPreferenceGraphQLClient = new UserPreferenceGraphQLClient(GraphQLClient);
        }

        protected abstract List<string> PriceSeriesIds { get; }

        protected abstract SeriesItemTypeCode SeriesItemTypeCode { get; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = ContentBlockId,
                Title = "Some content block title",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid
                    {
                        Id = $"{ContentBlockId}_Grid_1",
                        Title = "Some grid title",
                        PriceSeriesIds = PriceSeriesIds
                    }
                ]
            };

            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);
        }

        [Fact]
        public async Task Return_Default_Grid_Configuration_When_User_Preference_Does_Not_Exists()
        {
            // Assert
            var response = await contentBlockClient.GetContentBlock(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Updated_Grid_Configuration_When_User_Preference_Exists()
        {
            // Arrange
            var userPreferenceInput = BuildUserPreferenceInput();

            _ = await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInput);

            // Assert
            var response = await contentBlockClient.GetContentBlock(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Configuration_With_User_Preference_When_New_Series_Is_Added()
        {
            // Arrange
            var userPreferenceInput = BuildUserPreferenceInput();

            _ = await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInput);

            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = ContentBlockId,
                Title = "Some content block title",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid
                    {
                        Id = $"{ContentBlockId}_Grid_1",
                        Title = "Some grid title",
                        PriceSeriesIds = GetPriceSeriesIds()
                    }
                ]
            };

            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            // Assert
            var response = await contentBlockClient.GetContentBlock(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Configuration_With_User_Preference_When_New_Column_Is_Added()
        {
            // Arrange
            var userPreferenceInput = BuildUserPreferenceInputWithLessColumn();

            _ = await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInput);

            // Assert
            var response = await contentBlockClient.GetContentBlock(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        protected abstract UserPreferenceInput BuildUserPreferenceInput();

        protected abstract UserPreferenceInput BuildUserPreferenceInputWithLessColumn();

        protected abstract List<string> GetPriceSeriesIds();
    }
}