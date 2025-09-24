namespace Test.Infrastructure.GraphQL
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HotChocolate;
    using HotChocolate.Execution;

    using Microsoft.Extensions.DependencyInjection;

    public class QueryExecutor
    {
        private readonly IServiceProvider services;

        public QueryExecutor(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task<string> ExecuteAsync(string querySource, params (string Name, object Value)[] variables)
        {
            var requestBuilder = QueryRequestBuilder.New().SetQuery(querySource).SetServices(services);

            return await ExecuteInternalAsync(requestBuilder, variables);
        }

        public async Task<string> ExecuteAsyncWithScope(
            string querySource,
            params (string Name, object Value)[] variables)
        {
            var requestBuilder = QueryRequestBuilder.New()
               .SetQuery(querySource)
               .SetServices(services.CreateScope().ServiceProvider);

            return await ExecuteInternalAsync(requestBuilder, variables);
        }

        public async Task<string> ExecuteAsyncWithServiceProvider(
            string querySource,
            IServiceProvider serviceProvider,
            params (string Name, object Value)[] variables)
        {
            var requestBuilder = QueryRequestBuilder.New().SetQuery(querySource).SetServices(serviceProvider);

            return await ExecuteInternalAsync(requestBuilder, variables);
        }

        private async Task<string> ExecuteInternalAsync(
            IQueryRequestBuilder requestBuilder,
            params (string Name, object Value)[] variables)
        {
            foreach (var (name, value) in variables)
            {
                requestBuilder.AddVariableValue(name, value);
            }

            var mockPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), "mock"));
            requestBuilder.AddProperty("ClaimsPrincipal", mockPrincipal);
            requestBuilder.AddProperty("HttpContext", null);
            var request = requestBuilder.Create();

            var result = await services.ExecuteRequestAsync(request);
            return result.ToJson();
        }
    }
}
