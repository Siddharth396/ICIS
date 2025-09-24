namespace Authoring.Tests.Infrastructure.MongoDB.Serializers
{
    using FluentAssertions;

    using global::Infrastructure.MongoDB.Serializers;

    using global::MongoDB.Bson;
    using global::MongoDB.Bson.IO;
    using global::MongoDB.Bson.Serialization;

    using Xunit;

    public class DecimalToBsonConverterTests
    {
        private readonly DecimalToBsonConverter converter = new();
        private readonly BsonSerializationArgs serializationArgs = new BsonSerializationArgs(typeof(decimal), true, false);

        private readonly BsonDeserializationArgs deserializationArgs =
            new BsonDeserializationArgs { NominalType = typeof(decimal) };

        [Fact]
        public void Serialize_ShouldConvertNegativeZeroToPositiveZero()
        {
            // Arrange
            var input = -0.00m;
            var bsonDoc = new BsonDocument();
            using (var bsonWriter = new BsonDocumentWriter(bsonDoc))
            {
                bsonWriter.WriteStartDocument();
                bsonWriter.WriteName("value");

                // Act
                converter.Serialize(BsonSerializationContext.CreateRoot(bsonWriter), serializationArgs, input);

                bsonWriter.WriteEndDocument();
            }

            // Extract serialized value
            var serializedValue = bsonDoc["value"].AsDecimal128;

            // Assert
            var value = (decimal)serializedValue;
            value.Should().Be(0m, because: "negative zero should be converted to positive zero");
            GetSignBit(value).Should().Be(0, because: "the sign bit should be 0 for positive zero");
        }

        [Fact]
        public void Deserialize_ShouldConvertNegativeZeroToPositiveZero()
        {
            // Arrange
            var bson = new BsonDocument { { "value", new BsonDecimal128(-0.00m) } };
            using var bsonReader = new BsonDocumentReader(bson);
            bsonReader.ReadStartDocument();
            bsonReader.ReadName("value");

            // Act
            var result = converter.Deserialize(BsonDeserializationContext.CreateRoot(bsonReader), deserializationArgs);

            // Assert
            result.Should().Be(0m, because: "negative zero should be normalized to positive zero");
            GetSignBit(result).Should().Be(0, because: "the sign bit should be 0 for positive zero");
        }

        [Theory]
        [InlineData(0.00)]
        [InlineData(1.23)]
        [InlineData(-1.23)]
        [InlineData(99999.99)]
        public void Serialize_ShouldNotModifyOtherDecimalValues(decimal input)
        {
            // Arrange
            var bsonDoc = new BsonDocument();
            using (var bsonWriter = new BsonDocumentWriter(bsonDoc))
            {
                bsonWriter.WriteStartDocument();
                bsonWriter.WriteName("value");

                // Act
                converter.Serialize(BsonSerializationContext.CreateRoot(bsonWriter), serializationArgs, input);

                bsonWriter.WriteEndDocument();
            }

            // Extract serialized value
            var serializedValue = bsonDoc["value"].AsDecimal128;

            // Assert
            var value = (decimal)serializedValue;
            value.Should().Be(input, because: "serialization should not modify valid decimal values");
            GetSignBit(value).Should().Be(
                    GetSignBit(input),
                    because: "the sign bit should be the same for the input and output values");
        }

        [Theory]
        [InlineData(0.00)]
        [InlineData(1.23)]
        [InlineData(-1.23)]
        [InlineData(99999.99)]
        public void Deserialize_ShouldNotModifyOtherDecimalValues(decimal input)
        {
            // Arrange
            var bson = new BsonDocument { { "value", new BsonDecimal128(input) } };
            using var bsonReader = new BsonDocumentReader(bson);
            bsonReader.ReadStartDocument();
            bsonReader.ReadName("value");

            // Act
            var result = converter.Deserialize(BsonDeserializationContext.CreateRoot(bsonReader), deserializationArgs);

            // Assert
            result.Should().Be(input, because: "deserialization should not modify valid decimal values");
            GetSignBit(result).Should().Be(
                    GetSignBit(input),
                    because: "the sign bit should be the same for the input and output values");
        }

        private static int GetSignBit(decimal value)
        {
            return decimal.GetBits(value)[3];
        }
    }
}
