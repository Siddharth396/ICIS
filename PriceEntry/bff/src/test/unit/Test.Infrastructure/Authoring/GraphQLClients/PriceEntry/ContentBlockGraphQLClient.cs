namespace Test.Infrastructure.Authoring.GraphQLClients.PriceEntry
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Input;

    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.GraphQL;

    public class ContentBlockGraphQLClient
    {
        private readonly GraphQLClient client;

        public ContentBlockGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> GetContentBlock(
            string contentBlockId,
            DateTime assessedDateTime,
            int? version = null)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(Queries.GetContentBlock)
               .AddVariable("assessedDateTime", assessedDateTime)
               .AddVariable("contentBlockId", contentBlockId);

            if (version.HasValue)
            {
                builder.AddVariable("version", version);
            }

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockOnly(string contentBlockId, int? version, DateTime assessedDateTime)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(Queries.GetContentBlockOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime);

            if (version.HasValue)
            {
                builder.AddVariable("version", version);
            }

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockWithGridConfigurationOnly(string contentBlockId, int? version, DateTime assessedDateTime)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(Queries.GetContentBlockWithGridConfigurationOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime);

            if (version.HasValue)
            {
                builder.AddVariable("version", version);
            }

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockWithCommentaryOnly(string contentBlockId, DateTime assessedDateTime)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(Queries.GetContentBlockWithCommentaryOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime);

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockWithPriceSeriesOnly(string contentBlockId, DateTime assessedDateTime, bool isReviewMode = false)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(Queries.GetContentBlockWithPriceSeriesOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime)
               .AddVariable("isReviewMode", isReviewMode);

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockWithPriceSeriesValidationErrorsOnly(
            string contentBlockId,
            DateTime assessedDateTime,
            bool includeNotStarted = false)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(Queries.GetContentBlockWithPriceSeriesValidationErrorsOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime)
               .AddVariable("includeNotStarted", includeNotStarted);

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockWithWorkflowBusinessKeyOnly(
            string contentBlockId,
            DateTime assessedDateTime)
        {
            var builder = GraphQLQueryBuilder
               .New()
               .SetQuery(Queries.GetContentBlockWithWorkflowBusinessKeyOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime);

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockWithNextActionsOnly(
            string contentBlockId,
            DateTime assessedDateTime,
            bool isReviewMode = false)
        {
            var builder = GraphQLQueryBuilder
               .New()
               .SetQuery(Queries.GetContentBlockWithNextActionsOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime)
               .AddVariable("isReviewMode", isReviewMode);

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> SaveContentBlock(ContentBlockInput contentBlockInput)
        {
            return client.SendAsync(
                GraphQLQueryBuilder.New()
                   .SetQuery(Mutations.SaveContentBlock)
                   .AddVariable("contentBlockInput", contentBlockInput)
                   .Build());
        }

        public Task<HttpResponseMessage> GetContentBlockWithDataPackageIdOnly(
            string contentBlockId,
            DateTime assessedDateTime)
        {
            var builder = GraphQLQueryBuilder
               .New()
               .SetQuery(Queries.GetContentBlockWithDataPackageIdOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime);

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public async Task<string> GetWorkflowBusinessKey(string contentBlockId, DateTime assessedDateTime)
        {
            var jsonResponse = await GetContentBlockWithWorkflowBusinessKeyOnly(contentBlockId, assessedDateTime)
                                  .GetResponseAsJsonDocument();

            var workflowBusinessKey = jsonResponse.RootElement
               .GetProperty("data")
               .GetProperty("contentBlock")
               .GetProperty("workflowBusinessKey")
               .GetString();

            if (workflowBusinessKey is null)
            {
                throw new Exception("Workflow business key not found");
            }

            return workflowBusinessKey;
        }

        public Task<HttpResponseMessage> ToggleNonMarketAdjustment(
            string contentBlockId,
            int version,
            DateTime assessedDateTime,
            bool enabled)
        {
            const string Query = """
                                 mutation ToggleNonMarketAdjustment($contentBlockId: String!, $version: Int!, $assessedDateTime: DateTime!, $enabled: Boolean!) {
                                     toggleNonMarketAdjustment(contentBlockId: $contentBlockId, version: $version, assessedDateTime: $assessedDateTime, enabled: $enabled)
                                 }
                                 """;

            var builder = GraphQLQueryBuilder.New()
               .SetQuery(Query)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("version", version)
               .AddVariable("assessedDateTime", assessedDateTime)
               .AddVariable("enabled", enabled);

            var builtQuery = builder.Build();
            return client.SendAsync(builtQuery);
        }

        public Task<HttpResponseMessage> GetContentBlockWithNmaEnabledOnly(
            string contentBlockId,
            DateTime assessedDateTime)
        {
            var builder = GraphQLQueryBuilder
               .New()
               .SetQuery("""
                         query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!){
                                     contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime){
                                         contentBlockId
                                         version
                                         nmaEnabled
                                     }
                                 }
                         """)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime);

            var query = builder.Build();
            return client.SendAsync(query);
        }
    }
}