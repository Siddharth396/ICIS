namespace Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;

    using Test.Infrastructure.GraphQL;

    public class ContentBlockForDisplayAuthoringGraphQLClient
    {
        private readonly GraphQLClient client;

        public ContentBlockForDisplayAuthoringGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> GetContentBlockForDisplay(
            string contentBlockId,
            int? version = null)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(QueriesForDisplay.GetAuthoringContentBlockForDisplay)
               .AddVariable("contentBlockId", contentBlockId);

            if (version.HasValue)
            {
                builder.AddVariable("version", version);
            }

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockFromInputParametersForDisplay(
            List<string> seriesIds,
            DateTime assessedDateTime)
        {
            return client.SendAsync(
                GraphQLQueryBuilder.New()
                   .SetQuery(QueriesForDisplay.GetContentBlockFromInputParametersForDisplay)
                   .AddVariable("seriesIds", seriesIds)
                   .AddVariable("assessedDateTime", assessedDateTime)
                   .Build());
        }

        public Task<HttpResponseMessage> SaveContentBlockForDisplay(ContentBlockForDisplayInput contentBlockInput)
        {
            return client.SendAsync(
                GraphQLQueryBuilder.New()
                   .SetQuery("""
                             mutation saveContentBlockForDisplay($contentBlockInput: ContentBlockForDisplayInput!) {
                                         saveContentBlockForDisplay(contentBlockInput: $contentBlockInput) {
                                         contentBlockId
                                         errorCodes
                                         version
                                         }
                                     }
                             """)
                   .AddVariable("contentBlockInput", contentBlockInput)
                   .Build());
        }

        public Task<HttpResponseMessage> GetContentBlockWithGridConfigurationOnly(string contentBlockId, int? version = null)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(QueriesForDisplay.GetAuthoringContentBlockWithGridConfigurationOnly)
               .AddVariable("contentBlockId", contentBlockId);

            if (version.HasValue)
            {
                builder.AddVariable("version", version);
            }

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockForDisplayWithPriceSeriesOnly(string contentBlockId)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(QueriesForDisplay.GetAuthoringContentBlockForDisplayWithPriceSeriesOnly)
               .AddVariable("contentBlockId", contentBlockId);

            var query = builder.Build();
            return client.SendAsync(query);
        }
    }
}