namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public record DataPackageKey(string ContentBlockId, Version ContentBlockVersion, DateTime AssessedDateTime);
}
