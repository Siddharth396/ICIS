namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ColumnEditableWhen
    {
        [BsonElement("field")]
        public string Field { get; set; } = default!;

        [BsonElement("value")]
        public string Value { get; set; } = default!;
    }
}
