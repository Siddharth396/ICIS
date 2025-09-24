namespace Authoring.Tests.Application.Commentary.Mutation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.Commentary.DTOs.Input;

    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class CommentaryMutationShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockGraphQLClient contentBlockClient;
        private readonly CommentaryGraphQLClient commentaryGraphQLClient;

        public CommentaryMutationShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            commentaryGraphQLClient = new CommentaryGraphQLClient(GraphQLClient);
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { "dummy_price_series" }, HttpClient)
                                .SaveContentBlockDefinition();

            await businessFlow.Execute();
        }

        [Fact]
        public async Task Save_Commentary_With_Initial_Version()
        {
            // Arrange
            var commentaryInput = new CommentaryInput
            {
                ContentBlockId = ContentBlockId,
                CommentaryId = "92206d33-b512-49a2-941f-f566af6d3468",
                AssessedDateTime = AssessedDateTime,
                Version = "0.1"
            };

            // Act
            var result = await commentaryGraphQLClient.SaveCommentary(commentaryInput).GetRawResponse();

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Save_Commentary_With_Updated_Version()
        {
            // Arrange
            var commentaryInput = new CommentaryInput
            {
                ContentBlockId = ContentBlockId,
                CommentaryId = "de13e20a-7067-4d21-a1a5-6e3ae3ea3ca1",
                AssessedDateTime = AssessedDateTime,
                Version = "0.1"
            };
            await commentaryGraphQLClient.SaveCommentary(commentaryInput);

            commentaryInput.Version = "0.2";

            await commentaryGraphQLClient.SaveCommentary(commentaryInput);

            // Act
            var result = await contentBlockClient.GetContentBlockWithCommentaryOnly(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            // Assert
            result.MatchSnapshot();
        }
    }
}