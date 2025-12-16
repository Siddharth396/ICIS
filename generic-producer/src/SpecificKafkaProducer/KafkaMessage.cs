namespace KafkaProducer
{
    using System.Collections.Generic;
    using Avro;
    using Avro.Specific;

    public sealed class KafkaMessage
    {
        public string TopicName { get; set; }

        public string MessageKey { get; set; }

        public ISpecificRecord MessageValue { get; set; }

        public string PartitionKey { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public int PartitionCount { get; set; }
    }
}
