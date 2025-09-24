namespace BusinessLayer.PriceEntry.AutoMapper
{
    using System;
    using System.Globalization;
    using System.Linq;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using global::AutoMapper;

    using Infrastructure.Services.Workflow;

    public class PriceEntryMappingProfile : Profile
    {
        public PriceEntryMappingProfile()
        {
            CreateMap<Sort, DTOs.Output.Sort>();

            CreateMap<Column, DTOs.Output.Column>()
                .ForMember(dest => dest.ColumnOrder, exp => exp.MapFrom(src => src.DisplayOrder));

            CreateMap<GridConfiguration, DTOs.Output.GridConfiguration>();

            CreateMap<Commodity, DTOs.Output.Commodity>()
               .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name.English));

            CreateMap<CurrencyUnit, DTOs.Output.CurrencyUnit>()
               .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name.English));

            CreateMap<PeriodType, DTOs.Output.PeriodType>()
               .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name.English));

            CreateMap<RelativeFulfilmentPeriod, DTOs.Output.RelativeFulfilmentPeriod>()
               .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name.English));

            CreateMap<Location, DTOs.Output.Location>()
               .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name.English))
               .ForMember(dest => dest.Region, exp => exp.MapFrom(src => src.Region.Name.English));

            CreateMap<MeasureUnit, DTOs.Output.MeasureUnit>()
               .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name.English));

            CreateMap<ReferencePrice, DTOs.Output.ReferencePrice>();

            CreateMap<ReferencePeriod, DTOs.Output.ReferencePeriod>()
                .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name.English));

            CreateMap<PriceSeriesAggregation, DTOs.Output.PriceSeries>()
               .ForMember(dest => dest.SeriesName, exp => exp.MapFrom(src => src.SeriesName.English))
               .ForMember(dest => dest.SeriesShortName, exp => exp.MapFrom(src => src.SeriesShortName != null ? src.SeriesShortName.English : null))
               .ForMember(dest => dest.PriceSeriesName, exp => exp.MapFrom(src => GetSeriesName(src)))
               .ForMember(dest => dest.UnitDisplay, exp => exp.MapFrom(src => GetUnitDisplayName(src)))
               .ForMember(dest => dest.SeriesItemId, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.Id : string.Empty))
               .ForMember(dest => dest.SeriesId, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.SeriesId : string.Empty))
               .ForMember(dest => dest.AssessedDateTime, exp => exp.MapFrom(src => GetPriceSeriesItemAssessedDateTime(src.PriceSeriesItem)))
               .ForMember(dest => dest.DataUsed, exp => exp.MapFrom(src => GetDataUsed(src)))
               .ForMember(dest => dest.Price, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.Price : default))
               .ForMember(dest => dest.PriceLow, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PriceLow : default))
               .ForMember(dest => dest.PriceHigh, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PriceHigh : default))
               .ForMember(dest => dest.PriceMid, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PriceMid : default))
               .ForMember(dest => dest.LastAssessmentDate, exp => exp.MapFrom(src => GetLastAssessmentDate(src.LastAssessment)))
               .ForMember(dest => dest.LastAssessmentPrice, exp => exp.MapFrom(src => GetLastAssessmentPriceFormatted(src.LastAssessment)))
               .ForMember(dest => dest.LastAssessmentPriceValue, exp => exp.MapFrom(src => Exists(src.LastAssessment) ? src.LastAssessment!.Price : default))
               .ForMember(dest => dest.LastAssessmentPriceLowValue, exp => exp.MapFrom(src => Exists(src.LastAssessment) ? src.LastAssessment!.PriceLow : default))
               .ForMember(dest => dest.LastAssessmentPriceHighValue, exp => exp.MapFrom(src => Exists(src.LastAssessment) ? src.LastAssessment.PriceHigh : default))
               .ForMember(dest => dest.LastAssessmentPriceMidValue, exp => exp.MapFrom(src => Exists(src.LastAssessment) ? src.LastAssessment.PriceMid : default))
               .ForMember(dest => dest.LastAssessmentPeriodLabel, exp => exp.MapFrom(src => GetLastAssessmentPeriodLabel(src.LastAssessment)))
               .ForMember(dest => dest.LastAssessmentReferenceMarket, exp => exp.MapFrom(src => GetLastAssessmentReferenceMarket(src.LastAssessment)))
               .ForMember(dest => dest.PriceDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PriceDelta : default))
               .ForMember(dest => dest.AdjustedPriceDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.AdjustedPriceDelta : default))
               .ForMember(dest => dest.AssessmentMethod, exp => exp.MapFrom(src => GetAssessmentMethod(src)))
               .ForMember(dest => dest.ReferencePrice, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.ReferencePrice : default))
               .ForMember(dest => dest.PremiumDiscount, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PremiumDiscount : default))
               .ForMember(dest => dest.PriceLowDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PriceLowDelta : default))
               .ForMember(dest => dest.AdjustedPriceLowDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.AdjustedPriceLowDelta : default))
               .ForMember(dest => dest.PriceMidDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PriceMidDelta : default))
               .ForMember(dest => dest.AdjustedPriceMidDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.AdjustedPriceMidDelta : default))
               .ForMember(dest => dest.PriceHighDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.PriceHighDelta : default))
               .ForMember(dest => dest.AdjustedPriceHighDelta, exp => exp.MapFrom(src => Exists(src.PriceSeriesItem) ? src.PriceSeriesItem.AdjustedPriceHighDelta : default))
               .ForMember(dest => dest.Status, exp => exp.MapFrom(src => GetStatus(src.PriceSeriesItem)))
               .ForMember(dest => dest.LaunchDate, exp => exp.MapFrom(src => DateOnly.FromDateTime(src.LaunchDate)))
               .ForMember(dest => dest.TerminationDate, exp => exp.MapFrom(src => src.TerminationDate.HasValue ? DateOnly.FromDateTime(src.TerminationDate.Value) : null as DateOnly?))
               .ForMember(dest => dest.IsDerivedPriceSeries, exp => exp.MapFrom(src => PriceCategoryCode.Derived.Matches(src.PriceCategory.Code)))
               .ForMember(dest => dest.LastAssessmentPremiumDiscount, exp => exp.MapFrom(src => GetLastAssessmentPremiumDiscount(src.LastAssessment)))
               .ForMember(dest => dest.PeriodLabelTypeCode, exp => exp.MapFrom(src => GetEffectivePeriodLabelTypeCode(src)))
               .ForMember(dest => dest.LastAssessmentAppliesFromDateTime, exp => exp.MapFrom(src => GetLastAssessmentAppliesFromDateTime(src.LastAssessment)))
               .ForMember(dest => dest.PublicationScheduleId, exp => exp.MapFrom(src => src.PublicationSchedules.FirstOrDefault().ScheduleId));

            CreateMap<ColumnEditableWhen, DTOs.Output.ColumnEditableWhen>();

            CreateMap<PriceDelta, DTOs.Output.PriceDelta>();

            CreateMap<CustomConfig, DTOs.Output.CustomConfig>();
        }

        private static string? GetSeriesName(PriceSeriesAggregation priceSeries)
        {
            return priceSeries.SeriesShortName?.English ?? priceSeries.SeriesName.English;
        }

        private static string? GetUnitDisplayName(PriceSeriesAggregation priceSeries)
        {
            return !string.IsNullOrWhiteSpace(priceSeries.CurrencyUnit.Code)
                   && !string.IsNullOrWhiteSpace(priceSeries.MeasureUnit.Symbol)
                       ? $"{priceSeries.CurrencyUnit.Code}/{priceSeries.MeasureUnit.Symbol}"
                       : string.Empty;
        }

        private static string? GetLastAssessmentPremiumDiscount(PriceSeriesItem? lastAssessment)
        {
            return (AssessmentMethod.PremiumDiscount.Matches(lastAssessment?.AssessmentMethod) &&
                   lastAssessment?.PremiumDiscount != null) ? lastAssessment?.PremiumDiscount.FormatPrice() : null;
        }

        private string? GetLastAssessmentPriceFormatted(PriceSeriesItem lastAssessment)
        {
            if (lastAssessment?.Price != null)
            {
                return lastAssessment?.Price.FormatPrice();
            }

            if (lastAssessment?.PriceLow != null && lastAssessment?.PriceHigh != null)
            {
                return $"{lastAssessment?.PriceLow.FormatPrice()}-{lastAssessment?.PriceHigh.FormatPrice()}";
            }

            return null;
        }

        private string? GetLastAssessmentDate(PriceSeriesItem lastAssessment)
        {
            if (Exists(lastAssessment))
            {
                return lastAssessment.AssessedDateTime.ToString("dd MMM yyyy", DateTimeFormatInfo.InvariantInfo);
            }

            return null;
        }

        private string GetLastAssessmentPeriodLabel(PriceSeriesItem lastAssessment)
        {
            if (Exists(lastAssessment))
            {
                return lastAssessment.PeriodLabel;
            }

            return string.Empty;
        }

        private string? GetLastAssessmentReferenceMarket(PriceSeriesItem lastAssessment)
        {
            return lastAssessment?.ReferencePrice?.Market;
        }

        private DateTime? GetLastAssessmentAppliesFromDateTime(PriceSeriesItem priceSeriesItem)
        {
            return Exists(priceSeriesItem) ? priceSeriesItem.AppliesFromDateTime : null as DateTime?;
        }

        private DateTime? GetPriceSeriesItemAssessedDateTime(PriceSeriesItem priceSeriesItem)
        {
            return Exists(priceSeriesItem) ? priceSeriesItem.AssessedDateTime : null as DateTime?;
        }

        private bool Exists<T>(T obj)
        {
            return obj != null;
        }

        private string GetStatus(PriceSeriesItem priceSeriesItem)
        {
            return WorkflowStatus.GetDisplayValueForStatus(priceSeriesItem?.Status);
        }

        private string? GetAssessmentMethod(PriceSeriesAggregation priceSeries)
        {
            if (Exists(priceSeries.PriceSeriesItem))
            {
                return priceSeries.PriceSeriesItem.AssessmentMethod;
            }

            return priceSeries.LastAssessment?.AssessmentMethod;
        }

        private string? GetDataUsed(PriceSeriesAggregation priceSeries)
        {
            return priceSeries?.PriceSeriesItem?.DataUsed ?? priceSeries?.LastAssessment?.DataUsed;
        }

        private string GetEffectivePeriodLabelTypeCode(PriceSeriesAggregation priceSeries)
        {
            var periodLabelTypeCode = priceSeries.PeriodLabelTypeCode;

            if (PeriodLabelTypeCode.None.Matches(periodLabelTypeCode) || PeriodLabelTypeCode.RelativeFulfilmentTime.Matches(periodLabelTypeCode) ||
                (PeriodLabelTypeCode.ReferenceTime.Matches(periodLabelTypeCode) &&
                 PeriodTypeCode.IsAllowedReference(priceSeries.ReferencePeriod?.PeriodType.Code)))
            {
                return priceSeries.PeriodLabelTypeCode;
            }

            return PeriodLabelTypeCode.None.Value;
        }
    }
}
