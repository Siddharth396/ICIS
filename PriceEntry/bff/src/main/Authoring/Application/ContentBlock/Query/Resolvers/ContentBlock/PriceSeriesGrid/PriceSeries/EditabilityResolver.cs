namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock.PriceSeriesGrid.PriceSeries
{
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders;
    using Authoring.Application.ContentBlock.DataLoaders.Keys;
    using Authoring.Application.ContentBlock.Helpers;
    using Authoring.Application.ContentBlock.Query;

    using BusinessLayer.DataPackage.Helpers;
    using BusinessLayer.PriceEntry.DTOs.Output;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Serilog;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(PriceSeries))]
    public class EditabilityResolver
    {
        [GraphQLName(Constants.PriceSeries.ReadOnlyField)]
        public async Task<bool> GetReadOnlyStatus(
            [Parent] PriceSeries priceSeries,
            IResolverContext resolverContext,
            PriceSeriesEditabilityBatchLoader gridReadOnlyBatchLoader,
            [Service] ILogger logger,
            CancellationToken cancellationToken)
        {
            var localLogger = logger.ForContext<EditabilityResolver>();

            localLogger.Debug($"Resolving readOnly for price series {priceSeries.Id}");

            var contentBlockDefinition = resolverContext.GetContentBlockDefinitionFromScopedContext();
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();
            var isReviewMode = resolverContext.GetIsReviewModeFromScopedContext();

            var dataPackageKey = contentBlockDefinition.GetDataPackageKey(assessedDateTime);
            var gridReadOnlyBatchKey = new PriceSeriesEditabilityBatchKey(dataPackageKey, priceSeries.Status, isReviewMode);

            var isEditable = await gridReadOnlyBatchLoader.LoadAsync(gridReadOnlyBatchKey, cancellationToken);
            var isReadOnly = !isEditable;

            localLogger.Debug($"Resolved readOnly for price series {priceSeries.Id} to {isReadOnly}");

            return isReadOnly;
        }
    }
}