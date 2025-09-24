namespace Authoring.Application.Domain
{
    using System.Threading.Tasks;
    using BusinessLayer.Services.Domain;

    using global::Infrastructure.GraphQL;


    using HotChocolate;
    using HotChocolate.Authorization;
    using HotChocolate.Types;

    using Subscriber.Auth;

    [AddToGraphQLSchema]
    [ExtendObjectType("Mutation")]
    //[Authorize(Roles = new[] { "domain-write" })]
    public class Mutation
    {
        //public async Task<bool> Save(
        //    [GraphQLNonNullType] DomainMessageDto domainMessage,
        //    [GraphQLNonNullType] MetadataDto metadata,
        //    bool? isPublished,
        //    [Service] IUserContext userContext,
        //    [Service] IAuthoringService service)
        //{
        //    return await service.Save(domainMessage, metadata, userContext.UserGuidString, isPublished);
        //}
    }
}
