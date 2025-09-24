namespace Authoring.Application.ContentBlock.Query
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.ContentBlock.Services;
    using BusinessLayer.DataPackage.Helpers;
    using BusinessLayer.DataPackage.Services;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.Services.Workflow;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    using OperationType = HotChocolate.Language.OperationType;
    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Query)]
    public class ContentBlockQueries
    {
        [GraphQLName(Constants.ContentBlock.ContentBlockField)]
        public async Task<ContentBlockDefinition?> GetContentBlock(
            IResolverContext resolverContext,
            [GraphQLNonNullType] string contentBlockId,
            int? version,
            [GraphQLNonNullType, GraphQLName(Constants.ContentBlock.AssessedDateTimeParameter)] DateTime assessedDateTime,
            bool? isReviewMode,
            [Service] IContentBlockService service,
            [Service] IDataPackageDomainService dataPackageDomainService,
            [Service] IDataPackageMetadataDomainService dataPackageMetadataDomainService,
            [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(GetContentBlock)))
            {
                var localLogger = logger.ForContext<ContentBlockQueries>();

                localLogger.Debug("START: Fetching content block");

                var requestedVersion = version.HasValue ? Version.From(version.Value) : Version.Latest;

                var contentBlock = await service.GetContentBlock(
                                       contentBlockId,
                                       requestedVersion,
                                       assessedDateTime,
                                       isReviewMode ?? false);

                if (contentBlock != null)
                {
                    // We need to make sure the dataPackage metadata exists before we proceed to the other resolvers
                    await dataPackageMetadataDomainService.GetDataPackageId(contentBlock.GetDataPackageKey(assessedDateTime));

                    var dataPackageKey = contentBlock.GetDataPackageKey(assessedDateTime);
                    var dataPackage = await dataPackageDomainService.GetDataPackage(dataPackageKey);
                    resolverContext.ScopedContextData = resolverContext.ScopedContextData.SetItem(Constants.ScopedContext.DataPackageStatus, dataPackage != null ? new WorkflowStatus(dataPackage.Status) : WorkflowStatus.None);
                }

                // Set the contentBlockDefinition in the context so that the other resolvers can access it
                resolverContext.ScopedContextData = resolverContext.ScopedContextData.SetItem(
                    Constants.ScopedContext.ContentBlockDefinition,
                    contentBlock);

                // Set isReviewMode in the context so that the other resolvers can access it
                resolverContext.ScopedContextData = resolverContext.ScopedContextData.SetItem(
                    Constants.ScopedContext.IsReviewMode,
                    isReviewMode ?? false);

                localLogger.Debug("END: Fetching content block");

                return contentBlock;
            }
        }
    }
}