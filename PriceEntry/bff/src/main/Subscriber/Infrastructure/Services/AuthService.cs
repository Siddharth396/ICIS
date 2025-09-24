namespace Subscriber.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using global::Infrastructure.Services;

    using HotChocolate.Authorization;
    using HotChocolate.Resolvers;

    [ExcludeFromCodeCoverage]
    public class AuthService : IAuthService
    {
        public Task<bool> IsAuthorized(IMiddlewareContext context, AuthorizeDirective directive)
        {
            ////you can get the user context, and allowed roles for queries and take decision based on that
            ////var userContext = context.Service<IUserContext>();
            ////var allowedRoles = directive.Roles;

            return Task.FromResult(true);
        }

        public Task<bool> IsAuthorized(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives)
        {
            return Task.FromResult(true);
        }
    }
}
