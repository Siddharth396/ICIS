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
    public class NonMarketAdjustmentResolver
    {
        [GraphQLName(Constants.ContentBlock.NonMarketAdjustmentEnabled)]
        public async Task<bool> GetNonMarketAdjustmentState(
            [Parent] ContentBlockDefinition contentBlockDefinition,
            [Service] IDataPackageMetadataDomainService dataPackageMetadataDomainService,
            IResolverContext resolverContext)
        {
            var assessedDateTime = resolverContext.GetAssessedDateTimeVariable();
            return await dataPackageMetadataDomainService.IsNonMarketAdjustmentEnabled(
                       new DataPackageKey(
                           contentBlockDefinition.ContentBlockId,
                           Version.From(contentBlockDefinition.Version),
                           assessedDateTime));
        }
    }
}
