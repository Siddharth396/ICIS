namespace Subscriber.Application.Commentary
{
    using System.Threading.Tasks;

    using BusinessLayer.Services.Domain;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType("Query")]
    public class Query
    {
        //public async Task<ContentDto?> GetPublishedVersion(
        //    [GraphQLNonNullType] string pageId,
        //    [GraphQLNonNullType] string mfeIdentifier,
        //    [Service] ISubscriberService service)
        //{
        //    return await service.GetPublishedVersion(pageId, mfeIdentifier);
        //}
    }
}
