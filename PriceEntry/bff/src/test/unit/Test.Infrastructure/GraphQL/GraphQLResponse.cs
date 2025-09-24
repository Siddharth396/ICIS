namespace Test.Infrastructure.GraphQL
{
    public class GraphQLResponse<T>
    {
        public required T Data { get; set; }
    }
}