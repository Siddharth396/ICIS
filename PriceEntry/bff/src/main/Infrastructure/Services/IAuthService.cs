namespace Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HotChocolate.Authorization;
    using HotChocolate.Resolvers;

    public interface IAuthService
    {
        Task<bool> IsAuthorized(IMiddlewareContext context, AuthorizeDirective directive);

        Task<bool> IsAuthorized(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives);
    }
}
