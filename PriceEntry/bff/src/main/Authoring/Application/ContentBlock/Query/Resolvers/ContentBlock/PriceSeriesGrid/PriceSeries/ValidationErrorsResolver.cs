namespace Authoring.Application.ContentBlock.Query.Resolvers.ContentBlock.PriceSeriesGrid.PriceSeries
{
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.PriceEntry.DTOs.Output;
    using BusinessLayer.PriceEntry.Validators;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType(typeof(PriceSeries))]
    public class ValidationErrorsResolver
    {
        // Based on the HotChocolate documentation, for the AnyType to work as expected, this is the return type that is expected
        // https://chillicream.com/docs/hotchocolate/v13/defining-a-schema/scalars/#any-type
        [GraphQLName(Constants.PriceSeries.ValidationErrorsField)]
        [GraphQLType<AnyType>]
        public IReadOnlyDictionary<string, object>? GetValidationErrors(
            [Parent] PriceSeries priceSeries,
            [GraphQLDescription(
                "Set this to 'true' to get the validation errors for price series items with status 'Not Started'")]
            bool includeNotStarted = false)
        {
            var validationResult = PriceSeriesValidator.Validate(priceSeries, includeNotStarted);
            if (validationResult.IsValid)
            {
                return null;
            }

            return validationResult.ValidationErrors.ToDictionary(x => x.Key, x => (object)x.Value);
        }
    }
}
