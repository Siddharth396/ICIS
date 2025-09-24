namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock.PriceSeriesGrid
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders;
    using Authoring.Application.ContentBlock.DataLoaders.Keys;
    using Authoring.Application.ContentBlock.Helpers;

    using AutoMapper;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    using static BusinessLayer.Helpers.PeriodLabelTypeHelper;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(PriceSeriesGrid))]
    public class PriceSeriesResolver
    {
        [GraphQLName(Constants.ContentBlock.PriceSeriesField)]
        public async Task<List<BusinessLayer.PriceEntry.DTOs.Output.PriceSeries>?> GetPriceSeries(
            [Parent] PriceSeriesGrid priceSeriesGrid,
            [Service] ILogger logger,
            [Service] IAuthoringService priceEntryService,
            IResolverContext resolverContext,
            PriceSeriesBatchLoader priceSeriesBatchLoader,
            PeriodBatchDataLoader periodBatchDataLoader,
            UserPreferencesBatchLoader userPreferencesBatchLoader,
            CancellationToken cancellationToken,
            [Service] IMapper mapper)
        {
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();
            var contentBlockDefinition = resolverContext.GetContentBlockDefinitionFromScopedContext();
            var contentBlockId = contentBlockDefinition.ContentBlockId;

            using (LogContext.PushProperty("ContentBlockId", contentBlockId))
            {
                var localLogger = logger.ForContext<PriceSeriesResolver>();

                localLogger.Debug($"Resolving price series for price series grid {priceSeriesGrid.Id}");

                if (priceSeriesGrid.PriceSeriesIds == null || priceSeriesGrid.PriceSeriesIds.Count == 0)
                {
                    return null;
                }

                var priceSeriesBatchKey = new PriceSeriesBatchKey(assessedDateTime, priceSeriesGrid.PriceSeriesIds);
                var priceSeriesDetails = await priceSeriesBatchLoader.LoadAsync(priceSeriesBatchKey, cancellationToken);

                if (priceSeriesDetails.Count == 0)
                {
                    return null;
                }

                var periodBatchKeys = priceSeriesDetails
                   .Select(priceSeriesDetail =>
                    {
                        var absolutePeriodCalculationInput = GetAbsolutePeriodCalculationInput(
                                                             localLogger,
                                                             priceSeriesDetail,
                                                             assessedDateTime);

                        if (absolutePeriodCalculationInput is null)
                        {
                            return null;
                        }

                        var (referenceDate, periodCode) = absolutePeriodCalculationInput.Value;

                        return new PeriodBatchKey(referenceDate, periodCode ?? string.Empty);
                       })
                   .Where(periodBatchKey => periodBatchKey != null)
                   .Distinct()
                   .ToList();

                var absolutePeriods = (await periodBatchDataLoader.LoadAsync(periodBatchKeys!, cancellationToken))
                    .Where(period => period != null)
                    .Cast<PeriodCalculatorOutputItem>()
                    .ToList();

                var userPreference = await userPreferencesBatchLoader.LoadAsync(contentBlockId, cancellationToken);

                var priceSeries = await priceEntryService.GetPriceSeriesDetailsForGrid(
                                      priceSeriesGrid,
                                      assessedDateTime,
                                      userPreference,
                                      priceSeriesDetails,
                                      absolutePeriods);

                localLogger.Debug($"Resolved price series for price series grid {priceSeriesGrid.Id}");

                return priceSeries;
            }
        }
    }
}
