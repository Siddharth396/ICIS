using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Contracts
{
    public interface IAuthorizationService
    {
        Task<string> GetBearerTokenAsync(string correlationID);
    }
}