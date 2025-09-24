namespace Authoring.Tests.BusinessLayer.EventHandlers
{
    using System;
    using System.Threading.Tasks;

    using global::BusinessLayer.ContentBlock.Services;
    using global::BusinessLayer.PriceEntry.Handler;

    using global::Infrastructure.PubNub;
    using global::Infrastructure.PubNub.Models;
    using global::Infrastructure.Services.Workflow;

    using NSubstitute;

    using Test.Infrastructure.TestData;

    using Xunit;

    public class SendPubNubEventHandlerTests
    {
        private const string PriceSeriesId = "afd23d9d-b2c9-49e6-a61d-6b42d4a2e780";

        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly IPubNubNotificationService pubNubNotificationService;

        private readonly IContentBlockService contentBlockService;

        public SendPubNubEventHandlerTests()
        {
            pubNubNotificationService = Substitute.For<IPubNubNotificationService>();
            contentBlockService = Substitute.For<IContentBlockService>();
        }

        [Fact]
        public async Task Verify_PubNubNotificationService_Is_Called()
        {
            contentBlockService.GetContentBlockIds(PriceSeriesId)
               .Returns(["c44aa19e-fda4-4138-8e4f-cf91912c61d6"]);

            var priceItemEventService = new SendPubNubEventHandler(pubNubNotificationService, contentBlockService);

            await priceItemEventService.HandleAsync(new PriceSeriesItemSavedEvent { SeriesId = PriceSeriesId, AssessedDateTime = AssessedDateTime,  OperationType = OperationType.None });

            pubNubNotificationService.Received(1).AddPubNubNotification(Arg.Any<PubNubData<PriceItemEvent>>());
        }

        [Fact]
        public async Task Verify_PubNubNotificationService_Is_Not_Called()
        {
            contentBlockService.GetContentBlockIds(PriceSeriesId)
               .Returns([]);

            var priceItemEventService = new SendPubNubEventHandler(pubNubNotificationService, contentBlockService);

            await priceItemEventService.HandleAsync(new PriceSeriesItemSavedEvent { SeriesId = PriceSeriesId, AssessedDateTime = AssessedDateTime, OperationType = OperationType.None });

            pubNubNotificationService.Received(0).AddPubNubNotification(Arg.Any<PubNubData<PriceItemEvent>>());
        }
    }
}
