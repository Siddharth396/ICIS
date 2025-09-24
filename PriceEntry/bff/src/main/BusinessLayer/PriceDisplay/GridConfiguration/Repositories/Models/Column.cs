namespace BusinessLayer.PriceDisplay.GridConfiguration.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Column
    {
        [BsonElement("field")]
        public string Field { get; set; } = default!;

        [BsonElement("header_name")]
        public string HeaderName { get; set; } = default!;

        [BsonElement("cell_data_type")]
        public string CellDataType { get; set; } = default!;

        [BsonElement("values")]
        public IList<string> Values { get; set; } = [];

        [BsonElement("pinned")]
        public string? Pinned { get; set; }

        [BsonElement("type")]
        public string? Type { get; set; }

        [BsonElement("custom_config")]
        public CustomConfig? CustomConfig { get; set; }

        [BsonElement("tooltip_field")]
        public string? TooltipField { get; set; }

        [BsonElement("cell_type")]
        public string? CellType { get; set; }

        [BsonElement("hideable")]
        public bool Hideable { get; set; }

        [BsonElement("min_width")]
        public int? MinWidth { get; set; }

        [BsonElement("max_width")]
        public int? MaxWidth { get; set; }

        [BsonElement("width")]
        public int? Width { get; set; }

        [BsonElement("alternate_fields")]
        public IEnumerable<AlternateField> AlternateFields { get; init; } = [];

        [BsonElement("auto_size")]
        public bool AutoSize { get; init; } = true;

        [BsonElement("is_visible_to_subscriber")]
        public bool IsVisibleToSubscriber { get; init; } = true;
    }
}
