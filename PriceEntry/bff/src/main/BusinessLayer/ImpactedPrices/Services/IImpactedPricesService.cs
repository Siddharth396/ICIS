namespace BusinessLayer.ImpactedPrices.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.ImpactedPrices.DTOs.Output;

    public interface IImpactedPricesService
    {
        Task<GetImpactedPricesResponse> GetImpactedPrices(string priceSeriesId, DateTime assessedDateTime);

        Task<IDictionary<string, bool>> HasImpactedPrices(List<string> priceSeriesIds, DateTime assessedDateTime);
    }
}
