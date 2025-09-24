namespace Authoring.Application.PriceSeriesSelection.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceSeriesSelection.DTOs.Output;
    using BusinessLayer.PriceSeriesSelection.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Query)]
    public class PriceSeriesSelectionQueries
    {
        [GraphQLName("priceSeries")]
        public async Task<IEnumerable<PriceSeriesSelectionItem>> GetPriceSeriesWithFilters(
            Guid commodityId,
            Guid priceCategoryId,
            Guid regionId,
            Guid priceSettlementTypeId,
            Guid itemFrequencyId,
            [Service] IPriceSeriesService priceSeriesQueryService)
        {
            return await priceSeriesQueryService.GetPriceSeriesWithFilters(
                       commodityId,
                       priceCategoryId,
                       regionId,
                       priceSettlementTypeId,
                       itemFrequencyId);
        }

        [GraphQLName("filters")]
        public async Task<IEnumerable<Filter>> GetFilters(
            IEnumerable<string>? selectedPriceSeriesIds,
            [Service] IPriceSeriesService priceSeriesQueryService)
        {
            return await priceSeriesQueryService.GetFilters(selectedPriceSeriesIds ?? []);
        }
    }
}
