namespace Infrastructure.GraphQL
{
    using System.Diagnostics.CodeAnalysis;

    using HotChocolate.Resolvers;

    [ExcludeFromCodeCoverage]
    public class GraphQLErrorReporter : IErrorReporter
    {
        public void ReportCustomError(
            IResolverContext resolverContext,
            string code,
            string message)
        {
            resolverContext.ReportError(new CustomError(code, message));
        }
    }
}
