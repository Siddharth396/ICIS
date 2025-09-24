namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output
{
    using System;

    using HotChocolate;

    [GraphQLName("DropdownFilterItem")]
    public class DropdownFilterItem
    {
        public required string Name { get; set; }

        public required Guid Id { get; set; }
    }
}
