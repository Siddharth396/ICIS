namespace Infrastructure.GraphQL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using HotChocolate.Authorization;
    using HotChocolate.Resolvers;
    using Infrastructure.Services;

    [ExcludeFromCodeCoverage]
    public class AuthHandler : IAuthorizationHandler
    {
        private readonly IAuthService authService;

        public AuthHandler(IAuthService authService)
        {
            this.authService = authService;
        }

        public async ValueTask<AuthorizeResult> AuthorizeAsync(IMiddlewareContext context, AuthorizeDirective directive, CancellationToken cancellationToken = default(CancellationToken))
        {
            var isAuthorized = await authService.IsAuthorized(context, directive);

            return await Task.FromResult(((Func<AuthorizeResult>)(() =>
                                                                     {
                                                                         return isAuthorized ? AuthorizeResult.Allowed : AuthorizeResult.NotAllowed;
                                                                     }))());
        }

        public async ValueTask<AuthorizeResult> AuthorizeAsync(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives, CancellationToken cancellationToken = default(CancellationToken))
        {
            var isAuthorized = await authService.IsAuthorized(context, directives);

            return await Task.FromResult(((Func<AuthorizeResult>)(() =>
            {
                return isAuthorized ? AuthorizeResult.Allowed : AuthorizeResult.NotAllowed;
            }))());
        }
    }
}
