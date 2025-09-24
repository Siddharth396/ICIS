namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock
{
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.Helpers;

    using BusinessLayer.Commentary.DTOs.Output;
    using BusinessLayer.Commentary.Services;
    using BusinessLayer.ContentBlock.DTOs.Output;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(ContentBlockDefinition))]
    public class CommentaryResolver
    {
        [GraphQLName(Constants.ContentBlock.CommentaryField)]
        public async Task<CommentaryOutput?> GetCommentary(
            [Parent] ContentBlockDefinition contentBlockDefinition,
            [Service] ICommentaryService commentaryService,
            IResolverContext resolverContext)
        {
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();
            return await commentaryService.GetCommentary(
                       contentBlockDefinition.ContentBlockId,
                       assessedDateTime);
        }
    }
}