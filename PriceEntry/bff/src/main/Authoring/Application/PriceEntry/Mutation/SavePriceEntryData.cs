namespace Authoring.Application.PriceEntry.Mutation
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.DTOs.Output;
    using BusinessLayer.PriceEntry.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    [AddToGraphQLSchema]
    [ExtendObjectType("Mutation")]
    public class SavePriceEntryData
    {
        [GraphQLName("updatePriceEntryGridData")]
        public async Task<PriceEntryDataSaveResponse> Save(
            [GraphQLNonNullType] PriceItemInput priceItemInput,
            [Service] IAuthoringService service,
            [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(SavePriceEntryData)))
            {
                var localLogger = logger.ForContext<SavePriceEntryData>();

                localLogger.Debug("START: Saving price entry grid data");

                var response = await service.SavePriceEntryData(priceItemInput);

                localLogger.Debug("END: Saving price entry grid data");

                return response;
            }
        }
    }
}
