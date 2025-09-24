namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ContentBlockQueryHelper
    {
        public static ICollection<ICollection<string>> CreatePriceSeriesIds(params string[][] seriesIds)
        {
            return seriesIds.Select(seriesIdGroup => new List<string>(seriesIdGroup)).ToArray();
        }
    }
}
