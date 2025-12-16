namespace SpecificKafkaProducer.Test
{
    using System.Threading;
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using KafkaProducer;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class KafkaProducerTest
    {
        private static readonly AvroSerializerConfig AvroSerializerConfig = new AvroSerializerConfig()
        {
            AutoRegisterSchemas = true
        };

        private static SchemaRegistryConfig schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = "http://localhost:30081",
            RequestTimeoutMs = 100,
            MaxCachedSchemas = 10,
            BasicAuthUserInfo = ""
        };

        private Mock<IErrorHandler<PriceSeries>> errorHandler;

        [Fact]
        public void ThrowError_When_KafkaBrokerDown()
        {
            errorHandler = new Mock<IErrorHandler<PriceSeries>>();
            var kafkaSpecificProducer = new KafkaProducer<PriceSeries>(Options.Create(GetKafkaProducerOptions()), errorHandler.Object);
            Thread.Sleep(5000);
            errorHandler.Verify(er => er.ErrorHandler(It.IsAny<IProducer<string, PriceSeries>>(), It.IsAny<Error>()), Times.AtLeastOnce);
        }

        private static KafkaProducerOptions GetKafkaProducerOptions()
        {
            var kafkaProducerOptions = new KafkaProducerOptions()
            {
                SchemaRegistry = schemaRegistryConfig,
                AvroSerializer = AvroSerializerConfig,
                BootstrapServers = "localhost:30091",
                EnableIdempotence = true,
                MessageTimeoutMs = 1
            };

            return kafkaProducerOptions;
        }
    }
}