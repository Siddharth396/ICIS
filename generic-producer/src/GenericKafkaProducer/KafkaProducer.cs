namespace KafkaProducer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Avro;
    using Avro.Generic;
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using Icis.GenericKafkaProducer.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serilog;
    using JsonException = Newtonsoft.Json.JsonException;
    using Schema = Avro.Schema;

    public sealed class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> jsonProducer;
        private readonly IProducer<string, GenericRecord> avroProducer;
        private readonly ISchemaRegistryClient schemaRegistry;

        public KafkaProducer(IOptions<KafkaProducerOptions> options)
        {
            jsonProducer = new ProducerBuilder<string, string>(options.Value)
                .SetErrorHandler((p, error) =>
                {
                    Log.Error(error.ToString());
                })
                .SetLogHandler((p, message) =>
                {
                    Log.Information(message.ToString());
                })
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            if (options.Value.IsSchemaRegistryConfigured())
            {
                schemaRegistry = new CachedSchemaRegistryClient(options.Value.SchemaRegistry);

                avroProducer = new ProducerBuilder<string, GenericRecord>(options.Value)
                   .SetErrorHandler((p, error) =>
                   {
                       Log.Error(error.ToString());
                   })
                   .SetLogHandler((p, message) =>
                   {
                       Log.Information(message.ToString());
                   })
                   .SetKeySerializer(Serializers.Utf8)
                   .SetValueSerializer(new AvroSerializer<GenericRecord>(schemaRegistry, options.Value.AvroSerializer))
                   .Build();
            }
        }

        /// <summary>
        /// To enable circuit breaker pattern the consuming application if implements 
        /// the IErrorHandler Interfaces and registers in the DI, 
        /// the circuit breaker constructor will be called where 
        /// the consuming application has to handle the errors in the way desired
        /// </summary>
        /// <param name="options">Otions to connect kafka broker</param>
        /// <param name="errorHandler">To handle errors when kafka broker is down.</param>
        public KafkaProducer(IOptions<KafkaProducerOptions> options, IErrorHandler errorHandler)
        {
            jsonProducer = new ProducerBuilder<string, string>(options.Value)
                .SetErrorHandler(errorHandler.ErrorHandlerForJson)
                .SetLogHandler((p, message) =>
                {
                    Log.Information(message.ToString());
                })
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            if (options.Value.IsSchemaRegistryConfigured())
            {
                schemaRegistry = new CachedSchemaRegistryClient(options.Value.SchemaRegistry);

                avroProducer = new ProducerBuilder<string, GenericRecord>(options.Value)
                   .SetErrorHandler(errorHandler.ErrorHandlerForGenericRecord)
                   .SetLogHandler((p, message) =>
                   {
                       Log.Information(message.ToString());
                   })
                   .SetKeySerializer(Serializers.Utf8)
                   .SetValueSerializer(new AvroSerializer<GenericRecord>(schemaRegistry, options.Value.AvroSerializer))
                   .Build();
            }
        }

        /// <summary>
        /// Method to produce generic kafka message
        /// </summary>
        /// <param name="job"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ProducerResult> ProduceAsync(KafkaMessage kafkaMessage, CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(kafkaMessage.Schema))
            {
                var value = CreateAvroRecord(kafkaMessage.MessageValue, kafkaMessage.Schema);

                var message = new Message<string, GenericRecord> { Value = value };

                var result = await ProduceAsync(avroProducer, message, kafkaMessage, ct);

                return new ProducerResult(result.TopicPartitionOffset, result.Status);
            }
            else
            {
                var message = new Message<string, string> { Value = kafkaMessage.MessageValue };

                var result = await ProduceAsync(jsonProducer, message, kafkaMessage, ct);

                return new ProducerResult(result.TopicPartitionOffset, result.Status);
            }
        }

        public void Dispose()
        {
            jsonProducer?.Dispose();
            avroProducer?.Dispose();
            schemaRegistry?.Dispose();
        }

        /// <summary>
        /// Method to produce generic message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="producer"></param>
        /// <param name="message"></param>
        /// <param name="job"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private static async Task<DeliveryResult<string, T>> ProduceAsync<T>(IProducer<string, T> producer, Message<string, T> message, KafkaMessage kafkaMessage, CancellationToken ct)
        {
            message.Key = kafkaMessage.MessageKey;

            if (kafkaMessage.Headers?.Count > 0)
            {
                message.Headers = new Headers();

                foreach (var header in kafkaMessage.Headers)
                {
                    message.Headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
                }
            }

            AppendTraceparentInHeader(message.Headers);

            if (!string.IsNullOrWhiteSpace(kafkaMessage.PartitionKey) && kafkaMessage.PartitionCount > 0)
            {
                var partitionkeyByte = Encoding.UTF8.GetBytes(kafkaMessage.PartitionKey);
                var partition = Math.Abs(BitConverter.ToInt32(MD5.Create().ComputeHash(partitionkeyByte), 0) % kafkaMessage.PartitionCount);

                return await producer.ProduceAsync(new TopicPartition(kafkaMessage.TopicName, partition), message, ct);
            }
            else
            {
                return await producer.ProduceAsync(kafkaMessage.TopicName, message, ct);
            }
        }
      
        /// <summary>
        /// Method to get type values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static object GetValue(dynamic value, Schema schema)
        {
            var type = GetType(value);

            switch (type)
            {
                case JTokenType.String when IsTypeOrUnionWith(schema, Schema.Type.String):
                    return value.Value.ToString();

                case JTokenType.Integer when IsTypeOrUnionWith(schema, Schema.Type.Int):
                    return Convert.ToInt32(value.Value);

                case JTokenType.Integer when IsTypeOrUnionWith(schema, Schema.Type.Long):
                    return Convert.ToInt64(value.Value);

                case JTokenType.Integer when IsTypeOrUnionWith(schema, Schema.Type.Float):
                    return Convert.ToSingle(value.Value);

                case JTokenType.Integer when IsTypeOrUnionWith(schema, Schema.Type.Double):
                    return Convert.ToDouble(value.Value);

                case JTokenType.Boolean when IsTypeOrUnionWith(schema, Schema.Type.Boolean):
                    return Convert.ToBoolean(value.Value);

                case JTokenType.Float when IsTypeOrUnionWith(schema, Schema.Type.Float):
                    return Convert.ToSingle(value.Value);

                case JTokenType.Float when IsTypeOrUnionWith(schema, Schema.Type.Double):
                    return Convert.ToDouble(value.Value);

                case JTokenType.Object when IsTypeOrUnionWith(schema, Schema.Type.Record):
                    {
                        var o = new JObject(value);

                        GenericRecord result = null;

                        if (schema is UnionSchema union)
                        {
                            var record = (RecordSchema)union.Schemas.FirstOrDefault(x => x.Tag == Schema.Type.Record);

                            result = new GenericRecord(record);

                            foreach (KeyValuePair<string, JToken> data in o)
                            {
                                if (record != null && record.TryGetField(data.Key, out var field))
                                {
                                    result.Add(data.Key, GetValue(data.Value, field.Schema));
                                }
                            }
                        }
                        else if (schema is RecordSchema record)
                        {
                            result = new GenericRecord(record);

                            foreach (KeyValuePair<string, JToken> data in o)
                            {
                                if (record.TryGetField(data.Key, out var field))
                                {
                                    result.Add(data.Key, GetValue(data.Value, field.Schema));
                                }
                            }
                        }

                        return result;
                    }

                case JTokenType.Object when IsTypeOrUnionWith(schema, Schema.Type.Map):
                    {
                        var o = new JObject(value);
                        var mapResult = new Dictionary<string, object>();

                        if (schema is UnionSchema union)
                        {
                            var map = (MapSchema)union.Schemas.FirstOrDefault(x => x.Tag == Schema.Type.Map);

                            foreach (KeyValuePair<string, JToken> data in o)
                            {
                                mapResult.Add(data.Key, GetValue(data.Value, map?.ValueSchema));
                            }
                        }
                        else if (schema is MapSchema map)
                        {
                            foreach (KeyValuePair<string, JToken> data in o)
                            {
                                mapResult.Add(data.Key, GetValue(data.Value, map?.ValueSchema));
                            }
                        }

                        return mapResult;
                    }

                case JTokenType.Array when IsTypeOrUnionWith(schema, Schema.Type.Array):
                    {
                        var a = new JArray(value);
                        var result = new List<object>();

                        if (schema is UnionSchema union)
                        {
                            var arraySchema =
                                (ArraySchema)union.Schemas.FirstOrDefault(x => x.Tag == Schema.Type.Array);

                            foreach (var item in a)
                            {
                                result.Add(GetValue(item, arraySchema?.ItemSchema));
                            }
                        }
                        else if (schema is ArraySchema array)
                        {
                            foreach (var item in a)
                            {
                                result.Add(GetValue(item, array.ItemSchema));
                            }
                        }

                        return result.ToArray();
                    }
            }

            return null;
        }

        /// <summary>
        /// Method to return value type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static dynamic GetType(dynamic value)
        {
            var type = value.GetType();

            if (type == null)
            {
                type = ((JObject)value).Type;
            }
            else if (type == typeof(JArray))
            {
                type = ((JArray)value).Type;
            }
            else if (type == typeof(JValue))
            {
                type = ((JValue)value).Type;
            }
            else
            {
                type = ((JObject)value).Type;
            }

            return type;
        }

        private static bool IsTypeOrUnionWith(Schema schema, Schema.Type expected)
        {
            return schema.Tag == expected || (schema is UnionSchema union && union.Schemas.Any(x => x.Tag == expected));
        }

        private static void AppendTraceparentInHeader(Headers messageheaders)
        {
            messageheaders ??= new Headers();

            var dtd = Elastic.Apm.Agent.Tracer?.CurrentTransaction?.OutgoingDistributedTracingData;

            if (dtd != null && !messageheaders.Any(t => t.Key == "traceparent"))
            {
                messageheaders.Add("traceparent", Encoding.UTF8.GetBytes(dtd.SerializeToString()));
            }
        }

        /// <summary>
        /// Method to crete Avro record
        /// </summary>
        /// <param name="json"></param>
        /// <param name="avroSchema"></param>
        /// <returns></returns>
        private GenericRecord CreateAvroRecord(string json, string avroSchema)
        {
            try
            {
                var schema = (RecordSchema)Schema.Parse(avroSchema);

                var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);

                var result = (GenericRecord)GetValue(jsonObject, schema);

                return result;
            }
            catch (JsonException ex)
            {
                Log.Error(ex, $"Failed to parse json: {json}, got {ex.Message}");
                throw new InvalidOperationException($"Failed to parse json: {json}, got {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
        }
    }
}
