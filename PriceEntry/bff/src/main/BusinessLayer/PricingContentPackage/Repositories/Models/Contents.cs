namespace BusinessLayer.PricingContentPackage.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Contents
    {
        [BsonElement("content_blocks")]
        public required List<ContentBlock> ContentBlocks { get; set; }
    }
}