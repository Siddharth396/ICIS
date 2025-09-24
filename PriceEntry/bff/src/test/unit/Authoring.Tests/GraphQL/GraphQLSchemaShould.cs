namespace Authoring.Tests.GraphQL
{
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class GraphQLSchemaShould : WebApplicationTestBase
    {
        public GraphQLSchemaShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Be_As_Expected()
        {
            var response = await HttpClient.GetAsync("/v1/graphql?sdl");

            response.EnsureSuccessStatusCode();

            var schema = await response.Content.ReadAsStringAsync();

            schema.MatchSnapshot();
        }
    }
}
