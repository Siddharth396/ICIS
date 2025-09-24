namespace Subscriber.Application.PriceDisplay.ContentBlock.Query.Resolvers
{
    using System.Collections.Generic;

    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceDisplay.PriceSeries.DTOs;
    using BusinessLayer.PriceDisplay.PriceSeries.Services;
    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Types;

    using Serilog.Context;

    using Subscriber.Application.PriceDisplay.ContentBlock.Query;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(ContentBlockDefinitionForDisplay))]
    public class PriceSeriesDisplayResolver
    {
        [GraphQLName(ConstantsForDisplay.ContentBlock.PriceSeriesItemField)]
        public async Task<IEnumerable<PriceSeriesResponse>?> GetPriceSeriesItems(
             [Parent] ContentBlockDefinitionForDisplay contentBlockDefinitionDisplay,
             [GraphQLNonNullType, GraphQLName(ConstantsForDisplay.ContentBlock.AssessedDateTimeParameter)] long assessedDateTimestamp,
             [Service] IPriceSeriesService priceSeriesService)
        {
            using (LogContext.PushProperty("ContentBlockId", contentBlockDefinitionDisplay.ContentBlockId))
            {
                if (!contentBlockDefinitionDisplay.IsValidForPriceSeries())
                {
                    return null;
                }

                return await priceSeriesService.GetPublishedPriceSeriesDetails(contentBlockDefinitionDisplay.Rows!, assessedDateTimestamp);
            }
        }
    }
}
