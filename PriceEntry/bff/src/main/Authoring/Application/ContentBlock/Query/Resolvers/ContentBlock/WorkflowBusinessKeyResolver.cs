namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock
{
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.Helpers;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.PriceEntry.ValueObjects;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(ContentBlockDefinition))]
    public class WorkflowBusinessKeyResolver
    {
        [GraphQLName(Constants.ContentBlock.WorkflowBusinessKey)]
        public async Task<string?> GetWorkflowBusinessKey(
            [Parent] ContentBlockDefinition contentBlockDefinition,
            [Service] IDataPackageService dataPackageService,
            IResolverContext resolverContext)
        {
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();
            var workflowBusinessKey = await dataPackageService.GetWorkflowBusinessKey(
                                          new DataPackageKey(
                                              contentBlockDefinition.ContentBlockId,
                                              Version.From(contentBlockDefinition.Version),
                                              assessedDateTime));

            return workflowBusinessKey?.Value;
        }
    }
}
