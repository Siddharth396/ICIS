using SnD.EventProcessor.Poller.Model;

namespace SnD.EventProcessor.Poller.Contracts
{
    public interface IPollerConfigHelper
    {
        PollerConfig GetPollerConfig();
        string[] GetSnDQueueNames();
        SQSConfig GetSqsConfig();
        string GetSnDEventProcessorApiUrl();
        string GetSnDKafkaProducerApiUrl();
        AuthorizationRequestDetail GetDetailsForAuthorization();
        bool IsAuthorizationEnabled();
    }
}