namespace BusinessLayer.PriceEntry.Handler
{
    using System;

    using Infrastructure.EventDispatcher;
    using Infrastructure.Services.Workflow;

    public class PriceSeriesItemSavedEvent : IEvent
    {
        public required string SeriesId { get; set; }

        public required DateTime AssessedDateTime { get; set; }

        public required OperationType OperationType { get; set; }
    }
}
