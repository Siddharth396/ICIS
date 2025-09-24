namespace Authoring.Application.ContentBlock.Mutation
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Input;
    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.ContentBlock.Services;
    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.PriceEntry.ValueObjects;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Mutation)]
    public class ContentBlockMutations
    {
        [GraphQLName("saveContentBlock")]
        public async Task<ContentBlockSaveResponse> SaveContentBlock(
            [GraphQLNonNullType] ContentBlockInput contentBlockInput,
            [Service] IContentBlockService contentBlockService,
            [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(SaveContentBlock)))
            {
                var localLogger = logger.ForContext<ContentBlockMutations>();

                localLogger.Debug("START: Saving content block");

                var response = await contentBlockService.SaveContentBlock(contentBlockInput);

                localLogger.Debug("END: Saving content block");

                return response;
            }
        }

        [GraphQLName("toggleNonMarketAdjustment")]
        public async Task<bool> ToggleNonMarketAdjustment(
            [GraphQLNonNullType] string contentBlockId,
            [GraphQLNonNullType] int version,
            [GraphQLNonNullType] DateTime assessedDateTime,
            [GraphQLNonNullType] bool enabled,
            [Service] IDataPackageMetadataDomainService dataPackageMetadataDomainService,
            [Service] ILogger logger)
        {
            await dataPackageMetadataDomainService.ToggleNonMarketAdjustment(
                new DataPackageKey(contentBlockId, Version.From(version), assessedDateTime),
                enabled);

            return true;
        }
    }
}
