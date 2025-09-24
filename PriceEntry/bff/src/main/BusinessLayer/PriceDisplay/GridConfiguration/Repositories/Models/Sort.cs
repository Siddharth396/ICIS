namespace BusinessLayer.PriceDisplay.GridConfiguration.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Sort
    {
        [BsonElement("name")]
        public string Name { get; set; } = default!;

        [BsonElement("order")]
        public string Order { get; set; } = default!;
    }
}
