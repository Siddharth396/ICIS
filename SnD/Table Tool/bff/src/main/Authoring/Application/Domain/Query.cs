namespace Authoring.Application.Domain
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BusinessLayer.Services.Domain;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Types;

    using Microsoft.AspNetCore.Authorization;

    [AddToGraphQLSchema]
    [ExtendObjectType("Query")]
    //[Authorize(Roles = new[] { "domain-read" })]
    //[AllowAnonymous]
    public class Query
    {
        //public async Task<ContentDto?> GetSpecifcVersion(
        //    [GraphQLNonNullType] string pageId,
        //    [GraphQLNonNullType] string mfeIdentifier,
        //    int version,
        //    [Service] IAuthoringService service)
        //{
        //    return await service.GetSpecificVersion(pageId, mfeIdentifier, version);
        //}

        //public async Task<List<ContentDto>> GetVersions(
        //    [GraphQLNonNullType] string pageId,
        //    [GraphQLNonNullType] string mfeIdentifier,
        //    [Service] IAuthoringService service)
        //{
        //    return await service.GetVersions(pageId, mfeIdentifier);
        //}

        //public async Task<ContentDto?> GetPublishedVersion(
        //    [GraphQLNonNullType] string pageId,
        //    [GraphQLNonNullType] string mfeIdentifier,
        //    [Service] IAuthoringService service)
        //{
        //    return await service.GetPublishedVersion(pageId, mfeIdentifier);
        //}

        //public async Task<ContentDto?> Get(
        //    [GraphQLNonNullType] string pageId,
        //    [GraphQLNonNullType] string mfeIdentifier,
        //    [Service] IAuthoringService service)
        //{
        //    return await service.Get(pageId, mfeIdentifier);
        //}
    }
}
