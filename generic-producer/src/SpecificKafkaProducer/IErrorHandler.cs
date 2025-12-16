namespace KafkaProducer
{
    using Confluent.Kafka;

    public interface IErrorHandler<T>
    {
        void ErrorHandler(IProducer<string, T> producer, Error error);
    }
}