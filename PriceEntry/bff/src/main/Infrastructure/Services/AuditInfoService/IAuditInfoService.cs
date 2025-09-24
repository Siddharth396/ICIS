namespace Infrastructure.Services.AuditInfoService
{
    using Infrastructure.MongoDB.Models;

    public interface IAuditInfoService
    {
        AuditInfo GetAuditInfoForCurrentUser();

        AuditInfo GetAuditInfoForUser(string user);
    }
}
