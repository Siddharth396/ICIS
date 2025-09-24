namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class VesselCapacity
    {
        [BsonElement("typical_size")]
        public decimal? TypicalSize { get; set; }

        [BsonElement("typical_size_min")]
        public decimal? TypicalSizeMin { get; set; }

        [BsonElement("typical_size_max")]
        public decimal? TypicalSizeMax { get; set; }
    }
}
