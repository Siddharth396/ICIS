namespace KafkaProducer
{
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;

    public class KafkaProducerOptions : ProducerConfig
    {
        public SchemaRegistryConfig SchemaRegistry { get; set; }

        public AvroSerializerConfig AvroSerializer { get; set; }
    }
}
