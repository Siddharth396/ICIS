namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System;
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesAggregation
    {
        [BsonId]
        public string Id { get; set; } = default!;

        [BsonElement("series_name")]
        public Name SeriesName { get; set; } = default!;

        [BsonElement("series_short_name")]
        public Name? SeriesShortName { get; set; } = default!;

        [BsonElement("series_unique_name")]
        public Name? SeriesUniqueName { get; set; } = default!;

        [BsonElement("series_item_type_code")]
        public string SeriesItemTypeCode { get; set; } = default!;

        [BsonElement("commodity")]
        public Commodity Commodity { get; set; } = default!;

        [BsonElement("location")]
        public Location Location { get; set; } = default!;

        [BsonElement("assessed_location")]
        public Location? AssessedLocation { get; set; } = default!;

        [BsonElement("currency_unit")]
        public CurrencyUnit CurrencyUnit { get; set; } = default!;

        [BsonElement("measure_unit")]
        public MeasureUnit MeasureUnit { get; set; } = default!;

        [BsonElement("launch_date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime LaunchDate { get; set; } = default!;

        [BsonElement("termination_date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? TerminationDate { get; set; } = default!;

        [BsonElement("item_frequency")]
        public ItemFrequency ItemFrequency { get; set; } = default!;

        [BsonElement("ref_period")]
        public ReferencePeriod ReferencePeriod { get; set; } = default!;

        [BsonElement("price_category")]
        public PriceCategory PriceCategory { get; set; } = default!;

        [BsonElement("price_settlement_type")]
        public PriceSettlementType? PriceSettlementType { get; set; } = default!;

        [BsonElement("price_derivation_type")]
        public PriceDerivationType? PriceDerivationType { get; set; } = default!;

        [BsonElement("trade_terms")]
        public TradeTerms? TradeTerms { get; set; } = default!;

        [BsonElement("relative_fulfilment_period")]
        public RelativeFulfilmentPeriod? RelativeFulfilmentPeriod { get; set; } = default!;

        [BsonElement("derivation_inputs")]
        public List<DerivationInput> DerivationInputs { get; set; } = default!;

        [BsonElement("price_series_link")]
        public PriceSeriesLink? PriceSeriesLink { get; set; } = default!;

        [BsonElement("charter_horizon")]
        public CharterHorizon? CharterHorizon { get; set; } = default!;

        [BsonElement("charter_rate_code")]
        public string? CharterRateCode { get; set; } = default!;

        [BsonElement("vessel_capacity")]
        public VesselCapacity? VesselCapacity { get; set; } = default!;

        [BsonElement("vessel_engine_type")]
        public VesselEngineType? VesselEngineType { get; set; } = default!;

        [BsonElement("dlh_record_source")]
        public string DlhRecordSource { get; set; } = default!;

        [BsonElement("dlh_load_timestamp")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DlhLoadTimestamp { get; set; }

        [BsonElement("price_series_item")]
        public PriceSeriesItem PriceSeriesItem { get; set; } = default!;

        [BsonElement("last_assessments")]
        public List<PriceSeriesItem>? LastAssessments { get; set; } = default!;

        public PriceSeriesItem? LastAssessment { get; set; } = default!;

        [BsonElement("publication_schedules")]
        public List<PublicationSchedule>? PublicationSchedules { get; set; } = default!;

        [BsonElement("period_label_type_code")]
        public string PeriodLabelTypeCode { get; set; } = default!;
    }
}
