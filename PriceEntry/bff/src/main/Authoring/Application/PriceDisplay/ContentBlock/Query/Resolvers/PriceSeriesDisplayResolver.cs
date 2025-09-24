namespace Authoring.Application.PriceDisplay.ContentBlock.Query.Resolvers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Application.PriceDisplay.ContentBlock.Helpers;
    using Authoring.Application.PriceDisplay.ContentBlock.Query;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceDisplay.PriceSeries.DTOs;
    using BusinessLayer.PriceDisplay.PriceSeries.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Serilog.Context;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(ContentBlockDefinitionForDisplay))]
    public class PriceSeriesDisplayResolver
    {
        [GraphQLName(ConstantsForDisplay.ContentBlock.PriceSeriesItemField)]
        public async Task<IEnumerable<PriceSeriesResponse>?> GetPriceSeriesItems(
            IResolverContext resolverContext,
            [Parent] ContentBlockDefinitionForDisplay contentBlockDefinitionDisplay,
            [Service] IPriceSeriesService priceSeriesService)
        {
            using (LogContext.PushProperty("ContentBlockId", contentBlockDefinitionDisplay.ContentBlockId))
            {
                if (!contentBlockDefinitionDisplay.IsValidForPriceSeries())
                {
                    return null;
                }

                var isBuiltFromInputParameters = resolverContext.GetBuiltFromInputParametersFromScopedContext();

                if (isBuiltFromInputParameters == true && contentBlockDefinitionDisplay.AssessedDateTime.HasValue)
                {
                    return await priceSeriesService.GetCurrentOrPendingPriceSeriesDetails(contentBlockDefinitionDisplay.Rows!, contentBlockDefinitionDisplay.AssessedDateTime.Value);
                }

                return await priceSeriesService.GetPriceSeriesDetails(contentBlockDefinitionDisplay.Rows!);
            }
        }
    }
}
