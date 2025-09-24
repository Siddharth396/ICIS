namespace BusinessLayer.ContentBlock.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.Common.Mappers;
    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.ContentBlock.Repositories.Models;

    using PriceSeriesGrid = BusinessLayer.ContentBlock.DTOs.Output.PriceSeriesGrid;

    public class ContentBlockMapper : IModelMapper<ContentBlock, ContentBlockDefinition>
    {
        private readonly IModelMapper<Repositories.Models.PriceSeriesGrid, DTOs.Output.PriceSeriesGrid>
            priceSeriesGridMapper;

        public ContentBlockMapper(IModelMapper<Repositories.Models.PriceSeriesGrid, DTOs.Output.PriceSeriesGrid> priceSeriesGridMapper)
        {
            this.priceSeriesGridMapper = priceSeriesGridMapper;
        }

        public ContentBlockDefinition Map(ContentBlock model)
        {
            return new ContentBlockDefinition
            {
                ContentBlockId = model.ContentBlockId,
                Version = model.Version,
                Title = model.Title,
                PriceSeriesGrids = GetPriceSeriesGrids(model)
            };
        }

        private List<PriceSeriesGrid> GetPriceSeriesGrids(ContentBlock model)
        {
            if (model.PriceSeriesGrids != null)
            {
                return model.PriceSeriesGrids.Select(priceSeriesGridMapper.Map).ToList();
            }

            return Array.Empty<PriceSeriesGrid>().ToList();
        }
    }
}