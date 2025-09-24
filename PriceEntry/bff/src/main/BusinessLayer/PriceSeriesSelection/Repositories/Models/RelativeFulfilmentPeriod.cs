namespace BusinessLayer.PriceSeriesSelection.Repositories.Models
{
    using BusinessLayer.PriceEntry.Repositories.Models;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class RelativeFulfilmentPeriod
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;

        [BsonElement("name")]
        public Name Name { get; set; } = default!;

        [BsonElement("type")]
        public PeriodType PeriodType { get; set; } = default!;
    }
}
