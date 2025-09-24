namespace Authoring.Tests.Functional.Tests.Common.Infrastructure
{
    using Test.Infrastructure.Authoring;

    using Xunit;

    [CollectionDefinition(WebApplicationCollection.Name)]
    public class WebApplicationCollection : ICollectionFixture<AuthoringBffApplicationFactory>
    {
        public const string Name = "WebApplication";
    }
}
