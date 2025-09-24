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

        public string UserIdEncrypted => "dummy_encrypted_user_id";

        public string UserInfoUri { get; }

        public IEnumerable<string> Roles { get; private set; } = new List<string>() { "DUMMY_ROLE" };

        public Guid UserGuid => TestData.UserGuid;

        public string UserGuidString => TestData.UserGuid.ToString();

        public void SetFromRequestHeaders(HttpContext context)
        {
            // Do nothing but leave it empty as it will be executed by the UseSubscriberAuth middleware.
        }

        public void SetRoles(params string[] roles)
        {
            Roles = roles.ToList();
        }
    }
}