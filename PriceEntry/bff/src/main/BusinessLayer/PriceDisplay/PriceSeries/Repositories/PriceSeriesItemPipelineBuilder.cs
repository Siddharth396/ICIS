namespace BusinessLayer.PriceDisplay.PriceSeries.Repositories
{
    using System;
    using System.Collections.Generic;

    using Infrastructure.Services.Workflow;

    using MongoDB.Bson;

    public class PriceSeriesItemPipelineBuilder
    {
        public static BsonDocument FilterPriceSeriesByIds(IEnumerable<string> seriesId)
        {
            return new BsonDocument(
                "$match",
                new BsonDocument
                {
                    { "_id", new BsonDocument("$in", new BsonArray(seriesId)) }
                });
        }

        public static BsonDocument FilterPriceSeriesByIdsAndLaunchDate(IEnumerable<string> seriesId, DateTime launchDate)
        {
            return new BsonDocument(
                "$match",
                new BsonDocument
                {
                    { "_id", new BsonDocument("$in", new BsonArray(seriesId)) },
                    { "launch_date", new BsonDocument("$lte", launchDate) }
                });
        }

        public static BsonDocument GetPriceSeries(DateTime assessedDateTime)
        {
            var matchCondition = new BsonDocument
            {
                { "assessed_datetime", new BsonDocument("$eq", assessedDateTime) }
            };
            return GetPriceSeries(matchCondition);
        }

        public static BsonDocument GetPublishedPriceSeries(DateTime assessedDateTime)
        {
            var matchConditions = new BsonDocument
            {
                { "status", new BsonDocument("$in", new BsonArray { WorkflowStatus.Published.Value, WorkflowStatus.CorrectionPublished.Value }) },
                { "assessed_datetime", new BsonDocument("$lte", assessedDateTime) }
            };

            return GetPriceSeries(matchConditions);
        }

        public static BsonDocument Unwind(string path)
        {
            return new BsonDocument("$unwind", new BsonDocument { { "path", path }, { "preserveNullAndEmptyArrays", true } });
        }

        public static BsonDocument HandlePendingChanges()
        {
            return new BsonDocument(
                "$set",
                new BsonDocument
                {
                    { "price_series_item", new BsonDocument("$ifNull", new BsonArray { "$price_series_item.pending_changes", "$price_series_item" }) }
                });
        }

        public static BsonDocument GetPriceSeries()
        {
            return new BsonDocument(
                "$lookup",
                new BsonDocument
                {
                    { "from", "price_series_items" },
                    { "localField", "_id" },
                    { "foreignField", "series_id" },
                    { "as", "price_series_item" },
                    {
                        "pipeline",
                        new BsonArray
                        {
                            new BsonDocument("$sort", new BsonDocument("assessed_datetime", -1)),
                            new BsonDocument("$limit", 1)
                        }
                    }
                });
        }

        public static BsonDocument GetPriceDeltaType(BsonDocument matchConditions)
        {
            return new BsonDocument(
                "$lookup",
                new BsonDocument
                {
                    { "from", "price_delta_types" },
                    { "localField", "price_series_item.price_delta_type_guid" },
                    { "foreignField", "guid" },
                    { "as", "delta_type" }
                });
        }

        private static BsonDocument GetPriceSeries(BsonDocument matchConditions)
        {
            return new BsonDocument(
                "$lookup",
                new BsonDocument
                {
                    { "from", "price_series_items" },
                    { "localField", "_id" },
                    { "foreignField", "series_id" },
                    { "as", "price_series_item" },
                    {
                        "pipeline",
                        new BsonArray
                        {
                            new BsonDocument("$match", matchConditions),
                            new BsonDocument("$sort", new BsonDocument("assessed_datetime", -1)),
                            new BsonDocument("$limit", 1)
                        }
                    }
                });
        }
    }
}
