namespace Authoring.Application.Region.Query
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Region;

    using Elastic.Apm.Api;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;

    using HotChocolate;
    using HotChocolate.Data;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType("Query")]
    public class RegionQueries
    {
        private readonly IErrorReporter errorReporter;

        public RegionQueries(IErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<RegionCC>> GetRegions(IResolverContext resolverContext, [Service] IRegionService service)
        {
            var regionData = await service.GetRegion();
            return regionData.Data!;
        }
    }
}