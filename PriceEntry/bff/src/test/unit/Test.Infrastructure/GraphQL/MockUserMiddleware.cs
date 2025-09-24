namespace Test.Infrastructure.GraphQL
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HotChocolate.Execution;

    public class MockUserMiddleware
    {
        private readonly RequestDelegate next;

        public MockUserMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task InvokeAsync(IRequestContext context)
        {
            var claims = new List<Claim>();
            var mockPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            context.ContextData["ClaimsPrincipal"] = mockPrincipal;
            return next.Invoke(context).AsTask();
        }
    }
}
