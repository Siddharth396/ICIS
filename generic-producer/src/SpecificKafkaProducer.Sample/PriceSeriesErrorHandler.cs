namespace KafkaProducer
{
    using System;
    using Confluent.Kafka;
    using SpecificProducer.Sample;

    public class PriceSeriesErrorHandler : IErrorHandler<PriceSeries>
    {
        public void ErrorHandler(IProducer<string, PriceSeries> producer, Error error)
        {
            if (error.Code == ErrorCode.Local_AllBrokersDown)
            {
                Console.WriteLine("PriceSeriesErrorHandler: Kafka broker is down.");
            }
            else
            {
                Console.WriteLine($"PriceSeriesErrorHandler: {error.ToString()}.");
            }
        }
    }
}