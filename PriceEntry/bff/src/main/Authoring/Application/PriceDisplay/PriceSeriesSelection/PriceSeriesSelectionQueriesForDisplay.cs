namespace Authoring.Application.PriceDisplay.PriceSeriesSelection
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Query)]
    public class PriceSeriesSelectionQueriesForDisplay
    {
        [GraphQLName("priceSeriesForDisplayTool")]
        public async Task<PriceSeriesSelectionItem> GetPriceSeriesForDisplayWithFilters(
            List<Guid> commodities,
            bool includeInactivePriceSeries,
            [Service] IPriceSeriesDisplayService priceSeriesDisplayService)
        {
            return includeInactivePriceSeries ? await priceSeriesDisplayService.GetAllPriceSeries(commodities) :
                                                await priceSeriesDisplayService.GetActivePriceSeries(commodities);
        }
    }
}
