namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders;
    using Authoring.Application.ContentBlock.DataLoaders.Keys;
    using Authoring.Application.ContentBlock.Helpers;

    using BusinessLayer.ContentBlock.DTOs.Output;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(ContentBlockDefinition))]
    public class PublicationScheduleIdResolver
    {
        [GraphQLName(Constants.ContentBlock.PublicationScheduleId)]
        public async Task<string?> GetPublicationScheduleId(
            [Parent] ContentBlockDefinition contentBlockDefinition,
            PriceSeriesBatchLoader priceSeriesBatchLoader,
            IResolverContext resolverContext,
            CancellationToken cancellationToken)
        {
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();
            var priceSeriesBatchKey = new PriceSeriesBatchKey(assessedDateTime, contentBlockDefinition.GetPriceSeriesIds());
            var priceSeriesDetails = await priceSeriesBatchLoader.LoadAsync(priceSeriesBatchKey, cancellationToken);
            return priceSeriesDetails?.FirstOrDefault()?.PublicationSchedules?.FirstOrDefault()?.ScheduleId ?? null;
        }
    }
}
