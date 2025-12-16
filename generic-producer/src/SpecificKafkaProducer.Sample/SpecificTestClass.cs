namespace SpecificProducer.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Avro;
    using Avro.Specific;

    public class SpecificTestClass : ISpecificRecord
    {
        public static Schema _SCHEMA = Schema.Parse(@"
            {
                ""type"": ""record"",
                ""name"": ""Price"",
                ""namespace"": ""SpecificProducer.Sample"",
                ""fields"": [
                    {""name"": ""id"", ""type"": ""string""},
                    {""name"": ""series_id"", ""type"": ""string""},
                    {""name"": ""price"", ""type"": ""double""},
                    {""name"": ""date"", ""type"": ""long""}
                ]
            }");

        public virtual Schema Schema => _SCHEMA;

        public string Id { get; set; }

        public string SeriesId { get; set; }

        public double PriceValue { get; set; }

        public DateTimeOffset Date { get; set; }

        public virtual object Get(int fieldPos)
        {
            switch (fieldPos)
            {
                case 0:
                    return Id;
                case 1:
                    return SeriesId;
                case 2:
                    return PriceValue;
                case 3:
                    return Date.ToUnixTimeSeconds();
                default:
                    throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
            }
        }
       
        public virtual void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0:
                    Id = (string)fieldValue;
                    break;
                case 1:
                    SeriesId = (string)fieldValue;
                    break;
                case 2:
                    PriceValue = (double)fieldValue;
                    break;
                case 3:
                    Date = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                default:
                    throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            }
        }
    }

    public class MetaFields
    {
        public string Id { get; set; }
        public string CoorelationId { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string EventType { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public DateTimeOffset CreatedFor { get; set; }
        public DateTimeOffset ReleasedOn { get; set; }
    }

    public class PriceSeries : MetaFields, ISpecificRecord
    {
        public static readonly Schema _SCHEMA = Schema.Parse(@"{
                ""type"": ""record"",
                ""name"": ""Series"",
                ""namespace"": ""SpecificProducer.Sample"",
                ""fields"": [
                    {""name"" : ""id"",""type"" : ""string""},
                    {""name"" : ""correlation_id"",""type"" : ""string""},
                    {""name"" : ""source"",""type"" : ""string""},
                    {""name"" : ""type"",""type"" : ""string""},
                    {""name"" : ""event_type"",""type"" : ""string""},
                    {""name"" : ""event_time"",""type"" : ""long"",""logicalType"": ""timestamp-millis""},
                    {""name"" : ""created_for"",""type"" : ""long"",""logicalType"": ""timestamp-millis""},
                    {""name"" : ""released_on"",""type"" : ""long"",""logicalType"": ""timestamp-millis""},
                    {""name"" : ""versionNumber"", ""type"": ""int""},
                    {""name"" : ""versionDate"", ""type"": ""long""},
                    {""name"" : ""seriesVersionNumber"", ""type"": ""int""},                    
                    {""name"" : ""seriesId"", ""type"": ""string""},
                    {""name"" : ""createdOn"", ""type"": ""long""},
                    {""name"" : ""embargoDate"", ""type"": ""long""},
                    {""name"" : ""commodity"", ""type"": ""string""},
                    {""name"" : ""region"", ""type"": ""string""},
                    {""name"" : ""currency"", ""type"": ""string""},
                    {""name"" : ""measure"", ""type"": ""string""},
                    {""name"" : ""seriesOrder"", ""type"": ""long""},
                    {""name"" : ""low"", ""type"": ""double""},
                    {""name"" : ""mid"", ""type"": ""double""},
                    {""name"" : ""high"", ""type"": ""double""}
                ]
            }");

        public virtual Schema Schema => _SCHEMA;

        public int VersionNumber { get; set; }
        public DateTimeOffset VersionDate { get; set; }
        public int SeriesVersionNumber { get; set; }       
        public string SeriesId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset EmbargoDate { get; set; }
        public string Commodity { get; set; }
        public string Region { get; set; }
        public string Currency { get; set; }
        public string MeasureUnit { get; set; }
        public DateTimeOffset SeriesOrder { get; set; }
        public double Low { get; set; }
        public double Mid { get; set; }
        public double High { get; set; }

        public virtual object Get(int fieldPos)
        {
            switch (fieldPos)
            {
                case 0:
                    return Id;
                case 1:
                    return CoorelationId;
                case 2:
                    return Source;
                case 3:
                    return Type;
                case 4:
                    return EventType;
                case 5:
                    return EventTime.ToUnixTimeSeconds();
                case 6:
                    return CreatedFor.ToUnixTimeSeconds();
                case 7:
                    return ReleasedOn.ToUnixTimeSeconds();
                case 8:
                    return VersionNumber;
                case 9:
                    return VersionDate.ToUnixTimeSeconds();
                case 10:
                    return SeriesVersionNumber;
                case 11:
                    return SeriesId;
                case 12:
                    return CreatedOn.ToUnixTimeSeconds();
                case 13:
                    return EmbargoDate.ToUnixTimeSeconds();
                case 14:
                    return Commodity;
                case 15:
                    return Region;
                case 16:
                    return Currency;
                case 17:
                    return MeasureUnit;
                case 18:
                    return SeriesOrder.ToUnixTimeSeconds();
                case 19:
                    return Low;
                case 20:
                    return Mid;
                case 21:
                    return High;
                default:
                    throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
            };
        }
       
        public virtual void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0:
                    Id = (string)fieldValue;
                    break;
                case 1:
                    CoorelationId = (string)fieldValue;
                    break;
                case 2:
                    Source = (string)fieldValue;
                    break;
                case 3:
                    Type = (string)fieldValue;
                    break;
                case 4:
                    EventType = (string)fieldValue;
                    break;
                case 5:
                    EventTime = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                case 6:
                    CreatedFor = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                case 7:
                    ReleasedOn = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                case 8:
                    VersionNumber = (int)fieldValue;
                    break;
                case 9:
                    VersionDate = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                case 10:
                    SeriesVersionNumber = (int)fieldValue;
                    break;
                case 11:
                    SeriesId = (string)fieldValue;
                    break;
                case 12:
                    CreatedOn = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                case 13:
                    EmbargoDate = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                case 14:
                    Commodity = (string)fieldValue;
                    break;
                case 15:
                    Region = (string)fieldValue;
                    break;
                case 16:
                    Currency = (string)fieldValue;
                    break;
                case 17:
                    MeasureUnit = (string)fieldValue;
                    break;
                case 18:
                    SeriesOrder = DateTimeOffset.FromUnixTimeSeconds((long)fieldValue);
                    break;
                case 19:
                    Low = (double)fieldValue;
                    break;
                case 20:
                    Mid = (double)fieldValue;
                    break;
                case 21:
                    High = (double)fieldValue;
                    break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            };
        }
    }
}
