namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class MeasureUnit
    {
        [BsonElement("guid")]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        [BsonElement("name")]
        public Name Name { get; set; } = default!;

        [BsonElement("symbol")]
        public string Symbol { get; set; } = default!;
    }
}
