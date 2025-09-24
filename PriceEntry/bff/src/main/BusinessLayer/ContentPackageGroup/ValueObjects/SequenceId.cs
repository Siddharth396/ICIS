namespace BusinessLayer.ContentPackageGroup.ValueObjects
{
    using System.Collections.Generic;
    using System.Linq;

    using Infrastructure.Services;

    public record SequenceId
    {
        public SequenceId(IEnumerable<string> priceSeriesIds)
        {
            Value = Hasher.GetSha256Hash(string.Join(',', priceSeriesIds.Order()));
        }

        public string Value { get; }
    }
}
