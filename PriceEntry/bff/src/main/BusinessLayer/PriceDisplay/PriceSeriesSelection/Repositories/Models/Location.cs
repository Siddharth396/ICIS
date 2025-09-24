namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories.Models
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Location
    {
        [BsonElement("guid")]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        [BsonElement("name")]
        public Name Name { get; set; } = default!;

        [BsonElement("region")]
        public Region Region { get; set; } = default!;
    }
}
