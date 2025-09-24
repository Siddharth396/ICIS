using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Contracts
{
    public interface ISQSMessageProcessor
    {
        Task DeleteMessageAsync(string queueUrl, string recieptHandle, CancellationToken cancellationToken);
        Task<string> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken);
        Task<List<Message>> ReceiveMessageAsync(string queueUrl, int messageBatchSize, CancellationToken cancellationToken);
    }
}
