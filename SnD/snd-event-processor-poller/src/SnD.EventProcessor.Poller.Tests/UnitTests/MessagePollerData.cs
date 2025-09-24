using Amazon.SQS.Model;
using SnD.EventProcessor.Poller.Model;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace SnD.EventProcessor.Poller.Tests.UnitTests
{
    public class MessagePollerData
    {
        public int BatchSize = 10;
        public string TestQueueUrl = "test_queue_url";
        public string[] SndQueueNames = new string[] { "queue_1" };
        public CancellationToken Token = new CancellationToken();

        public readonly PollerConfig PollerConfig = new PollerConfig
        {
            IntervalInSeconds = 1,
            MaxInitialDelayInSeconds = 1,
            MessageBatchSize = 10
        };

        public List<Message> MessageList = new List<Message>
        {
            new Message
            {
                ReceiptHandle =  "test_receipt_handle",
                Body = JsonSerializer.Serialize(
                    new PublishMessage
                    {
                        SnDMessage = JsonSerializer.Serialize(
                            new SnDEvent
                            {
                                EventType = "updated",
                                EntityType = "snd-company",
                                EntityId = 101
                            })
                    })
            }
         };
    }
}

