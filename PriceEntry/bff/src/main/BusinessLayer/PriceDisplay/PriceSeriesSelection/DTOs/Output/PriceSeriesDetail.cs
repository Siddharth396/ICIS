namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output
{
    using System;

    [HotChocolate.GraphQLName("PriceSeriesDetailForDisplay")]
    public class PriceSeriesDetail
    {
        public required string Id { get; set; }

        public Guid ItemFrequencyId { get; set; }

        public Guid RegionId { get; set; }

        public Guid PriceSettlementTypeId { get; set; }

        public Guid PriceCategoryId { get; set; }

        public required string SeriesItemTypeCode { get; set; }

        public required string Name { get; set; }
    }
}
