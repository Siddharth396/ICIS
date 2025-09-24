namespace BusinessLayer.PriceEntry.DTOs.Input
{
    using System;
    using System.Text.Json.Serialization;

    using BusinessLayer.Helpers;

    public class GasPricePayload
    {
        [JsonPropertyName("assessed_datetime")]
        public required long AssessedDateTimestamp { get; set; }

        [JsonPropertyName("fulfilment_from_datetime")]
        public required long FulfilmentFromTimestamp { get; set; }

        [JsonPropertyName("fulfilment_until_datetime")]
        public required long FulfilmentUntilTimestamp { get; set; }

        [JsonPropertyName("market_code")]
        public required string MarketCode { get; set; }

        [JsonPropertyName("mid")]
        public required decimal Mid { get; set; }

        public DateTime GetAssessedDateTime()
        {
            return AssessedDateTimestamp.FromUnixTimeMilliseconds();
        }

        public DateTime GetFulfilmentFrom()
        {
            return FulfilmentFromTimestamp.FromUnixTimeMilliseconds();
        }

        public DateTime GetFulfilmentUntil()
        {
            return FulfilmentUntilTimestamp.FromUnixTimeMilliseconds();
        }
    }
}
