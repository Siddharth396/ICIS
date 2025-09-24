namespace SnD.EventProcessor.Poller.Model
{
    public class PollerConfig
    {
        public static string Name { get; private set; } = "PollerConfig";
        public int IntervalInSeconds { get; set; }
        public int MaxInitialDelayInSeconds { get; set; }
        public int MessageBatchSize { get; set; }
    }
}
