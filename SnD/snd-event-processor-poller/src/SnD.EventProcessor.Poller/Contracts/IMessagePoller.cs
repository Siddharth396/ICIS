using System.Threading;
using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Contracts
{
    public interface IMessagePoller
    {
        Task PollQueuesAsync(CancellationToken cancellationToken);
    }
}
