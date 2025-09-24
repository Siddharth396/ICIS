namespace Infrastructure.GraphQL
{
    using HotChocolate.Resolvers;

    public interface IErrorReporter
    {
        void ReportCustomError(
            IResolverContext resolverContext,
            string code,
            string message);
    }
}
