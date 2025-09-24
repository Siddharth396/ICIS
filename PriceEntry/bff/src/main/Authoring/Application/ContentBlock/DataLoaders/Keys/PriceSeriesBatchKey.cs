namespace Authoring.Application.ContentBlock.DataLoaders.Keys
{
    using System;
    using System.Collections.Generic;

    public record PriceSeriesBatchKey(DateTime AssessedDateTime, List<string> PriceSeriesIds);
}
