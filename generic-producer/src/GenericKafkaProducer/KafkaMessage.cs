namespace KafkaProducer
{
    using System.Collections.Generic;

    public sealed class KafkaMessage
    {
        public string TopicName { get; set; }

        public string MessageKey { get; set; }

        public string MessageValue { get; set; }

        public string Schema { get; set; }

        public string PartitionKey { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public int PartitionCount { get; set; }
    }
}
