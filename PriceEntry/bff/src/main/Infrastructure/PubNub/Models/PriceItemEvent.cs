namespace Infrastructure.PubNub.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class PriceItemEvent
    {
        private const string PriceAuthoringEvents = "PriceAuthoringEvents";

        private PriceItemEvent(IEnumerable<string> contentBlockIds, DateTime assessedDateTime)
        {
            ContentBlockIds = contentBlockIds;
            AssessedDateTime = assessedDateTime;
        }

        [JsonProperty("contentBlockIds")]
        public IEnumerable<string> ContentBlockIds { get; }

        [JsonProperty("assessedDateTime")]
        public DateTime AssessedDateTime { get; }

        public static PubNubData<PriceItemEvent> CreatePubNubData(IEnumerable<string> contentBlockIds, DateTime assessedDateTime)
        {
            return new PubNubData<PriceItemEvent>
            {
                Type = PriceAuthoringEvents,
                Content = new PriceItemEvent(contentBlockIds, assessedDateTime)
            };
        }
    }
}
