namespace Authoring.Application.ContentBlock.DataLoaders.Keys
{
    using System;

    public record PeriodBatchKey(DateOnly ReferenceDate, string PeriodCode);
}
