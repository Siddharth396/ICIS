namespace KafkaProducer.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Avro.Generic;
    using Confluent.Kafka;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using Microsoft.Extensions.Options;
    using Moq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class KafkaProducerTest
    {
        private static readonly Type Type = typeof(KafkaProducer);
        private static readonly AvroSerializerConfig AvroSerializerConfig = new AvroSerializerConfig()
        {
            AutoRegisterSchemas = true
        };

        private static SchemaRegistryConfig schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = "http://localhost:30081",
            RequestTimeoutMs = 100,
            MaxCachedSchemas = 10,
            BasicAuthUserInfo = ""
        };
        private readonly MethodInfo[] methods;
        private object expectedType = null;
        private object jsonObject;
        private object inputType = null;
        private object actualType = null;
       
        private Mock<IErrorHandler> errorHandler;

        public KafkaProducerTest()
        {
            methods = Type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        }

        [Fact]
        public void GetValue_StringTest_Success()
        {
            var schema = @"{""type"": ""string""}";
            var messageValue = @"{""string"": ""test-1""}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = "testValue".GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { inputType, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_IntTest_Success()
        {
            var schema = @"{""type"": ""int""}";
            var messageValue = @"{""Integer"": 342314}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = 342314.GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { inputType, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_LongTest_Success()
        {
            var schema = @"{""type"": ""long""}";
            var messageValue = @"{""long"": 1231232322343423}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = 1231232322343423.GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { inputType, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_FloatTest_Success()
        {
            var schema = @"{""type"": ""float""}";
            var messageValue = @"{""float"": 20.98}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = ((Single)1.1M).GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { inputType, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_DoubleTest_Success()
        {
            var schema = @"{""type"": ""double""}";
            var messageValue = @"{""double"": 51.23}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = 20.98.GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { inputType, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_BooleanTest_Success()
        {
            var schema = @"{""type"": ""boolean""}";
            var messageValue = @"{""boolean"": true}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = true.GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { inputType, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_NullTest_Success()
        {
            var schema = @"{""type"": ""null""}";
            var messageValue = @"{""null"": null}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = null;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema });
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_RecordTest_Success()
        {
            var schema = @"{""type"": ""record"",""name"": ""complex"",""namespace"": ""KafkaProducer.KafkaProducer"",
	                        ""fields"": [{""name"": ""string"",""type"": ""string""},
		                     {""name"": ""dateTime"",""type"": ""double"",""logicalType"": ""timestamp-millis""},
		                     {""name"": ""number"",""type"": ""long""},{""name"": ""boolean"",""type"": ""boolean""}
	                        ]}";
            var messageValue = @"{""string"": ""test-1"",""dateTime"": 1627600729001,""number"": 1231232322343423,""boolean"": true}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = new object().GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema }).GetType().BaseType.Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_ArrayTest_Success()
        {
            var schema = @"{""type"": ""array"",""name"": ""complex"",""namespace"": ""KafkaProducer.KafkaProducer"",
                             ""items"": {""name"": ""NameDetails"",""type"": ""record"",
                               ""fields"": [{""name"": ""id"",""type"": ""int""},
                                            {""name"": ""text"", ""type"": ""string""},
                                            {""name"": ""user_id"",""type"": ""int""}
                                           ]
                                        }}";
            var messageValue = @"[{""id"":1,""text"":""some text"",""user_id"":1},{ ""id"":1,""text"":""some text"",""user_id"":2}]";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = new object[] { "test1", "test2" }.GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_MapTest_Success()
        {
            var schema = @"{""type"": ""map"",""name"": ""sub_rec"",""namespace"": ""KafkaProducer.KafkaProducer"",
                           ""Key"":""string"",""values"" : ""string"",""default"": { }
                           }";
            var messageValue = @"{""sub_rec"" : [
                                                 { ""key1"" : ""value"" }, { ""key2"" : ""value"" }
                                                ]}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = new Dictionary<string, string>() { }.GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema }).GetType().Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_Union_IntNull_Test_Success()
        {
            var schema = @"{ ""type"" : ""record"", ""name"" : ""empdetails "", ""namespace"" : ""KafkaProducer.KafkaProducer"",
                           ""fields"" : [{ ""name"" : ""experience"", ""type"": [""int"", ""null""] },
                                         { ""name"" : ""age"", ""type"": ""int"" }
                                        ] }";
            var messageValue = @"{""experience"": null, ""age"": 28}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = new object().GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema }).GetType().BaseType.Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_Union_IntString_Test_Success()
        {
            var schema = @"{ ""type"" : ""record"", ""name"" : ""empdetails "", ""namespace"" : ""KafkaProducer.KafkaProducer"",
                           ""fields"" : [{ ""name"" : ""experience"", ""type"": [""int"", ""string""] },
                                         { ""name"" : ""age"", ""type"": ""int"" }
                                        ] }";
            var messageValue = @"{""experience"": ""5"", ""age"": 28}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = new object().GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema }).GetType().BaseType.Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_Union_LongFloat_Test_Success()
        {
            var schema = @"{ ""type"" : ""record"", ""name"" : ""empdetails "", ""namespace"" : ""KafkaProducer.KafkaProducer"",
                           ""fields"" : [{ ""name"" : ""experience"", ""type"": [""long"", ""float""] },
                                         { ""name"" : ""age"", ""type"": ""int"" }
                                        ] }";
            var messageValue = @"{""experience"": 20.98, ""age"": 28}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = new object().GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema }).GetType().BaseType.Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetValue_Union_BooleanDouble_Test_Success()
        {
            var schema = @"{ ""type"" : ""record"", ""name"" : ""empdetails "", ""namespace"" : ""KafkaProducer.KafkaProducer"",
                           ""fields"" : [{ ""name"" : ""experience"", ""type"": [""boolean"", ""double""] },
                                         { ""name"" : ""age"", ""type"": ""int"" }
                                        ] }";
            var messageValue = @"{""experience"": 51.89, ""age"": 28}";
            Avro.Schema _schema = Avro.Schema.Parse(schema);
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = new object().GetType().Name;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetValue");
            actualType = method.Invoke(null, new object[] { jsonObject, _schema }).GetType().BaseType.Name;
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetType_StringTypeTest()
        {
            var messageValue = @"{""string"": ""test-123""}";
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = ((JValue)"testValue").Type;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetType");
            actualType = method.Invoke(null, new object[] { inputType });
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetType_BooleanTypeTest()
        {
            var messageValue = @"{""boolean"": true}";
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = ((JValue)true).Type;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetType");
            actualType = method.Invoke(null, new object[] { inputType });
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetType_IntegerTypeTest()
        {
            var messageValue = @"{""Integer"": 342314}";
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = ((JValue)432432).Type;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetType");
            actualType = method.Invoke(null, new object[] { inputType });
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetType_NullTypeTest()
        {
            var messageValue = @"{""null"": null}";
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            inputType = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)jsonObject).First).Value;
            expectedType = JTokenType.Null;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetType");
            actualType = method.Invoke(null, new object[] { inputType });
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetType_RecordTypeTest()
        {
            var messageValue = @"{
	            ""string"": ""test-1"",
	            ""dateTime"": 1627600729001,
	            ""number"": 1231232322343423,
	            ""boolean"": true
            }";
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = ((JObject)jsonObject).Type;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetType");
            actualType = method.Invoke(null, new object[] { jsonObject });
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void GetType_RecordType_With_Type_Field_Test()
        {
            var messageValue = @"{
                ""Type"": ""MyType""
            }";
            jsonObject = JsonConvert.DeserializeObject<dynamic>(messageValue);
            expectedType = ((JObject)jsonObject).Type;
            MethodInfo method = methods.FirstOrDefault(x => x.Name == "GetType");
            actualType = method.Invoke(null, new object[] { jsonObject });
            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void ThrowError_When_KafkaBrokerDown()
        {
            errorHandler = new Mock<IErrorHandler>();
            var kafkaGenericProducer = new KafkaProducer(Options.Create(GetKafkaProducerOptions()), errorHandler.Object);
            Thread.Sleep(5000);
            errorHandler.Verify(er => er.ErrorHandlerForGenericRecord(It.IsAny<IProducer<string, GenericRecord>>(), It.IsAny<Error>()), Times.AtLeastOnce);
            errorHandler.Verify(er => er.ErrorHandlerForJson(It.IsAny<IProducer<string, string>>(), It.IsAny<Error>()), Times.AtLeastOnce);
        }

        private static KafkaProducerOptions GetKafkaProducerOptions()
        {
            var kafkaProducerOptions = new KafkaProducerOptions()
            {
                SchemaRegistry = schemaRegistryConfig,
                AvroSerializer = AvroSerializerConfig,
                BootstrapServers = "localhost:30091",
                EnableIdempotence = true,
                MessageTimeoutMs = 60000
            };

            return kafkaProducerOptions;
        }
    }
}