namespace Subscriber.Tests.GraphQL
{
    using System.Threading.Tasks;

    using Snapshooter.Xunit;
    using Subscriber.Tests.Infrastructure;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Subscriber;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class GraphQLSchemaShould : WebApplicationTestBase
    {
        public GraphQLSchemaShould(
            SubscriberBffApplicationFactory subscriberFactory,
            AuthoringBffApplicationFactory authoringFactory)
            : base(subscriberFactory, authoringFactory)
        {
        }

        [Fact]
        public async Task Subscriber_Be_As_Expected()
        {
            var subscriberResponse = await SubscriberHttpClient.GetAsync("/v1/graphql?sdl");

            subscriberResponse.EnsureSuccessStatusCode();

            var subscriberSchema = await subscriberResponse.Content.ReadAsStringAsync();

            subscriberSchema.MatchSnapshot();
        }
    }
}
