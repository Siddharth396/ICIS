namespace Subscriber.Tests.Infrastructure
{
    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Subscriber;

    using Xunit;

    [CollectionDefinition(Name)]
    public class WebApplicationCollection : ICollectionFixture<AuthoringBffApplicationFactory>,
                                            ICollectionFixture<SubscriberBffApplicationFactory>
    {
        public const string Name = "SubscriberWebApplication";
    }
}
