namespace Subscriber.Application.Capacity_Sub
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Capacity;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType("Query")]
    public class CapacityQueries
    {
        private readonly IErrorReporter errorReporter;

        public CapacityQueries(IErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
        }

        public async Task<IEnumerable<CapacityDevelopment>> GetCapacityDevelopmentsByCommoditiesAndRegions(
            [GraphQLNonNullType] string commodities,
            [GraphQLNonNullType] string regions,
            int? pageNo,
            int? pageSize,
            IResolverContext resolverContext,
            [Service] ICapacityDevelopmentService service)
        {
            var developments = await service.GetCapacityDevelopmentsByCommoditiesAndRegions(commodities, regions, pageNo, pageSize);

            if (developments.IsFailure)
            {
                errorReporter.ReportCustomError(
                    resolverContext,
                    developments.Error?.Code ?? "Unknown Error",
                    developments.Error?.Message ?? "An Unknown Error has occured");
            }

            return developments.Data!;
        }
    }
}
