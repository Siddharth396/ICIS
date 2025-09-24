namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class GasPriceSeriesItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("specification_name")]
        public required string SpecificationName { get; set; }

        [BsonElement("market_code")]
        public required string MarketCode { get; set; }

        [BsonElement("assessed_datetime")]
        public required DateTime AssessedDateTime { get; set; }

        [BsonElement("status")]
        public required string Status { get; set; }

        [BsonElement("period_label")]
        public required string PeriodLabel { get; set; }

        [BsonElement("fulfilment_from_datetime")]
        public required DateTime FulfilmentFromDate { get; set; }

        [BsonElement("fulfilment_until_datetime")]
        public required DateTime FulfilmentUntilDate { get; set; }

        [BsonElement("mid")]
        public required decimal Mid { get; set; }
    }
}
