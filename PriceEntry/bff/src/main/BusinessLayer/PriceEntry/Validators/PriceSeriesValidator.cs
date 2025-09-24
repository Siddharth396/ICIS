namespace BusinessLayer.PriceEntry.Validators
{
    using BusinessLayer.PriceEntry.DTOs.Output;
    using BusinessLayer.PriceEntry.ValueObjects;

    public class PriceSeriesValidator
    {
        public static ValidationResult Validate(PriceSeries priceSeries, bool includeNotStarted = false)
        {
            return priceSeries.SeriesItemTypeCode switch
            {
                SeriesItemTypeCode.SingleValueWithReference => ValidateSingleValueWithReferenceSeries(
                    priceSeries,
                    includeNotStarted),
                SeriesItemTypeCode.Range => ValidateRangeSeries(priceSeries, includeNotStarted),
                SeriesItemTypeCode.SingleValue => ValidateSingleValueSeries(priceSeries, includeNotStarted),
                SeriesItemTypeCode.CharterRateSingleValue => ValidateCharterRateSingleValueSeries(
                    priceSeries,
                    includeNotStarted),
                _ => new ValidationResult()
            };
        }

        public static ValidationResult ValidateCharterRateSingleValueSeries(
            ICharterRateSingleValueValidationData priceSeries,
            bool includeNotStarted)
        {
            var validationResult = new ValidationResult();
            var noFieldsAreSet = priceSeries.Price == null
                                 && string.IsNullOrWhiteSpace(priceSeries.DataUsed)
                                 && priceSeries.AdjustedPriceDelta == null;

            if (!includeNotStarted && noFieldsAreSet)
            {
                return validationResult;
            }

            if (priceSeries.Price is null)
            {
                validationResult.AddError(nameof(PriceSeries.Price), "Price is required.");
            }

            if (string.IsNullOrWhiteSpace(priceSeries.DataUsed))
            {
                validationResult.AddError(nameof(PriceSeries.DataUsed), "Data used is required.");
            }
            else if (!DataUsed.IsValid(priceSeries.DataUsed))
            {
                validationResult.AddError(
                    nameof(PriceSeries.DataUsed),
                    $"Data used '{priceSeries.DataUsed}' is invalid.");
            }

            return validationResult;
        }

        public static ValidationResult ValidateRangeSeries(IRangeValidationData priceSeries, bool includeNotStarted)
        {
            var validationResult = new ValidationResult();

            var noFieldsAreSet = priceSeries.PriceLow == null
                                 && priceSeries.PriceHigh == null
                                 && priceSeries.AdjustedPriceLowDelta == null
                                 && priceSeries.AdjustedPriceHighDelta == null;

            if (!includeNotStarted && noFieldsAreSet)
            {
                return validationResult;
            }

            var priceLowValidationResult = ValidatePrice("Price low", priceSeries.PriceLow);

            if (!priceLowValidationResult.IsValid)
            {
                validationResult.AddError(nameof(PriceSeries.PriceLow), priceLowValidationResult.ErrorMessage);
            }

            var priceHighValidationResult = ValidatePrice("Price high", priceSeries.PriceHigh);

            if (!priceHighValidationResult.IsValid)
            {
                validationResult.AddError(nameof(PriceSeries.PriceHigh), priceHighValidationResult.ErrorMessage);
            }

            if (priceSeries.PriceHigh < priceSeries.PriceLow)
            {
                validationResult.AddError(nameof(PriceSeries.PriceHigh), "Price low must be less or equal than price high.");
            }

            if (priceSeries is
                {
                    AdjustedPriceHighDelta: not null,
                    AdjustedPriceLowDelta: null
                })
            {
                validationResult.AddError(nameof(PriceSeries.AdjustedPriceLowDelta), "Adjusted low change must be set.");
            }

            if (priceSeries is
                {
                    AdjustedPriceLowDelta: not null,
                    AdjustedPriceHighDelta: null
                })
            {
                validationResult.AddError(nameof(PriceSeries.AdjustedPriceHighDelta), "Adjusted high change must be set.");
            }

            return validationResult;
        }

        public static ValidationResult ValidateSingleValueSeries(
            ISingleValueValidationData priceSeries,
            bool includeNotStarted)
        {
            var validationResult = new ValidationResult();
            var noFieldsAreSet = priceSeries.Price == null && priceSeries.AdjustedPriceDelta == null;

            if (!includeNotStarted && noFieldsAreSet)
            {
                return validationResult;
            }

            var priceValidationResult = ValidatePrice("Price", priceSeries.Price);

            if (!priceValidationResult.IsValid)
            {
                validationResult.AddError(nameof(PriceSeries.Price), priceValidationResult.ErrorMessage);
            }

            return validationResult;
        }

        public static ValidationResult ValidateSingleValueWithReferenceSeries(
            ISingleValueWithReferenceValidationData priceSeries,
            bool includeNotStarted)
        {
            var noFieldsAreSet = string.IsNullOrWhiteSpace(priceSeries.AssessmentMethod)
                                 && priceSeries.Price == null
                                 && priceSeries.ReferencePriceValue == null
                                 && priceSeries.PremiumDiscount == null
                                 && string.IsNullOrWhiteSpace(priceSeries.DataUsed)
                                 && priceSeries.AdjustedPriceDelta == null;

            var validationResult = new ValidationResult();

            if (!includeNotStarted && noFieldsAreSet)
            {
                return validationResult;
            }

            if (priceSeries.AssessmentMethod == null)
            {
                validationResult.AddError(nameof(PriceSeries.AssessmentMethod), "Assessment method is required.");
            }
            else if (!AssessmentMethod.IsValid(priceSeries.AssessmentMethod))
            {
                validationResult.AddError(
                    nameof(PriceSeries.AssessmentMethod),
                    $"Assessment method '{priceSeries.AssessmentMethod}' is invalid.");
            }

            if (AssessmentMethod.PremiumDiscount.Matches(priceSeries.AssessmentMethod))
            {
                if (priceSeries.PremiumDiscount == null)
                {
                    validationResult.AddError(nameof(PriceSeries.PremiumDiscount), "Premium/Discount is required.");
                }

                if (priceSeries.ReferencePriceValue == null)
                {
                    validationResult.AddError(nameof(PriceSeries.ReferencePrice), "Reference price is required.");
                }
                else
                {
                    var referencePriceValidationResult = ValidatePrice(
                        "Reference price",
                        priceSeries.ReferencePriceValue);

                    if (!referencePriceValidationResult.IsValid)
                    {
                        validationResult.AddError(
                            nameof(PriceSeries.ReferencePrice),
                            "Selected reference price could not be found for the date, please ensure it is added.");
                    }
                }
            }

            if (AssessmentMethod.Assessed.Matches(priceSeries.AssessmentMethod))
            {
                var priceValidationResult = ValidatePrice("Price", priceSeries.Price);

                if (!priceValidationResult.IsValid)
                {
                    validationResult.AddError(nameof(PriceSeries.Price), priceValidationResult.ErrorMessage);
                }
            }

            if (string.IsNullOrWhiteSpace(priceSeries.DataUsed))
            {
                validationResult.AddError(nameof(PriceSeries.DataUsed), "Data used is required.");
            }
            else if (!DataUsed.IsValid(priceSeries.DataUsed))
            {
                validationResult.AddError(
                    nameof(PriceSeries.DataUsed),
                    $"Data used '{priceSeries.DataUsed}' is invalid.");
            }

            return validationResult;
        }

        private static (bool IsValid, string ErrorMessage) ValidatePrice(string fieldName, decimal? price)
        {
            if (price is null)
            {
                return (false, $"{fieldName} is required.");
            }

            if (!Price.TryCreate(price.Value, out _))
            {
                return (false, $"{fieldName} must be greater than 0.");
            }

            return (true, string.Empty);
        }
    }
}