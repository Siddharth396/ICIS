namespace KafkaProducer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Avro.Specific;

    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;

    using Microsoft.Extensions.Options;

    using Serilog;

    public sealed class KafkaProducer<T> : IKafkaProducer<T> where T : ISpecificRecord
    {
        private readonly IProducer<string, T> avroProducer;
        private readonly ISchemaRegistryClient schemaRegistry;

        public KafkaProducer(IOptions<KafkaProducerOptions> options)
        {
            schemaRegistry = new CachedSchemaRegistryClient(options.Value.SchemaRegistry);

            avroProducer = new ProducerBuilder<string, T>(options.Value)
               .SetErrorHandler((p, error) =>
               {
                   Log.Error(error.ToString());
               })
               .SetLogHandler((p, message) =>
               {
                   Log.Information(message.ToString());
               })
               .SetKeySerializer(Serializers.Utf8)
               .SetValueSerializer(new AvroSerializer<T>(schemaRegistry, options.Value.AvroSerializer))
               .Build();
        }

        /// <summary>
        /// To enable circuit breaker pattern the consuming application if implements 
        /// the IErrorHandler Interfaces and registers in the DI, 
        /// the circuit breaker constructor will be called where 
        /// the consuming application has to handle the errors in the way desired
        /// </summary>
        /// <param name="options">Otions to connect kafka broker</param>
        /// <param name="errorHandler">To handle errors when kafka broker is down.</param>
        public KafkaProducer(IOptions<KafkaProducerOptions> options, IErrorHandler<T> errorHandlerDelegate)
        {
            schemaRegistry = new CachedSchemaRegistryClient(options.Value.SchemaRegistry);

            avroProducer = new ProducerBuilder<string, T>(options.Value)
               .SetErrorHandler(errorHandlerDelegate.ErrorHandler)
               .SetLogHandler((p, message) =>
               {
                   Log.Information(message.ToString());
               })
               .SetKeySerializer(Serializers.Utf8)
               .SetValueSerializer(new AvroSerializer<T>(schemaRegistry, options.Value.AvroSerializer))
               .Build();
        }

        /// <summary>
        /// Method to produce specific kafka message
        /// </summary>
        /// <param name="kafkaMsg"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<DeliveryResult<string, T>> ProduceAsync(KafkaMessage kafkaMessage, CancellationToken ct)
        {
            var message = new Message<string, T>
            {
                Key = kafkaMessage.MessageKey,
                Value = (T)kafkaMessage.MessageValue
            };

            if (kafkaMessage.Headers?.Count > 0)
            {
                message.Headers = new Headers();

                foreach (var header in kafkaMessage.Headers)
                {
                    message.Headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
                }
            }

            AppendTraceparentInHeader(message.Headers);

            if (!string.IsNullOrWhiteSpace(kafkaMessage.PartitionKey) && kafkaMessage.PartitionCount > 0)
            {
                var partition = Math.Abs(kafkaMessage.PartitionKey.GetHashCode()) % kafkaMessage.PartitionCount;

                var data = await avroProducer.ProduceAsync(new TopicPartition(kafkaMessage.TopicName, partition), message, ct);

                return data;
            }
            else
            {
                return await avroProducer.ProduceAsync(kafkaMessage.TopicName, message, ct);
            }
        }

        public void Dispose()
        {
            avroProducer?.Dispose();
            schemaRegistry?.Dispose();
        }

        private void AppendTraceparentInHeader(Headers messageheaders)
        {
            messageheaders ??= new Headers();

            var dtd = Elastic.Apm.Agent.Tracer?.CurrentTransaction?.OutgoingDistributedTracingData;

            if (dtd != null && !messageheaders.Any(t => t.Key == "traceparent"))
            {
                messageheaders.Add("traceparent", Encoding.UTF8.GetBytes(dtd.SerializeToString()));
            }
        }
    }
}