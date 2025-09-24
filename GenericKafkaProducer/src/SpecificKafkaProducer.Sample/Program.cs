namespace SpecificProducer.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using KafkaProducer;
    using Microsoft.Extensions.Options;
    using Serilog;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            Console.WriteLine("Welcome to Generic Kafka Producer Sample application!");

            try
            {
                IKafkaProducer<PriceSeries> kafkaSpecificProducer = new KafkaProducer<PriceSeries>(Options.Create(GetKafkaProducerOptions()));

                int count = 10;

                for (int i = 0; i <= count; i++)
                {
                    var result = await kafkaSpecificProducer.ProduceAsync(GetSampleKafkaMessage(), CancellationToken.None);
                    Console.WriteLine("Published message in Kafka - " + i);
                    Console.WriteLine("Partition: " + result.Partition.Value);
                }

                kafkaSpecificProducer = new KafkaProducer<PriceSeries>(Options.Create(GetKafkaProducerOptions()), new PriceSeriesErrorHandler());

                // To Test the circuit breaker, need to turn off the broker from docker while the loop is running.
                for (int i = 0; i < 100; i++)
                {
                    var errorResult = await kafkaSpecificProducer.ProduceAsync(GetSampleKafkaMessage(), CancellationToken.None);
                    Console.WriteLine("Published message in Kafka - " + i);
                    Console.WriteLine("Partition: " + errorResult.Partition.Value);
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Method to initialize Specific Class
        /// </summary>
        /// <returns></returns>
        private static SpecificTestClass GetSpecificTestClass()
        {
            var test = new SpecificTestClass()
            {
                Id = "TestId",
                SeriesId = "TestSeries",
                PriceValue = 12.98,
                Date = DateTime.Now
            };
            return test;
        }

        /// <summary>
        /// Method to populate Test PriceSeries data
        /// </summary>
        /// <returns></returns>
        private static PriceSeries GetSpecificPriceSeries()
        {
            var priceSeries = new PriceSeries
            {
                Id = Guid.NewGuid().ToString(),
                CoorelationId = Guid.NewGuid().ToString(),
                Source = "TestSource",
                Type = "PricesPublished",
                EventType = "Published",
                EventTime = DateTimeOffset.Now.UtcDateTime,
                CreatedFor = DateTimeOffset.Now.UtcDateTime,
                ReleasedOn = DateTimeOffset.Now.UtcDateTime.AddDays(1),
                VersionNumber = 1,
                VersionDate = DateTimeOffset.Now.Date,
                SeriesId = "4003027",
                SeriesVersionNumber = 1,
                CreatedOn = DateTimeOffset.Now.UtcDateTime,
                EmbargoDate = DateTimeOffset.Now.UtcDateTime.AddDays(1),
                Commodity = "butadiene",
                Region = "europe north west",
                Currency = "EUR",
                MeasureUnit = "MT",
                SeriesOrder = DateTimeOffset.Now.Date.Subtract(TimeSpan.FromDays(12)),
                Low = 720.0000,
                Mid = 740.0000,
                High = 760.0000
            };
            return priceSeries;
        }

        /// <summary>
        /// Method to populate sample kafka Message
        /// </summary>
        /// <returns></returns>
        private static KafkaMessage GetSampleKafkaMessage()
        {
            var headers = new Dictionary<string, string>
            {
                { "correlation_id", Guid.NewGuid().ToString() }
            };

            var kafkaMessage = new KafkaMessage()
            {
                TopicName = "TestSpecificTopic",
                Headers = headers,
                MessageKey = $"Test_{Guid.NewGuid()}_Published",
                MessageValue = GetSpecificPriceSeries(),
                PartitionCount = 1,
            };
            return kafkaMessage;
        }

        /// <summary>
        /// Method to populate Schema registry and Avro serializer config
        /// </summary>
        /// <returns></returns>
        private static KafkaProducerOptions GetKafkaProducerOptions()
        {
            var kafkaProducerOptions = new KafkaProducerOptions()
            {
                SchemaRegistry = schemaRegistryConfig,
                AvroSerializer = AvroSerializerConfig,
                BootstrapServers = "localhost:30092",
                EnableIdempotence = true,
                MessageTimeoutMs = 60000
            };

            return kafkaProducerOptions;
        }

        private static readonly AvroSerializerConfig AvroSerializerConfig = new AvroSerializerConfig()
        {
            AutoRegisterSchemas = true
        };

        private static SchemaRegistryConfig schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = "http://localhost:30081",
            RequestTimeoutMs = 5000,
            MaxCachedSchemas = 10,
            BasicAuthUserInfo = ""
        };
    }
}