namespace Infrastructure.Services.AuditInfoService
{
    using Infrastructure.MongoDB.Models;

    using Microsoft.Extensions.Internal;

    using Subscriber.Auth;

    public class AuditInfoService : IAuditInfoService
    {
        private readonly ISystemClock clock;

        private readonly IUserContext userContext;

        public AuditInfoService(ISystemClock clock, IUserContext userContext)
        {
            this.clock = clock;
            this.userContext = userContext;
        }

        public AuditInfo GetAuditInfoForCurrentUser()
        {
            return GetAuditInfoForUser(userContext.UserIdEncrypted);
        }

        public AuditInfo GetAuditInfoForUser(string user)
        {
            return new AuditInfo
            {
                Timestamp = clock.UtcNow.UtcDateTime,
                User = user,
            };
        }
    }
}
