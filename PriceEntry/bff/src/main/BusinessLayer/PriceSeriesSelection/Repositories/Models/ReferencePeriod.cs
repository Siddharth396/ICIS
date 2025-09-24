namespace BusinessLayer.PriceSeriesSelection.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ReferencePeriod
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;

        [BsonElement("name")]
        public Name Name { get; set; } = default!;
    }
}
