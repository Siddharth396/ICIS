namespace Test.Infrastructure.Authoring.GraphQLClients.PriceEntry
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.DTOs.Input;

    using Test.Infrastructure.GraphQL;

    public class CommentaryGraphQLClient
    {
        private readonly GraphQLClient client;

        public CommentaryGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> SaveCommentary(CommentaryInput commentaryInput)
        {
            return client.SendAsync(
                GraphQLQueryBuilder
                   .New()
                   .SetQuery(Mutations.SaveCommentary)
                   .AddVariable("commentaryInput", commentaryInput)
                   .Build());
        }
    }
}
