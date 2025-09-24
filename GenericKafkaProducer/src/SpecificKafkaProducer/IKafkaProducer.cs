namespace KafkaProducer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    public interface IKafkaProducer<T> : IDisposable
    {
        Task<DeliveryResult<string, T>> ProduceAsync(KafkaMessage kafkaMessage, CancellationToken ct);
    }
}