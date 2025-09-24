namespace Infrastructure.PubNub.Models
{
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    [ExcludeFromCodeCoverage]
    public class PubNubData<T>
    {
        [JsonProperty("type")]
        public required string Type { get; set; }

        [JsonProperty("content")]
        public required T Content { get; set; }
    }
}
