namespace BusinessLayer.DataPackage.Handler
{
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.Services;

    using Infrastructure.EventDispatcher;

    using Serilog;

    internal class UpdateCommentaryEventHandler : IEventHandler<DataPackageUpdatedEvent>
    {
        private readonly ILogger logger;

        private readonly ICommentaryService commentaryService;

        public UpdateCommentaryEventHandler(
            ILogger logger,
            ICommentaryService commentaryService)
        {
            this.logger = logger.ForContext<UpdateCommentaryEventHandler>();
            this.commentaryService = commentaryService;
        }

        public async Task HandleAsync(DataPackageUpdatedEvent @event)
        {
            var dataPackageStatus = @event.Status;

            var localLogger = logger.ForContext(
                "DataPackageUpdateEvent Received, Handling CommentaryUpdates",
                dataPackageStatus);

            localLogger.Debug("DataPackageUpdateEvent Received for Status {status}", dataPackageStatus);

            await commentaryService.HandleDataPackageUpdatedEvent(
                dataPackageStatus,
                @event.DataPackage.ContentBlock.Id,
                @event.DataPackage.AssessedDateTime);
        }
    }
}