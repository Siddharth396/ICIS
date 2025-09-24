namespace BusinessLayer.DataPackage.Handler
{
    using System;

    using Infrastructure.EventDispatcher;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public class NonMarketAdjustmentDisabledEvent : IEvent
    {
        public NonMarketAdjustmentDisabledEvent(
            string contentBlockId,
            Version contentBlockVersion,
            DateTime assessedDateTime)
        {
            ContentBlockId = contentBlockId;
            ContentBlockVersion = contentBlockVersion;
            AssessedDateTime = assessedDateTime;
        }

        public DateTime AssessedDateTime { get; }

        public string ContentBlockId { get; }

        public Version ContentBlockVersion { get; }
    }
}
