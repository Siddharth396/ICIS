namespace BusinessLayer.PriceDisplay.GridConfiguration.DTOs
{
    using System.Collections.Generic;

    public record AlternateField(IEnumerable<string> SeriesItemTypeCodes, string Field, string? PriceDeltaField);
}
