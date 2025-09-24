namespace MongoDbSeeder.Data
{
    using System.Collections.Generic;

    using MongoDB.Bson;

    public static class PriceSeriesData
    {
        public static readonly IEnumerable<BsonDocument> LngJsonFile = Data.LoadDocuments("lng_price_series.json");
        public static readonly IEnumerable<BsonDocument> StyreneJsonFile = Data.LoadDocuments("styrene_price_series.json");
        public static readonly IEnumerable<BsonDocument> MelamineJsonFile = Data.LoadDocuments("melamine_price_series.json");
        public static readonly IEnumerable<BsonDocument> CharterRateJsonFile = Data.LoadDocuments("charter_rate_series.json");
        public static readonly IEnumerable<BsonDocument> CausticSodaJsonFile = Data.LoadDocuments("caustic_soda_price_series.json");
    }
}
