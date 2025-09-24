namespace BusinessLayer.DataPackage.Helpers
{
    using System;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public static class DataPackageKeyHelper
    {
        public static DataPackageKey GetDataPackageKey(this ContentBlockDefinition contentBlock, DateTime assessedDateTime)
        {
            return new DataPackageKey(contentBlock.ContentBlockId, Version.From(contentBlock.Version), assessedDateTime);
        }
    }
}
