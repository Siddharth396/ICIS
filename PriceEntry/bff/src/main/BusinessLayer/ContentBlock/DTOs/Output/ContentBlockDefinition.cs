namespace BusinessLayer.ContentBlock.DTOs.Output
{
    using System.Collections.Generic;
    using System.Linq;

    using HotChocolate;

    public class ContentBlockDefinition
    {
        public required string ContentBlockId { get; set; }

        public required int Version { get; set; }

        public string? Title { get; set; }

        public List<PriceSeriesGrid>? PriceSeriesGrids { get; set; } = default!;

        [GraphQLIgnore]
        public List<string> GetPriceSeriesIds()
        {
            return PriceSeriesGrids?.SelectMany(psg => psg.PriceSeriesIds ?? []).ToList() ?? [];
        }
    }
}
