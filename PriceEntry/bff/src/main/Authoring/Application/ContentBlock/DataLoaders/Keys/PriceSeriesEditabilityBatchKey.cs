namespace Authoring.Application.ContentBlock.DataLoaders.Keys
{
    using BusinessLayer.PriceEntry.ValueObjects;

    public record PriceSeriesEditabilityBatchKey(DataPackageKey DataPackageKey, string? PriceSeriesItemStatus, bool IsReviewMode);
}
