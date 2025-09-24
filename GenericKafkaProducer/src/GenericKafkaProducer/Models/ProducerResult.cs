namespace Icis.GenericKafkaProducer.Models
{
    using Confluent.Kafka;

    public class ProducerResult
    {
        public ProducerResult(TopicPartitionOffset topicPartitionOffset, PersistenceStatus status)
        {
            TopicPartitionOffset = topicPartitionOffset;
            Status = status;
        }

        public TopicPartitionOffset TopicPartitionOffset { get; }

        public PersistenceStatus Status { get; }
    }
}
