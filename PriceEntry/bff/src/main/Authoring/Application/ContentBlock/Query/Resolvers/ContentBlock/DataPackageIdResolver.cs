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
    public class DataPackageIdResolver
    {
        [GraphQLName(Constants.ContentBlock.DataPackageId)]
        public async Task<string> GetDataPackageId(
            [Parent] ContentBlockDefinition contentBlockDefinition,
            [Service] IDataPackageMetadataDomainService dataPackageMetadataDomainService,
            IResolverContext resolverContext)
        {
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();

            var dataPackageId = await dataPackageMetadataDomainService.GetDataPackageId(
                                    new DataPackageKey(
                                        contentBlockDefinition.ContentBlockId,
                                        Version.From(contentBlockDefinition.Version),
                                        assessedDateTime));

            return dataPackageId.Value;
        }
    }
}
