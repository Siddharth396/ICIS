namespace Authoring.Tests.Application.UserPreference.Mutation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.ContentBlock.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::BusinessLayer.UserPreference.DTOs.Input;

    using Snapshooter;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    public abstract class UserPreferenceTestBase : WebApplicationTestBase
    {
        protected const string ContentBlockId = "contentBlockId";

        protected const string PriceSeriesGridIdOne = "priceSeriesGridIdOne";

        protected const string PriceSeriesGridIdTwo = "priceSeriesGridIdTwo";

        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        private readonly UserPreferenceGraphQLClient userPreferenceGraphQLClient;

        protected UserPreferenceTestBase(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            userPreferenceGraphQLClient = new UserPreferenceGraphQLClient(GraphQLClient);
        }

        protected abstract List<PriceSeriesGrid> PriceSeriesGrids { get; }

        protected abstract SeriesItemTypeCode SeriesItemTypeCode { get; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = ContentBlockId,
                Title = "Some content block title",
                PriceSeriesGrids = PriceSeriesGrids
            };

            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);
        }

        [Fact]
        public async Task Save_User_Preference()
        {
            // Arrange
            var userPreferenceInput = RetrieveUserPreferenceInputForGrid(PriceSeriesGridIdOne);

            // Act
            await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInput);

            var response = await contentBlockClient.GetContentBlock(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            // Assert
            response.MatchSnapshotWithUserPreferenceId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Update_User_Preference()
        {
            // Arrange
            var userPreferenceInput = RetrieveUserPreferenceInputForGrid(PriceSeriesGridIdOne);

            // Act
            await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInput);

            userPreferenceInput = UpdateUserPreferenceInput(userPreferenceInput);

            await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInput);

            // Assert
            var response = await contentBlockClient.GetContentBlock(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshotWithUserPreferenceId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Save_User_Preference_For_Second_Grid_When_FirstGrid_Preference_Exists()
        {
            // Arrange
            var userPreferenceInputFirstGrid = RetrieveUserPreferenceInputForGrid(PriceSeriesGridIdOne);
            _ = await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInputFirstGrid);

            // Act
            var userPreferenceInputSecondGrid = RetrieveUserPreferenceInputForGrid(PriceSeriesGridIdTwo);

            await userPreferenceGraphQLClient.SaveUserPreference(userPreferenceInputSecondGrid);

            // Assert
            var response = await contentBlockClient.GetContentBlock(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshotWithUserPreferenceId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        protected abstract List<UserPreferenceInput> GetUserPreferenceInputs();

        protected abstract UserPreferenceInput UpdateUserPreferenceInput(UserPreferenceInput userPreferenceInput);

        private UserPreferenceInput RetrieveUserPreferenceInputForGrid(string priceSeriesGridId)
        {
            var userPreferenceInputs = GetUserPreferenceInputs();

            return userPreferenceInputs.Single(x => x.PriceSeriesGridId == priceSeriesGridId);
        }
    }
}