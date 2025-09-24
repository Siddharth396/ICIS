namespace Authoring.Application.Commentary.Mutation
{
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.DTOs.Input;
    using BusinessLayer.Commentary.Repositories.Models;
    using BusinessLayer.Commentary.Services;
    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Mutation)]
    public class CommentaryMutations
    {
        // TODO: Consider returning CommentaryOutput instead of Commentary that comes from DB
        [GraphQLName("saveCommentary")]
        public async Task<Commentary> SaveCommentary(
            [GraphQLNonNullType] CommentaryInput commentaryInput,
            [Service] ICommentaryService commentaryService,
            [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(SaveCommentary)))
            {
                var localLogger = logger.ForContext<CommentaryMutations>();

                localLogger.Debug("START: Saving commentary");

                var response = await commentaryService.SaveCommentary(commentaryInput);

                localLogger.Debug("END: Saving commentary");

                return response;
            }
        }
    }
}
