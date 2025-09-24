namespace Authoring.Tests.Functional.Tests.Common.Infrastructure
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.GraphQL;
    using Test.Infrastructure.Mongo;

    using Xunit;

    public abstract class WebApplicationTestBase : IDisposable, IAsyncLifetime
    {
        private readonly Database database;

        private readonly AuthoringBffApplicationFactory factory;

        protected WebApplicationTestBase(AuthoringBffApplicationFactory factory)
        {
            this.factory = factory;
            HttpClient = factory.CreateDefaultClient();
            GraphQLClient = new GraphQLClient(HttpClient);

            database = factory.Services.GetService<Database>()!;
        }

        public GraphQLClient GraphQLClient { get; }

        protected HttpClient HttpClient { get; }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        public virtual async Task DisposeAsync()
        {
            await database.ResetNonReferenceDataCollections();
        }

        public T GetService<T>()
            where T : notnull
        {
            return factory.Services.GetRequiredService<T>();
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
