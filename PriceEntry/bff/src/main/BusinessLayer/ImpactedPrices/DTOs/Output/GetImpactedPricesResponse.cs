namespace BusinessLayer.ImpactedPrices.DTOs.Output
{
    public class GetImpactedPricesResponse
    {
        public required bool IsSuccess { get; set; }

        public string? ErrorCode { get; set; }

        public ImpactedPrices? ImpactedPrices { get; set; }

        public static GetImpactedPricesResponse Error(string errorCode) => new() { IsSuccess = false, ErrorCode = errorCode };
    }
}
