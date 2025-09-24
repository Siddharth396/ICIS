namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.Helpers;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.DataPackage.Services;

    using global::Infrastructure.GraphQL;
    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(ContentBlockDefinition))]
    public class NextActionsResolver
    {
        [GraphQLName(Constants.ContentBlock.NextActions)]
        public async Task<List<NextAction>?> GetNextActions(
            IResolverContext resolverContext,
            [Parent] ContentBlockDefinition contentBlockDefinition,
            [Service] IDataPackageService dataPackageService)
        {
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();
            var isReviewMode = resolverContext.GetIsReviewModeFromScopedContext();

            var actions = await dataPackageService.GetNextActions(contentBlockDefinition, assessedDateTime, isReviewMode);

            return actions.Select(x => new NextAction(x.Key, x.Value, x.IsAllowed)).ToList();
        }
    }
}
