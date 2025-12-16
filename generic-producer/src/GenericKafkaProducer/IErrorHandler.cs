namespace KafkaProducer
{
    using Avro.Generic;
    using Confluent.Kafka;

    public interface IErrorHandler
    {
        void ErrorHandlerForGenericRecord(IProducer<string, GenericRecord> producer, Error error);

        void ErrorHandlerForJson(IProducer<string, string> producer, Error error);
    }
}