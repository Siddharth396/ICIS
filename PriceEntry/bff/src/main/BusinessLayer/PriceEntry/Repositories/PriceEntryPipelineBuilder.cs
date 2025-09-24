namespace BusinessLayer.PriceEntry.Repositories
{
    using System;
    using System.Collections.Generic;

    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.Services.Workflow;

    using MongoDB.Bson;

    public static class PriceEntryPipelineBuilder
    {
        public static BsonDocument FilterPriceSeries(IEnumerable<string>? priceSeriesIds, DateTime assessedDateTime)
        {
            return new BsonDocument(
                "$match",
                new BsonDocument
                {
                    {
                        "$and",
                        new BsonArray
                        {
                            new BsonDocument("_id", new BsonDocument("$in", new BsonArray(priceSeriesIds))),
                            new BsonDocument
                            {
                                {
                                    "$or",
                                    new BsonArray
                                    {
                                       new BsonDocument("termination_date", new BsonDocument("$eq", BsonNull.Value)),
                                       new BsonDocument("termination_date", new BsonDocument("$gte", assessedDateTime.Date))
                                    }
                                }
                            }
                        }
                    }
                });
        }

        public static BsonDocument FilterPriceSeriesItem(DateTime assessedDateTime)
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
                            new BsonDocument("$match", new BsonDocument("assessed_datetime", assessedDateTime))
                        }
                    }
                });
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

        public static BsonDocument IncludeLastAssessments(DateTime assessedDateTime)
        {
            return new BsonDocument(
                "$lookup",
                new BsonDocument
                {
                    { "from", "price_series_items" },
                    {
                        "let",
                        new BsonDocument
                        {
                            { "series_id", "$_id" },
                            {
                                "linked_series_id",
                                new BsonDocument(
                                    "$cond",
                                    new BsonArray
                                    {
                                        new BsonDocument(
                                            "$eq",
                                            new BsonArray
                                            {
                                                "$price_series_link.series_link_reason_code",
                                                PriceSeriesLinkReasonCode.HasSubsequentAssessmentForSameFulfilmentPeriod.Value
                                            }),
                                        "$price_series_link.object_series_id",
                                        BsonNull.Value
                                    })
                            }
                        }
                    },
                    {
                        "pipeline",
                        new BsonArray
                        {
                            new BsonDocument(
                                "$match",
                                new BsonDocument(
                                    "$and",
                                    new BsonArray
                                    {
                                        new BsonDocument("$expr", new BsonDocument("$or", new BsonArray { new BsonDocument("$eq", new BsonArray { "$series_id", "$$series_id" }), new BsonDocument("$eq", new BsonArray { "$series_id", "$$linked_series_id" }) })),
                                        new BsonDocument("$expr", new BsonDocument("$lt", new BsonArray { "$assessed_datetime", assessedDateTime })),
                                        new BsonDocument("$or", new BsonArray { new BsonDocument("status", WorkflowStatus.Published.Value), new BsonDocument("status", WorkflowStatus.CorrectionPublished.Value) }),
                                    })),
                            new BsonDocument("$sort", new BsonDocument("assessed_datetime", -1)),
                            new BsonDocument("$limit", 2)
                        }
                    },
                    { "as", "last_assessments" }
                });
        }

        public static BsonDocument Unwind(string path)
        {
            return new BsonDocument("$unwind", new BsonDocument { { "path", path }, { "preserveNullAndEmptyArrays", true } });
        }
    }
}