namespace BusinessLayer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Serilog;

    using DTOOutput = BusinessLayer.PriceEntry.DTOs.Output;

    // todo: We will put this logic inside absoluteperioddomain service when we do restructuring as part of this VIK-1097
    public static class PeriodLabelTypeHelper
    {
        private static readonly Dictionary<string, Func<DateTime, DateTime>> ReferencePeriodCodeCalculators = new()
        {
            { ReferencePeriodCode.Monthly, dt => dt.AddMonths(1) },
            { ReferencePeriodCode.Quarterly, dt => dt.AddMonths(3) }
        };

        public static (DateOnly ReferenceDate, string PeriodCode)? GetAbsolutePeriodCalculationInput(
            ILogger logger,
            PriceSeries priceSeries,
            DateTime assessedDateTime,
            DateTime? lastPublishedAppliesFromDateTime)
        {
            return GetAbsolutePeriodCalculationInput(
                assessedDateTime,
                lastPublishedAppliesFromDateTime,
                priceSeries.RelativeFulfilmentPeriod?.Code,
                priceSeries.ReferencePeriod?.Code,
                priceSeries.PeriodLabelTypeCode,
                logger);
        }

        public static (DateOnly ReferenceDate, string PeriodCode)? GetAbsolutePeriodCalculationInput(
            ILogger logger,
            PriceSeriesAggregation priceSeriesAggregation,
            DateTime assessedDateTime)
        {
            var lastAssessment = priceSeriesAggregation.LastAssessments?.FirstOrDefault();

            return GetAbsolutePeriodCalculationInput(
                assessedDateTime,
                lastAssessment?.AppliesFromDateTime,
                priceSeriesAggregation.RelativeFulfilmentPeriod?.Code,
                priceSeriesAggregation.ReferencePeriod?.Code,
                priceSeriesAggregation.PeriodLabelTypeCode,
                logger);
        }

        public static (DateOnly ReferenceDate, string PeriodCode)? GetAbsolutePeriodCalculationInput(
            ILogger logger,
            DTOOutput.PriceSeries priceSeries,
            DateTime assessedDateTime)
        {
            return GetAbsolutePeriodCalculationInput(
                assessedDateTime,
                priceSeries.LastAssessmentAppliesFromDateTime,
                priceSeries.RelativeFulfilmentPeriod?.Code,
                priceSeries.ReferencePeriod?.Code,
                priceSeries.PeriodLabelTypeCode,
                logger);
        }

        private static (DateOnly ReferenceDate, string PeriodCode)? GetAbsolutePeriodCalculationInput(
            DateTime assessedDateTime,
            DateTime? lastPublishedAppliesFromDateTime,
            string? fulfilmentPeriodCode,
            string? referencePeriodCode,
            string? periodLabelTypeCode,
            ILogger logger)
        {
            var absolutePeriodCalculationInput = periodLabelTypeCode switch
            {
                _ when PeriodLabelTypeCode.ReferenceTime.Matches(periodLabelTypeCode) => GetReferencePeriodCalculationInput(
                                                                                                    assessedDateTime,
                                                                                                    lastPublishedAppliesFromDateTime,
                                                                                                    referencePeriodCode,
                                                                                                    logger),
                _ when PeriodLabelTypeCode.RelativeFulfilmentTime.Matches(periodLabelTypeCode) => GetRelativeFulfilmentPeriodCalculationInput(
                                                                                                    assessedDateTime,
                                                                                                    fulfilmentPeriodCode,
                                                                                                    logger),
                _ when PeriodLabelTypeCode.None.Matches(periodLabelTypeCode) => LogAndReturnNull<(DateTime, string)?>(logger, "None Period label type: {PeriodLabelTypeCode}", periodLabelTypeCode),
                _ => LogAndReturnNull<(DateTime, string)?>(logger, "Unhandled period label type: {PeriodLabelTypeCode}", periodLabelTypeCode)
            };

            if (absolutePeriodCalculationInput is null)
            {
                return null;
            }

            var (assessedDatetime, periodCode) = absolutePeriodCalculationInput.Value;

            var referenceDate = DateOnly.FromDateTime(assessedDatetime);

            return (referenceDate, periodCode);
        }

        private static (DateTime AssessedDatetime, string PeriodCode)? GetReferencePeriodCalculationInput(
            DateTime assessedDatetime,
            DateTime? lastPublishedAppliesFromDateTime,
            string? referencePeriodCode,
            ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(referencePeriodCode))
            {
                return LogAndReturnNull<(DateTime, string)?>(logger, "Missing reference period code.");
            }

            if (!ReferencePeriodCode.IsAllowed(referencePeriodCode))
            {
                return LogAndReturnNull<(DateTime, string)?>(logger, "Reference period code: {ReferencePeriodCode} is not supported", referencePeriodCode);
            }

            if (lastPublishedAppliesFromDateTime is null)
            {
                return (assessedDatetime, referencePeriodCode);
            }

            var nextReferenceDateCalculator = ReferencePeriodCodeCalculators.GetValueOrDefault(referencePeriodCode);

            if (nextReferenceDateCalculator is null)
            {
                return LogAndReturnNull<(DateTime, string)?>(logger, "No calculation found for allowed reference period code: {ReferencePeriodCode}", referencePeriodCode);
            }

            var updatedAssessedDatetime = nextReferenceDateCalculator(lastPublishedAppliesFromDateTime.Value);

            return (updatedAssessedDatetime, referencePeriodCode);
        }

        private static (DateTime AssessedDatetime, string PeriodCode)? GetRelativeFulfilmentPeriodCalculationInput(
            DateTime assessedDatetime,
            string? fulfilmentPeriodCode,
            ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(fulfilmentPeriodCode))
            {
                return LogAndReturnNull<(DateTime, string)?>(logger, "Missing relative fulfilment period code");
            }

            if (!RelativeFulfilmentPeriodCode.IsAllowed(fulfilmentPeriodCode))
            {
                return LogAndReturnNull<(DateTime, string)?>(logger, "Reference period code: {ReferencePeriodCode} is not supported", fulfilmentPeriodCode);
            }

            return (assessedDatetime, fulfilmentPeriodCode);
        }

        private static T? LogAndReturnNull<T>(ILogger logger, string message, params object?[]? propertyValues)
        {
            logger.Debug(message, propertyValues);
            return default(T);
        }
    }
}
