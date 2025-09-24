namespace Test.Infrastructure.Authoring.GraphQLClients.PriceEntry
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.UserPreference.DTOs.Input;

    using Test.Infrastructure.GraphQL;

    public class UserPreferenceGraphQLClient
    {
        private readonly GraphQLClient client;

        public UserPreferenceGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> SaveUserPreference(UserPreferenceInput userPreferenceInput)
        {
            return client.SendAsync(
                     GraphQLQueryBuilder
                    .New()
                    .SetQuery(Mutations.SaveUserPreference)
                    .AddVariable("userPreferenceInput", userPreferenceInput)
                    .Build());
        }
    }
}
