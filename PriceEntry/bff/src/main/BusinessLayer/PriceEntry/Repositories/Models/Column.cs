namespace BusinessLayer.PriceEntry.Repositories.Models
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

        [BsonElement("display_order")]
        public int DisplayOrder { get; set; } = default!;

        [BsonElement("editable")]
        public bool Editable { get; set; } = default!;

        [BsonElement("values")]
        public IList<string>? Values { get; set; } = default!;

        [BsonElement("pinned")]
        public string? Pinned { get; set; } = default!;

        [BsonElement("editable_when")]
        public ColumnEditableWhen? EditableWhen { get; set; }

        [BsonElement("type")]
        public string? Type { get; set; }

        [BsonElement("custom_config")]
        public CustomConfig? CustomConfig { get; set; }

        [BsonElement("tooltip_field")]
        public string? TooltipField { get; set; } = null!;

        [BsonElement("cell_type")]
        public string? CellType { get; set; } = null!;

        [BsonElement("hideable")]
        public bool Hideable { get; set; } = default!;

        [BsonElement("hidden")]
        public bool? Hidden { get; set; } = default!;

        [BsonElement("for_correction_workflow")]
        public bool? ForCorrectionWorkflow { get; set; } = default!;
    }
}
