namespace KafkaProducer.Sample
{
    using System;
    using Avro.Generic;
    using Confluent.Kafka;

    public class ErrorHandler : IErrorHandler
    {
        public void ErrorHandlerForGenericRecord(IProducer<string, GenericRecord> producer, Error error)
        {
            LogError(error);
        }

        public void ErrorHandlerForJson(IProducer<string, string> producer, Error error)
        {
            LogError(error);
        }

        private void LogError(Error error)
        {
            if (error.Code == ErrorCode.Local_AllBrokersDown)
            {
                Console.WriteLine("ErrorHandler: Kafka broker is down.");
            }
            else
            {
                Console.WriteLine($"ErrorHandler: {error}.");
            }
        }
    }
}