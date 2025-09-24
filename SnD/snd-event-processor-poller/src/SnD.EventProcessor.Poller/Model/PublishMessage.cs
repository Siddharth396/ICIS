using System;
using System.Text.Json.Serialization;

namespace SnD.EventProcessor.Poller.Model
{
    [Serializable]
    public class PublishMessage
    {
        [JsonPropertyName("@default")]
        public string @default { get; set; } = string.Empty;

        [JsonPropertyName("SnDMessage")]
        public string SnDMessage { get; set; }

        [JsonPropertyName("sqs")]
        public string sqs { get; set; }
    }
}
