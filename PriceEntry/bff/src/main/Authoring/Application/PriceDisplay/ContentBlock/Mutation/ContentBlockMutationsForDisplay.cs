namespace Authoring.Application.PriceDisplay.ContentBlock.Mutation
{
    using System.Threading.Tasks;

    using Authoring.Application.PriceDisplay.ContentBlock.Query;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;
    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceDisplay.ContentBlock.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Mutation)]
    public class ContentBlockMutationsForDisplay
    {
        [GraphQLName(ConstantsForDisplay.ContentBlock.SaveContentBlockField)]
        public async Task<ContentBlockSaveResponseForDisplay> SaveContentBlockForDisplay(
            [GraphQLNonNullType] ContentBlockForDisplayInput contentBlockInput,
            [Service] IContentBlockServiceForDisplay service,
            [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(SaveContentBlockForDisplay)))
            {
                var localLogger = logger.ForContext<ContentBlockMutationsForDisplay>();

                localLogger.Debug("START: Saving content block");

                var response = await service.SaveContentBlock(contentBlockInput);

                localLogger.Debug("END: Saving content block");

                return response;
            }
        }
    }
}
