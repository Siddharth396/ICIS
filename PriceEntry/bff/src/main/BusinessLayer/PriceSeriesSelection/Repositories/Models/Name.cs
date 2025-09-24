namespace BusinessLayer.PriceSeriesSelection.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Name
    {
        [BsonElement("en")]
        public string? English { get; set; } = default!;

        [BsonElement("zh")]
        public string? Chinese { get; set; } = default!;
    }
}
