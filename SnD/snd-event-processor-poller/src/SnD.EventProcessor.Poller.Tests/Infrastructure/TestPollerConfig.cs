
using SnD.EventProcessor.Poller.Model;

namespace SnD.EventProcessor.Poller.Tests.Infrastructure
{
    public class TestPollerConfig
    {
        public static PollerConfig PollerConfigWithFiveSecondsInterval = new PollerConfig
        {
            MaxInitialDelayInSeconds = 1,
            IntervalInSeconds = 5,
            MessageBatchSize = 10
        };
    }
}
