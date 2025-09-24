using Amazon.SQS;

namespace SnD.EventProcessor.Poller.Contracts
{
    public interface ISQSClientService
    {
        IAmazonSQS GetClient();
    }
}
