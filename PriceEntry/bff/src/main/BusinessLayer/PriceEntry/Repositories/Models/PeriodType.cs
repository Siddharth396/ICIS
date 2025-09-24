namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PeriodType
    {
        [BsonElement("name")]
        public Name Name { get; set; } = default!;

        [BsonElement("code")]
        public string Code { get; set; } = default!;

        [BsonElement("has_relative_def_perspective")]
        public bool HasRelativeDefPerspective { get; set; }
    }
}
