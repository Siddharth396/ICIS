namespace Authoring.Infrastructure.Services
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using global::Infrastructure.Services;

    using HotChocolate.Authorization;
    using HotChocolate.Resolvers;

    public class AuthService : IAuthService
    {
        public async Task<bool> IsAuthorized(IMiddlewareContext context, AuthorizeDirective directive)
        {
            ////you can get the user context, and allowed roles for queries and take decision based on that
            ////var userContext = context.Service<IUserContext>();
            ////var allowedRoles = directive.Roles;

            return true;
        }

        public async Task<bool> IsAuthorized(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives)
        {
            return false;
        }

    }
}
