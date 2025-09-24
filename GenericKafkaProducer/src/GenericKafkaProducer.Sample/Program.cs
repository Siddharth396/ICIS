namespace KafkaProducer.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using Microsoft.Extensions.Options;
    using Serilog;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            Console.WriteLine("Welcome to Generic Kafka Producer Sample application!");

            try
            {
                IKafkaProducer kafkaProducer = new KafkaProducer(Options.Create(GetKafkaProducerOptions()));

                int count = 10;

                for (int i = 0; i <= count; i++)
                {
                    await kafkaProducer.ProduceAsync(GetSampleKafkaMessage1(), CancellationToken.None);
                    Console.WriteLine("Published message in Kafka - " + i);
                }

                kafkaProducer = new KafkaProducer(Options.Create(GetKafkaProducerOptions()), new ErrorHandler());
                for (int i = 0; i < 25; i++)
                {
                    await kafkaProducer.ProduceAsync(GetSampleKafkaMessage1(), CancellationToken.None);
                    Console.WriteLine("Published message in Kafka - " + i);
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Method to populate sample kafka Message.
        /// </summary>
        /// <returns></returns>
        private static KafkaMessage GetSampleKafkaMessage1()
        {
            var headers = new Dictionary<string, string>
            {
                { "correlation_id", Guid.NewGuid().ToString() }
            };

            var schema = @"{
	            ""type"": ""record"",
	            ""name"": ""primitive"",
	            ""namespace"": ""KafkaProducer.KafkaProducer"",
	            ""fields"": [
				            {
			            ""name"": ""string"",
			            ""type"": ""string""
		            },
		            {
			            ""name"": ""dateTime"",
			            ""type"": ""double"",
			            ""logicalType"": ""timestamp-millis""
		            },
		            {
			            ""name"": ""number"",
			            ""type"": ""long""
		            },
		            {
			            ""name"": ""boolean"",
			            ""type"": ""boolean""
		            }
	            ]
            }";

            var messageValue = @"{
	            ""string"": ""test"",
	            ""dateTime"": 1627600729001,
	            ""number"": 1231232322343423,
	            ""boolean"": true
            }";

            var kafkaMessage = new KafkaMessage()
            {
                TopicName = "TestGenericTopic",
                Headers = headers,
                MessageKey = $"Test_{Guid.NewGuid()}_Published",
                MessageValue = messageValue,
                PartitionCount = 1,
                PartitionKey = "testPartition",
                Schema = schema // Comment this line if we want to publish data in JSON format
            };

            return kafkaMessage;
        }

        /// <summary>
        /// Method to populate sample kafka Message.
        /// </summary>
        /// <returns></returns>
        private static KafkaMessage GetSampleKafkaMessage()
        {
            var headers = new Dictionary<string, string>
            {
                { "correlation_id", Guid.NewGuid().ToString() }
            };

            var schema = @"{
              ""fields"": [
                {
                  ""name"": ""model_run_uid"",
                  ""type"": ""int""
                },
                {
                  ""name"": ""metadata"",
                  ""type"": {
                    ""type"": ""map"",
                    ""values"": ""string""
                  }
                },
                {
                  ""name"": ""model_name"",
                  ""type"": ""string""
                },
                {
                  ""name"": ""event_trigger"",
                  ""type"": ""string""
                },
                {
                  ""logicalType"": ""timestamp-millis"",
                  ""name"": ""version_datetime"",
                  ""type"": ""double""
                },
                {
                  ""name"": ""publication_status"",
                  ""type"": ""string""
                },
                {
                  ""name"": ""scenario_name"",
                  ""type"": ""string""
                },
                {
                  ""name"": ""series_items"",
                  ""type"": {
                    ""items"": {
                      ""fields"": [
                        {
                          ""logicalType"": ""timestamp-millis"",
                          ""name"": ""applies_from"",
                          ""type"": ""double""
                        },
                        {
                          ""name"": ""output_1_forecast_series_code"",
                          ""type"": ""string""
                        },
                        {
                          ""name"": ""output_1_forecast_short_name"",
                          ""type"": ""string""
                        },
                        {
                          ""name"": ""output_1_forecast_long_name"",
                          ""type"": ""string""
                        },
                        {
                          ""name"": ""output_1_forecast_unadjusted_forecast"",
                          ""type"": ""float""
                        },
                        {
                          ""name"": ""output_1_forecast_adjustment"",
                          ""type"": ""float""
                        },
                        {
                          ""name"": ""output_1_forecast_adjusted_forecast"",
                          ""type"": ""float""
                        },
                        {
                          ""default"": null,
                          ""name"": ""output_1_forecast_adjustment_comment"",
                          ""type"": [
                            ""string"",
                            ""null""
                          ]
                        },
                        {
                          ""default"": null,
                          ""name"": ""output_1_forecast_latest_published_adjusted_forecast"",
                          ""type"": [
                            ""float"",
                            ""null""
                          ]
                        },
                        {
                          ""default"": null,
                          ""name"": ""total_input_cost"",
                          ""type"": [
                            ""float"",
                            ""null""
                          ]
                        },
                        {
                          ""default"": null,
                          ""name"": ""input_1_forecast_series_code"",
                          ""type"": [
                            ""string"",
                            ""null""
                          ]
                        },
                        {
                          ""default"": null,
                          ""name"": ""input_1_forecast_short_name"",
                          ""type"": [
                            ""string"",
                            ""null""
                          ]
                        },
                        {
                          ""default"": null,
                          ""name"": ""input_1_forecast_price"",
                          ""type"": [
                            ""float"",
                            ""null""
                          ]
                        }
                      ],
                      ""name"": ""series_item"",
                      ""type"": ""record""
                    },
                    ""type"": ""array""
                  }
                }
              ],
              ""name"": ""ModelForecastPrices"",
              ""namespace"": ""ADP.Kafka.Entities"",
              ""type"": ""record""
            }";

            var messageValue = @"{
  ""model_run_uid"": 3295,
  ""metadata"": {
    ""event_type"": ""Published"",
    ""partition_key"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
    ""correlation_id"": ""388e6100-6dc1-47b6-a84a-b8ca141f6af9"",
    ""id"": ""5b4f2f3f-2549-4817-a69a-4e8149bba76b"",
    ""source"": ""ADP"",
    ""type"": ""ADP.Kafka.Entities.ModelForecastPrices"",
    ""event_time"": ""1627304499261""
  },
  ""model_name"": ""mod_price_europe_mediterranean_gas_oil_spot_v1"",
  ""event_trigger"": ""model_run"",
  ""version_datetime"": 1627304497.958459,
  ""publication_status"": ""Draft"",
  ""scenario_name"": ""base"",
  ""series_items"": [
    {
      ""applies_from"": 1625097600.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 618.8817,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 618.8817,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 624.6046
      }
    },
    {
      ""applies_from"": 1627776000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 604.1177,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 604.1177,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 620.0999
      }
    },
    {
      ""applies_from"": 1630454400.0002,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 599.31305,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 599.31305,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 616.026
      }
    },
    {
      ""applies_from"": 1633046400.0005466756346476753657,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 603.44727,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 603.44727,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 612.7636
      }
    },
    {
      ""applies_from"": 1635724800.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 602.38855,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 602.38855,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 611.2931
      }
    },
    {
      ""applies_from"": 1638316800.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 577.7518,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 577.7518,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 580.8932
      }
    },
    {
      ""applies_from"": 1640995200.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 634.9174,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 634.9174,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 611.8904
      }
    },
    {
      ""applies_from"": 1643673600.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 643.0685,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 643.0685,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 610.3356
      }
    },
    {
      ""applies_from"": 1646092800.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 549.8397,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 549.8397,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 569.8538
      }
    },
    {
      ""applies_from"": 1648771200.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 634.84625,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 634.84625,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 635.9518
      }
    },
    {
      ""applies_from"": 1651363200.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 693.9822,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 693.9822,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 651.3586
      }
    },
    {
      ""applies_from"": 1654041600.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 642.9962,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 642.9962,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 629.78
      }
    },
    {
      ""applies_from"": 1656633600.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 623.0352,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 623.0352,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 628.7965
      }
    },
    {
      ""applies_from"": 1659312000.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 626.84894,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 626.84894,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 643.3497
      }
    },
    {
      ""applies_from"": 1661990400.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 604.20404,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 604.20404,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 621.0534
      }
    },
    {
      ""applies_from"": 1664582400.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 608.71466,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 608.71466,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 618.1123
      }
    },
    {
      ""applies_from"": 1667260800.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 607.8214,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 607.8214,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 616.8063
      }
    },
    {
      ""applies_from"": 1669852800.000,
      ""output_1_forecast_series_code"": ""5e28b7af-aebf-4886-a443-513631eb951f"",
      ""output_1_forecast_short_name"": ""gas oil europe mediterranean"",
      ""output_1_forecast_long_name"": ""gas oil europe mediterranean forecast usd cif spot mt"",
      ""output_1_forecast_unadjusted_forecast"": 612.9218,
      ""output_1_forecast_adjustment"": 0,
      ""output_1_forecast_adjusted_forecast"": 612.9218,
      ""output_1_forecast_adjustment_comment"": null,
      ""output_1_forecast_latest_published_adjusted_forecast"": null,
      ""total_input_cost"": null,
      ""input_1_forecast_series_code"": {
        ""string"": ""6e7a576a-0d76-4812-b51a-b8cf0fab03bd""
      },
      ""input_1_forecast_short_name"": {
        ""string"": ""gas oil europe north west""
      },
      ""input_1_forecast_price"": {
        ""float"": 615.0022
      }
    }
  ]
}";

            var kafkaMessage = new KafkaMessage()
            {
                TopicName = "TestGenericTopic",
                Headers = headers,
                MessageKey = $"Test_{Guid.NewGuid()}_Published",
                MessageValue = messageValue,
                PartitionCount = 1,
                Schema = schema // Comment this line if we want to publish data in JSON format
            };

            return kafkaMessage;
        }

        /// <summary>
        /// Method to populate Schema registry and Avro serializer config.
        /// </summary>
        /// <returns></returns>
        private static KafkaProducerOptions GetKafkaProducerOptions()
        {
            var kafkaProducerOptions = new KafkaProducerOptions()
            {
                SchemaRegistry = schemaRegistryConfig,
                AvroSerializer = AvroSerializerConfig,
                BootstrapServers = "localhost:31093",
                EnableIdempotence = true
            };

            return kafkaProducerOptions;
        }

        private static readonly AvroSerializerConfig AvroSerializerConfig = new AvroSerializerConfig()
        {
            AutoRegisterSchemas = true
        };

        private static SchemaRegistryConfig schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = "localhost:8089",
            RequestTimeoutMs = 5000,
            MaxCachedSchemas = 10,
            BasicAuthUserInfo = string.Empty
        };
    }
}