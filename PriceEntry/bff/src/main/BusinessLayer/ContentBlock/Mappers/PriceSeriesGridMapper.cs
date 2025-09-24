namespace BusinessLayer.ContentBlock.Mappers
{
    using BusinessLayer.Common.Mappers;

    public class PriceSeriesGridMapper : IModelMapper<Repositories.Models.PriceSeriesGrid, DTOs.Output.PriceSeriesGrid>
    {
        public DTOs.Output.PriceSeriesGrid Map(Repositories.Models.PriceSeriesGrid model)
        {
            return new DTOs.Output.PriceSeriesGrid()
            {
                Id = model.PriceSeriesGridId,
                Title = model.Title,
                PriceSeriesIds = model.PriceSeriesIds,
                SeriesItemTypeCode = model.SeriesItemTypeCode,
            };
        }
    }
}
