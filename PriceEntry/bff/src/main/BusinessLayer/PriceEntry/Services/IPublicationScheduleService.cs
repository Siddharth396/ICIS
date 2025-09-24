namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    public interface IPublicationScheduleService
    {
        Task<NextPublicationDate?> GetNextPublicationDate(PriceSeries priceSeries, DateTime assessedDateTime);
    }
}
