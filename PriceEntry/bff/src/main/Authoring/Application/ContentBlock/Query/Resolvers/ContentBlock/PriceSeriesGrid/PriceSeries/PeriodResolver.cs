namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock.PriceSeriesGrid.PriceSeries
{
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders;
    using Authoring.Application.ContentBlock.DataLoaders.Keys;
    using Authoring.Application.ContentBlock.Helpers;
    using Authoring.Application.ContentBlock.Query;

    using BusinessLayer.PriceEntry.DTOs.Output;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;
    using Serilog;

    using static BusinessLayer.Helpers.PeriodLabelTypeHelper;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(PriceSeries))]
    public class PeriodResolver
    {
        [GraphQLName(Constants.PriceSeries.PeriodField)]
        public async Task<string> GetPeriod(
            [Parent] PriceSeries priceSeries,
            [Service] ILogger logger,
            IResolverContext resolverContext,
            PeriodBatchDataLoader periodBatchDataLoader,
            CancellationToken cancellationToken)
        {
            var localLogger = logger.ForContext<PeriodResolver>();

            localLogger.Debug($"Resolving period for price series {priceSeries.Id}");

            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();

            string period;

            var absolutePeriodCalculationInput = GetAbsolutePeriodCalculationInput(
                                            localLogger,
                                            priceSeries,
                                            assessedDateTime);

            if (absolutePeriodCalculationInput is null)
            {
                return string.Empty;
            }

            var (referenceDate, periodCode) = absolutePeriodCalculationInput.Value;

            var periodBatchKey = new PeriodBatchKey(referenceDate, periodCode);

            var periodOutputItem = await periodBatchDataLoader.LoadAsync(periodBatchKey, cancellationToken);
            period = periodOutputItem?.Label ?? string.Empty;

            localLogger.Debug($"Resolved period for price series {priceSeries.Id} to {period}");

            return period;
        }
    }
}