namespace Infrastructure.PubNub
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PubNubOptions
    {
        public const string ConfigKey = "PubNub";

        public required bool Enabled { get; set; }

        public required string PublishKey { get; set; }

        public required int RetryCount { get; set; }

        public required string SubscribeKey { get; set; }

        public required string Uuid { get; set; }
    }
}
