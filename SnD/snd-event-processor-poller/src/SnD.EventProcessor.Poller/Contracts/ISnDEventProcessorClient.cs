using SnD.EventProcessor.Poller.Model;
using System.Net;
using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Contracts
{
    public interface ISnDEventProcessorClient
    {
        Task<(bool, HttpStatusCode)> PostEventToApiAsync(SnDEvent sndEvent, string correlationID);
    }
}
