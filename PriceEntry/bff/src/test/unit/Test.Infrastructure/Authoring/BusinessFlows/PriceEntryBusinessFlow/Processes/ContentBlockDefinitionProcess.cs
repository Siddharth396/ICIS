namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Input;

    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    public class ContentBlockDefinitionProcess : IPriceEntryBusinessFlowProcess
    {
        private readonly string contentBlockId;
        private readonly IReadOnlyCollection<IReadOnlyCollection<string>>? priceSeriesIds;

        public ContentBlockDefinitionProcess(string contentBlockId, IReadOnlyCollection<IReadOnlyCollection<string>>? priceSeriesIds)
        {
            this.contentBlockId = contentBlockId ?? throw new ArgumentNullException(nameof(contentBlockId));
            this.priceSeriesIds = priceSeriesIds;
        }

        public async Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = contentBlockId,
                Title = "Test Content Block",
                PriceSeriesGrids = priceSeriesIds == null || priceSeriesIds.Count == 0
                                       ? [new PriceSeriesGrid()]
                                       : priceSeriesIds
                                         ?.Select(
                                               (seriesIds, index) => new PriceSeriesGrid
                                               {
                                                   Id = $"{contentBlockId}_Grid_{index + 1}",
                                                   Title = $"Test Grid {index + 1}",
                                                   PriceSeriesIds = seriesIds.ToList()
                                               })
                                          .ToList()
            };
            var contentBlockClient = new ContentBlockGraphQLClient(new GraphQLClient(httpClient));
            return await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);
        }
    }
}
