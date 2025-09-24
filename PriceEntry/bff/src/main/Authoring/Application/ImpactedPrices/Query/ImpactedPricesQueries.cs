namespace Authoring.Application.ImpactedPrices.Query
{
    using System;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.Query;

    using BusinessLayer.ImpactedPrices.DTOs.Output;
    using BusinessLayer.ImpactedPrices.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Query)]
    public class ImpactedPricesQueries
    {
        [GraphQLName(Constants.ImpactedPrices.ImpactedPricesField)]
        public async Task<GetImpactedPricesResponse> GetImpactedPrices(
            [GraphQLNonNullType] string priceSeriesId,
            [GraphQLNonNullType] DateTime assessedDateTime,
            [Service] IImpactedPricesService impactedPricesService)
        {
            var impactedPrices = await impactedPricesService.GetImpactedPrices(priceSeriesId, assessedDateTime);
            return impactedPrices;
        }
    }
}
