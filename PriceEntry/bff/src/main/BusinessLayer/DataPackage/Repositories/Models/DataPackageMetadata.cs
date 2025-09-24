namespace BusinessLayer.DataPackage.Repositories.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class DataPackageMetadata
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("data_package_id_metadata")]
        public required DataPackageIdMetadata DataPackageIdMetadata { get; set; }

        [BsonElement("non_market_adjustment_enabled")]
        public bool NonMarketAdjustmentEnabled { get; set; }
    }
}
