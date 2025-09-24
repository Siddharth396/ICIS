namespace Infrastructure.MongoDB.Serializers
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization;
    using global::MongoDB.Bson.Serialization.Serializers;

    public class DecimalToBsonConverter : StructSerializerBase<decimal>
    {
        private static readonly DecimalSerializer BaseSerializer = new(BsonType.Decimal128);

        public override decimal Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var decimalValue = BaseSerializer.Deserialize(context, args);

            // Normalize negative zero back to positive zero
            if (decimalValue == 0m)
            {
                return 0m;
            }

            return decimalValue;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, decimal value)
        {
            // Convert negative zero (-0.00m) to positive zero (0.00m)
            if (value == 0m)
            {
                value = 0m;
            }

            BaseSerializer.Serialize(context, args, value);
        }
    }
}
