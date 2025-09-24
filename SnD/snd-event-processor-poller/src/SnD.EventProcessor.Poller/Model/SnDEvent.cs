using SnD.EventProcessor.Poller.Constants;
using System;
using System.Text.Json.Serialization;

namespace SnD.EventProcessor.Poller.Model
{
    [Serializable]
    public class SnDEvent
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("EntityId")]
        public int EntityId { get; set; }

        [JsonPropertyName("EntityType")]
        public string EntityType { get; set; }

        [JsonPropertyName("MessageType")]
        public string MessageType { get; set; } = "Realtime";

        [JsonPropertyName("EventType")]
        public string EventType { get; set; }

        [JsonPropertyName("EventTime")]
        public DateTime EventTime { get; set; }

        [JsonPropertyName("OtherProperties")]
        public string OtherProperties { get; set; } = null;

        public string GetDetails()
        {
            return $"Event Id: {Id}, Event Type: {EventType}, Entity Type: {EntityType}, Entity Id {EntityId} and Event Time: {EventTime} and MessageType:{MessageType} and OtherProperties:{OtherProperties}";
        }
        public bool IsValid()
        {
            var isValid = EntityId > 0 &&
                Array.Exists(SnDEntityTypes.EntityTypes, x => x.Equals(EntityType?.ToLower())) &&
                Array.Exists(SnDEventTypes.EventTypes, x => x.Equals(EventType?.ToLower()));

            return isValid;
        }
    }
}
