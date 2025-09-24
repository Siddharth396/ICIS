namespace Authoring.Application.Outage.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Outage;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;
    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType("Query")]
    public class OutageQueries
    {
        private readonly IErrorReporter errorReporter;

        public OutageQueries(IErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
        }

        public async Task<IEnumerable<Outage>> GetOutagesByCommoditiesAndRegions(
            [GraphQLNonNullType] string commodities,
            [GraphQLNonNullType] string regions,
            int? pageNo,
            int? pageSize,
            IResolverContext resolverContext,
            [Service] IOutageService service)
        {
            var outageData = await service.GetOutagesByCommoditiesAndRegions(commodities, regions, pageNo, pageSize);

            if (outageData.IsFailure)
            {
                errorReporter.ReportCustomError(
                    resolverContext,
                    outageData.Error?.Code ?? "Unknown Error",
                    outageData.Error?.Message ?? "An Unknown Error has occured");
            }

            return outageData.Data!;
        }
    }
}
