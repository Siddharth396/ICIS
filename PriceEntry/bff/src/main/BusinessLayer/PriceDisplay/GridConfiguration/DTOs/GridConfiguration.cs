namespace BusinessLayer.PriceDisplay.GridConfiguration.DTOs
{
    using System.Collections.Generic;

    using HotChocolate;

    [GraphQLName("PriceDisplayGridConfiguration")]
    public class GridConfiguration
    {
        public IEnumerable<Column> Columns { get; set; } = [];

        public Sort? Sort { get; set; } = default!;

        public IEnumerable<string> SeriesItemTypeCodes { get; set; } = [];
    }
}
