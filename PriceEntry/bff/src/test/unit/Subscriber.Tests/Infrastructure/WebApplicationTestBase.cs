namespace Subscriber.Tests.Infrastructure
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.GraphQL;
    using Test.Infrastructure.Mongo;
    using Test.Infrastructure.Subscriber;

    using Xunit;

    public abstract class WebApplicationTestBase : IDisposable, IAsyncLifetime
    {
        private readonly Database database;

        protected WebApplicationTestBase(SubscriberBffApplicationFactory subscriberFactory, AuthoringBffApplicationFactory authoringFactory)
        {
            AuthoringHttpClient = authoringFactory.CreateDefaultClient();
            AuthoringGraphQLClient = new GraphQLClient(AuthoringHttpClient);

            SubscriberHttpClient = subscriberFactory.CreateDefaultClient();
            SubscriberGraphQLClient = new GraphQLClient(SubscriberHttpClient);

            database = authoringFactory.Services.GetService<Database>()!;
        }

        public GraphQLClient AuthoringGraphQLClient { get; }

        public GraphQLClient SubscriberGraphQLClient { get; }

        protected HttpClient AuthoringHttpClient { get; }

        protected HttpClient SubscriberHttpClient { get; }

        public void Dispose()
        {
            SubscriberHttpClient.Dispose();
            AuthoringHttpClient.Dispose();
        }

        public virtual async Task DisposeAsync()
        {
            await database.ResetNonReferenceDataCollections();
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
