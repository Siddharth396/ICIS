namespace Authoring.Application.PriceDisplay.PriceSeriesSelection
{
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
    public class PriceSeriesFilterQueries
    {
        [GraphQLName("commodities")]
        public async Task<IEnumerable<DropdownFilterItem>> GetCommodities([Service] IFilterService filterService)
        {
            return await filterService.GetCommodities();
        }
    }
}
