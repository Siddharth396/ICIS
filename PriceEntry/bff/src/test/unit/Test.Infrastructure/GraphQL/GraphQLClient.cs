namespace Test.Infrastructure.GraphQL
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Threading.Tasks;

    public class GraphQLClient
    {
        private readonly HttpClient httpClient;

        public GraphQLClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        ///     Use this method the get the "data" field from the GraphQL response, and convert the data to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the "data" field should be converted.</typeparam>
        /// <param name="query">GraphQL query.</param>
        /// <returns>
        ///     An instance of <typeparamref name="T" /> that represents the "data" field from the GraphQL response.
        /// </returns>
        public async Task<T> SendAndGetDataFromResponseAs<T>(string query)
        {
            var response = await SendAsync(query);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<GraphQLResponse<T>>();

            return content!.Data;
        }

        /// <summary>
        ///     Use this method to send a GraphQL query and get the raw HTTP response.
        /// </summary>
        /// <param name="query">GraphQL query.</param>
        /// <returns>Raw HTTP response.</returns>
        public Task<HttpResponseMessage> SendAsync(string query)
        {
            var content = new StringContent(
                query,
                Encoding.UTF8,
                "application/json");
            return httpClient.PostAsync(
                "/v1/graphql",
                content);
        }
    }
}