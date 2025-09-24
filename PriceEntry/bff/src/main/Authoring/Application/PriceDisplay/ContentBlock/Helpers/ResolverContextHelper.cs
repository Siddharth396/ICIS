namespace Authoring.Application.PriceDisplay.ContentBlock.Helpers
{
    using Authoring.Application.PriceDisplay.ContentBlock.Query;

    using HotChocolate.Resolvers;

    public static class ResolverContextHelper
    {
        public static bool? GetBuiltFromInputParametersFromScopedContext(this IResolverContext resolverContext)
        {
            if (resolverContext.ScopedContextData.TryGetValue(ConstantsForDisplay.ScopedContext.IsBuiltFromInputParameters, out var isBuiltFromInputParameters))
            {
                return isBuiltFromInputParameters as bool?;
            }

            return false;
        }
    }
}
