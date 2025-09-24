using System;

using MongoDB.Bson.Serialization.Attributes;

namespace BusinessLayer.PriceSeriesSelection.Repositories.Models
{
    [BsonIgnoreExtraElements]
    public class PublicationSchedule
    {
        [BsonElement("guid")]
        public required string ScheduleId { get; set; }

        [BsonElement("applies_from_date")]
        public DateTime? AppliesFromDate { get; set; }

        [BsonElement("applies_until_date")]
        public DateTime? AppliesUntilDate { get; set; }

        [BsonElement("name")]
        public Name ScheduleName { get; set; } = default!;

        [BsonElement("frequency")]
        public ItemFrequency Frequency { get; set; } = default!;
    }
}
