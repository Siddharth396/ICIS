namespace Subscriber.Tests.Application.Domain
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using Snapshooter.Xunit;

    using Test.Infrastructure.GraphQL;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection("Subscriber stub collection")]
    public class QueryShould
    {
        private readonly QueryExecutor executor;

        public QueryShould(SubscriberStubFixture fixture)
        {
            executor = fixture.Executor;
        }

        //[Fact]
        //public async Task Return_PublishedVersion()
        //{
        //    // Act
        //    var result = await executor.ExecuteAsync(Queries.PublishedVersion, ("pageId", "commentary"), ("mfeIdentifier", "widget_0"));

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.MatchSnapshot();
        //}
    }
}
