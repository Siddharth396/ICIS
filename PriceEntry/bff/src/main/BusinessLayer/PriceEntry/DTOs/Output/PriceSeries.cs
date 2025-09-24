namespace BusinessLayer.PriceEntry.DTOs.Output
{
    using System;

    using BusinessLayer.PriceEntry.Validators;
    using HotChocolate;

    public class PriceSeries : ISingleValueWithReferenceValidationData,
                               IRangeValidationData,
                               ISingleValueValidationData,
                               ICharterRateSingleValueValidationData
    {
        public string Id { get; set; }

        public string SeriesName { get; set; } = default!;

        public string? SeriesShortName { get; set; } = default!;

        public string SeriesItemTypeCode { get; set; } = default!;

        public Commodity Commodity { get; set; } = default!;

        public Location Location { get; set; } = default!;

        public CurrencyUnit CurrencyUnit { get; set; } = default!;

        public MeasureUnit MeasureUnit { get; set; } = default!;

        public DateOnly LaunchDate { get; set; } = default!;

        public DateOnly? TerminationDate { get; set; }

        public RelativeFulfilmentPeriod? RelativeFulfilmentPeriod { get; set; } = default!;

        public string PriceSeriesName { get; set; } = default!;

        public string UnitDisplay { get; set; } = default!;

        public string SeriesItemId { get; set; } = default!;

        public string SeriesId { get; set; } = default!;

        public DateTime? AssessedDateTime { get; set; }

        public string? DataUsed { get; set; } = default!;

        public decimal? Price { get; set; } = default!;

        public decimal? PriceLow { get; set; } = default!;

        public decimal? PriceHigh { get; set; } = default!;

        public decimal? PriceMid { get; set; } = default!;

        public decimal? AdjustedPriceDelta { get; set; }

        public decimal? AdjustedPriceHighDelta { get; set; }

        public decimal? AdjustedPriceLowDelta { get; set; }

        public decimal? AdjustedPriceMidDelta { get; set; }

        public string? LastAssessmentDate { get; set; } = default!;

        public string? LastAssessmentPrice { get; set; } = default!;

        public decimal? LastAssessmentPriceValue { get; set; } = default!;

        public decimal? LastAssessmentPriceLowValue { get; set; } = default!;

        public decimal? LastAssessmentPriceHighValue { get; set; } = default!;

        public decimal? LastAssessmentPriceMidValue { get; set; } = default!;

        [GraphQLIgnore]
        public string LastAssessmentPeriodLabel { get; set; } = default!;

        public decimal? PriceDelta { get; set; } = default!;

        public string? AssessmentMethod { get; set; } = default!;

        public ReferencePrice? ReferencePrice { get; set; }

        public decimal? PremiumDiscount { get; set; }

        public decimal? PriceLowDelta { get; set; } = default!;

        public decimal? PriceMidDelta { get; set; } = default!;

        public decimal? PriceHighDelta { get; set; } = default!;

        public string? Status { get; set; } = default!;

        public bool IsDerivedPriceSeries { get; set; } = default!;

        public string? LastAssessmentReferenceMarket { get; set; } = default!;

        public string? LastAssessmentPremiumDiscount { get; set; } = default!;

        public string PeriodLabelTypeCode { get; set; } = default!;

        public ReferencePeriod ReferencePeriod { get; set; } = default!;

        public DateTime? LastAssessmentAppliesFromDateTime { get; set; } = default!;

        [GraphQLIgnore]
        public decimal? ReferencePriceValue => ReferencePrice?.Price;

        public string? PublicationScheduleId { get; set; }
    }
}
