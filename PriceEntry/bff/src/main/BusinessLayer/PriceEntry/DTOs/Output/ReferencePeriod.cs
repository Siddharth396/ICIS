namespace BusinessLayer.PriceEntry.DTOs.Output
{
    public class ReferencePeriod
    {
        public string Name { get; set; } = default!;

        public string Code { get; set; } = default!;

        public PeriodType PeriodType { get; set; } = default!;
    }
}
