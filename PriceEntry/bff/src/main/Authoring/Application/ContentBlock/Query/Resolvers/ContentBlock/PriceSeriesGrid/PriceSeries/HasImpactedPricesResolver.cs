namespace Authoring.Application.ContentBlock.Query.PriceSeriesResolvers
{
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders;
    using Authoring.Application.ContentBlock.DataLoaders.Keys;
    using Authoring.Application.ContentBlock.Helpers;

    using BusinessLayer.PriceEntry.DTOs.Output;
    using global::Infrastructure.GraphQL;
    using global::Infrastructure.Services.Workflow;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Serilog;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(PriceSeries))]
    public class HasImpactedPricesResolver
    {
        [GraphQLName(Constants.PriceSeries.HasImpactedPricesField)]
        public async Task<bool?> HasImpactedPrices(
            [Parent] PriceSeries priceSeries,
            [Service] ILogger logger,
            IResolverContext resolverContext,
            PriceSeriesHasImpactedPricesBatchDataLoader priceSeriesHasImpactedPricesBatchDataLoader)
        {
            var localLogger = logger.ForContext<HasImpactedPricesResolver>();

            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();

            localLogger.Debug("resolving hasImpactedPrices for price series {priceSeriesId} on assessed date {AssessedDateTime}", priceSeries.Id, assessedDateTime);

            var dataPackageWorkflowStatus = resolverContext.GetDataPackageWorkflowStatusFromScopedContext();

            if (dataPackageWorkflowStatus.Equals(WorkflowStatus.None))
            {
                localLogger.Debug("No data package found for price series {priceSeriesId} on assessed date {AssessedDateTime}", priceSeries.Id, assessedDateTime);
                return null;
            }

            if (!WorkflowStatus.IsCorrectionPrePublishStatus(dataPackageWorkflowStatus))
            {
                localLogger.Debug("Price series {priceSeriesId} on assessed date {AssessedDateTime} is not in a correction workflow, returning null", priceSeries.Id, assessedDateTime);
                return null;
            }

            var priceSeriesBatchKey = new HasImpactedPricesBatchKey(priceSeries.Id, assessedDateTime);

            var hasImpactedPrices = await priceSeriesHasImpactedPricesBatchDataLoader.LoadAsync(priceSeriesBatchKey);

            localLogger.Debug("Resolved hasImpactedPrices for price series {priceSeriesId} on assessed date {AssessedDateTime} to {hasImpactedPrices}", priceSeries.Id, assessedDateTime, hasImpactedPrices);

            return hasImpactedPrices;
        }
    }
}
