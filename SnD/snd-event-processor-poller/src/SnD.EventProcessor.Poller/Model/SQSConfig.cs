namespace SnD.EventProcessor.Poller.Model
{
    public class SQSConfig
    {        
        public static string Name { get; private set; } = "SQSConfig";
        public string SQSAccessKey { get; set; }
        public string SQSSecret { get; set; }
        public string SQSRegion { get; set; }
        public int MessageWaitTime { get; set; }
        public int MessageVisibilityTimeout { get; set; }
        public int RequestTimeOutInSeconds { get; set; }
        public int MaxRetryCount { get; set; }
    }
}
