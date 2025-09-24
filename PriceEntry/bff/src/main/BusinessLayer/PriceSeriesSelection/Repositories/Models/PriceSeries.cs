namespace BusinessLayer.PriceSeriesSelection.Repositories.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    using PriceDerivationType = BusinessLayer.PriceEntry.Repositories.Models.PriceDerivationType;

    [BsonIgnoreExtraElements]
    public class PriceSeries
    {
        private static readonly Guid AsiaEastLocationGuid = new("f5705134-98a1-487d-ba79-399438c34214");

        [BsonId]
        public string Id { get; set; } = default!;

        [BsonElement("series_name")]
        public Name SeriesName { get; set; } = default!;

        [BsonElement("series_short_name")]
        public Name? SeriesShortName { get; set; } = default!;

        [BsonElement("commodity")]
        public Commodity Commodity { get; set; } = default!;

        [BsonElement("relative_fulfilment_period")]
        public RelativeFulfilmentPeriod? RelativeFulfilmentPeriod { get; set; } = default!;

        [BsonElement("ref_period")]
        public ReferencePeriod? ReferencePeriod { get; set; } = default!;

        [BsonElement("series_item_type_code")]
        public string SeriesItemTypeCode { get; set; } = default!;

        [BsonElement("currency_unit")]
        public CurrencyUnit CurrencyUnit { get; set; } = default!;

        [BsonElement("measure_unit")]
        public MeasureUnit MeasureUnit { get; set; } = default!;

        [BsonElement("item_frequency")]
        public ItemFrequency ItemFrequency { get; set; } = default!;

        [BsonElement("price_category")]
        public PriceCategory PriceCategory { get; set; } = default!;

        [BsonElement("location")]
        public Location Location { get; set; } = default!;

        [BsonElement("derivation_inputs")]
        public List<DerivationInput>? DerivationInputs { get; set; } = default!;

        [BsonElement("price_derivation_type")]
        public PriceDerivationType? PriceDerivationType { get; set; } = default!;

        [BsonElement("price_settlement_type")]
        public PriceSettlementType? PriceSettlementType { get; set; } = default!;

        [BsonElement("price_series_link")]
        public List<PriceSeriesLink>? PriceSeriesLinks { get; set; } = default!;

        [BsonElement("trade_terms")]
        public TradeTerms? TradeTerms { get; set; } = default!;

        [BsonElement("termination_date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? TerminationDate { get; set; } = default!;

        [BsonElement("publication_schedules")]
        public List<PublicationSchedule>? PublicationSchedules { get; set; } = default!;

        [BsonElement("launch_date")]
        public DateTime LaunchDate { get; set; }

        [BsonElement("period_label_type_code")]
        public string? PeriodLabelTypeCode { get; set; } = default!;

        public static bool IsValidDerivedSeries(PriceSeries derivedSeries)
        {
            return PriceCategoryCode.Derived.Matches(derivedSeries.PriceCategory.Code)
                   && derivedSeries.PriceDerivationType is not null
                   && BusinessLayer.PriceEntry.ValueObjects.PriceDerivationType.Index.Matches(derivedSeries.PriceDerivationType.Code)
                   && derivedSeries.DerivationInputs is not null
                   && derivedSeries.DerivationInputs.Any(x =>
                       x.DerivationFunctionKey is DerivationFunctionKey.PeriodAvg
                       || (x.DerivationFunctionKey is DerivationFunctionKey.RegionalAvg && derivedSeries.Location.Guid == AsiaEastLocationGuid));
        }

        public PriceSeriesLink? GetPriceSeriesLink() =>
            PriceSeriesLinks?.FirstOrDefault(
                            x => PriceSeriesLinkReasonCode
                               .HasSubsequentAssessmentForSameFulfilmentPeriod
                               .Matches(x.ReasonCode));

        public bool? HasRelativeDefPerspective() => RelativeFulfilmentPeriod?.PeriodType.HasRelativeDefPerspective;
    }
}
