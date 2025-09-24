namespace Subscriber.Application.PriceDisplay.ContentBlock.Query
{
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.ValueObjects;
    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceDisplay.ContentBlock.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Query)]
    public class ContentBlockQueriesForDisplay
    {
        [GraphQLName(ConstantsForDisplay.ContentBlock.ContentBlockField)]
        public async Task<ContentBlockDefinitionForDisplay?> GetContentBlockForPriceDisplay(
            [GraphQLNonNullType] string contentBlockId,
            int? version,
            [Service] IContentBlockServiceForDisplay service,
            [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(GetContentBlockForPriceDisplay)))
            {
                var localLogger = logger.ForContext<ContentBlockQueriesForDisplay>();

                localLogger.Debug("START: Fetching content block");

                var requestedVersion = version.HasValue ? Version.From(version.Value) : Version.Latest;

                var contentBlock = await service.GetContentBlock(contentBlockId, requestedVersion);

                localLogger.Debug("END: Fetching content block");

                return contentBlock;
            }
        }
    }
}
