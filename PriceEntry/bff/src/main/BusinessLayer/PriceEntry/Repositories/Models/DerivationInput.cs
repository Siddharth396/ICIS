namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class DerivationInput
    {
        [BsonElement("derivation_function_key")]
        public required string DerivationFunctionKey { get; set; }

        [BsonElement("parameter_index")]
        public required int ParameterIndex { get; set; }

        [BsonElement("parameter_base_weight")]
        public decimal? ParameterBaseWeight { get; set; }

        [BsonElement("series_id")]
        public required string SeriesId { get; set; }
    }
}
