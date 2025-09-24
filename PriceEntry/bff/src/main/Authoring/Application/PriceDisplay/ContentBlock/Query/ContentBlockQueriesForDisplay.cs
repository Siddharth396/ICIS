namespace Authoring.Application.PriceDisplay.ContentBlock.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceDisplay.ContentBlock.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

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

        [GraphQLName(ConstantsForDisplay.ContentBlock.ContentBlockFromInputParametersField)]
        public async Task<ContentBlockDefinitionForDisplay> GetContentBlockFromInputParametersForPriceDisplay(
            IResolverContext resolverContext,
            [GraphQLNonNullType] List<string> seriesIds,
            [GraphQLNonNullType] DateTime assessedDateTime,
            [Service] IContentBlockServiceForDisplay service,
            [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(GetContentBlockFromInputParametersForPriceDisplay)))
            {
                var localLogger = logger.ForContext<ContentBlockQueriesForDisplay>();

                localLogger.Debug("START: Fetching content block");

                var contentBlock = await service.GetContentBlockFromInputParameters(
                    seriesIds,
                    assessedDateTime);

                localLogger.Debug("END: Fetching content block");

                resolverContext.ScopedContextData = resolverContext.ScopedContextData.SetItem(ConstantsForDisplay.ScopedContext.IsBuiltFromInputParameters, true);

                return contentBlock;
            }
        }
    }
}
