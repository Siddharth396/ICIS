namespace Authoring.Application.PriceDisplay.ContentBlock.Query.Resolvers
{
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceDisplay.GridConfiguration.DTOs;
    using BusinessLayer.PriceDisplay.GridConfiguration.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(ContentBlockDefinitionForDisplay))]
    public class GridConfigurationResolver
    {
        [GraphQLName(ConstantsForDisplay.ContentBlock.GridConfigurationField)]
        public async Task<GridConfiguration?> GetGridConfiguration(
            [Parent] ContentBlockDefinitionForDisplay contentBlockDefinition,
            [Service] IGridConfigurationService priceSeriesDisplayService)
        {
            var seriesItemTypeCodes = contentBlockDefinition.Rows?.Select(x => x.SeriesItemTypeCode).Distinct().ToList();

            if (seriesItemTypeCodes == null || seriesItemTypeCodes.Count == 0)
            {
                return null;
            }

            return await priceSeriesDisplayService.GetGridConfiguration(seriesItemTypeCodes, contentBlockDefinition!.ContentBlockId, contentBlockDefinition.Columns);
        }
    }
}