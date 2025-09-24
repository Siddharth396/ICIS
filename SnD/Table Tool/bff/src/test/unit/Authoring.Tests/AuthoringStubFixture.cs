namespace Authoring.Tests
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using Test.Infrastructure.GraphQL;
    using Test.Infrastructure.Stubs;

    public class AuthoringStubFixture : IDisposable
    {
        public AuthoringStubFixture()
        {
            App = AuthoringApp.Build();
            ServiceProvider = App.ServiceProvider;
            Executor = new QueryExecutor(App.ServiceProvider);
        }

        public AuthoringApp App { get; }

        public IServiceProvider ServiceProvider { get; private set; }

        public QueryExecutor Executor { get; private set; }

        public void Dispose()
        {
            ServiceProvider = null;
        }

        public T GetService<T>()
            where T : class =>
            ServiceProvider.GetRequiredService<T>();
    }
}
