namespace Test.Infrastructure.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Subscriber.Auth;

    using Microsoft.AspNetCore.Http;

    using Test.Infrastructure.TestData;

    public class TestUserContext : IUserContext
    {
        public string AccessToken => "dummy_token_for_now";

        public string EmailDomain => "lexisnexisrisk.com";

        public bool IsInternalUser => true;

        public IEnumerable<string> Roles { get; private set; } = new List<string>();

        public Guid UserGuid => Guid.Empty;

        public string UserGuidString => string.Empty;

        public string UserId => string.Empty;

        public void SetFromRequestHeaders(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public void SetRoles(params string[] roles)
        {
            Roles = roles.ToList();
        }
    }
}