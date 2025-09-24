namespace BusinessLayer.PriceEntry.DTOs.Output
{
    public record PriceDelta
    {
        public required string PriceField { get; init; }

        public required string PriceDeltaField { get; init; }

        public required string PrecisionField { get; init; }
    }
}
