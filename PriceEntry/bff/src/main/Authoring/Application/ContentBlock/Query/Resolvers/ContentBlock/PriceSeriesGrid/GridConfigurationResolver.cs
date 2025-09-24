namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock.PriceSeriesGrid
{
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders;
    using Authoring.Application.ContentBlock.Helpers;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceEntry.DTOs.Output;
    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.Services.Factories;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(PriceSeriesGrid))]
    public class GridConfigurationResolver
    {
        [GraphQLName(Constants.ContentBlock.GridConfigurationField)]
        public async Task<GridConfiguration?> GetGridConfiguration(
            [Parent] PriceSeriesGrid priceSeriesGrid,
            [Service] ILogger logger,
            [Service] IAuthoringService authoringService,
            UserPreferencesBatchLoader userPreferencesBatchLoader,
            GridConfigurationBatchLoader gridConfigurationBatchLoader,
            IResolverContext resolverContext,
            CancellationToken cancellationToken)
        {
            var contentBlockDefinition = resolverContext.GetContentBlockDefinitionFromScopedContext();

            using (LogContext.PushProperty("ContentBlockId", contentBlockDefinition.ContentBlockId))
            {
                var localLogger = logger.ForContext<GridConfigurationResolver>();

                localLogger.Debug($"Resolving grid configuration for price series grid {priceSeriesGrid.Id}");

                if (string.IsNullOrWhiteSpace(priceSeriesGrid.SeriesItemTypeCode))
                {
                    return null;
                }

                // If this batch loader key has been called previously, it gets re-used, so it doesn't need to go to the db again
                var userPreference = await userPreferencesBatchLoader.LoadAsync(contentBlockDefinition.ContentBlockId, cancellationToken);

                var seriesItemTypeCode = SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(priceSeriesGrid.SeriesItemTypeCode);

                var gridConfiguration = await gridConfigurationBatchLoader.LoadAsync(
                                            seriesItemTypeCode,
                                            cancellationToken);

                gridConfiguration = authoringService.GetGridConfigurationWithUserPreferences(
                                            priceSeriesGrid.Id,
                                            gridConfiguration,
                                            userPreference);

                var dataPackageWorkflowStatus = resolverContext.GetDataPackageWorkflowStatusFromScopedContext();

                gridConfiguration =
                    authoringService.GetGridConfigurationWithCorrectionColumns(
                        gridConfiguration,
                        dataPackageWorkflowStatus);

                localLogger.Debug($"Resolved grid configuration for price series grid {priceSeriesGrid.Id}");

                return gridConfiguration;
            }
        }
    }
}
