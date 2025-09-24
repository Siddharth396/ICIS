namespace Authoring.Tests.Application.Domain
{
    using System.Threading.Tasks;

    using FluentAssertions;

    using Snapshooter.Xunit;

    using Test.Infrastructure.GraphQL;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection("Authoring stub collection")]
    public class QueryShould
    {
        private readonly QueryExecutor executor;

        public QueryShould(AuthoringStubFixture fixture)
        {
            executor = fixture.Executor;
        }

        //[Fact]
        //public async Task Return_Versions()
        //{
        //    // Act
        //    var result = await executor.ExecuteAsync(Queries.GetVersions, ("pageId", "commentary"), ("mfeIdentifier", "widget_0"));

        //    // Assert
        //    result.Should().NotBeNull();
        //    //result.Should().BeNull();
        //    result.MatchSnapshot();
        //}

        //[Fact]
        //public async Task Return_SpecifcVersion()
        //{
        //    // Act
        //    var result = await executor.ExecuteAsync(Queries.SpecifcVersion, ("pageId", "commentary"), ("mfeIdentifier", "widget_0"), ("version", 2));

        //    result.Should().NotBeNull();
        //    result.MatchSnapshot();
        //}

        //[Fact]
        //public async Task Return_PublishedVersion()
        //{
        //    // Act
        //    var result = await executor.ExecuteAsync(Queries.PublishedVersion, ("pageId", "commentary"), ("mfeIdentifier", "widget_0"));

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.MatchSnapshot();
        //}

        //[Fact]
        //public async Task Return_Content()
        //{
        //    // Act
        //    var result = await executor.ExecuteAsync(Queries.Get, ("pageId", "commentary"), ("mfeIdentifier", "widget_0"));

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.MatchSnapshot();
        //}

        //[Fact]
        //public async Task Return_Null_Content()
        //{
        //    // Act
        //    var result = await executor.ExecuteAsync(Queries.Get, ("pageId", "not_existt"), ("mfeIdentifier", "not_existt"));

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.MatchSnapshot();
        //}
    }
}
