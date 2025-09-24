namespace KafkaProducer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Avro.Generic;
    using Confluent.Kafka;
    using Icis.GenericKafkaProducer.Models;

    public interface IKafkaProducer : IDisposable
    {
        Task<ProducerResult> ProduceAsync(KafkaMessage kafkaMessage, CancellationToken ct);
    }
}