namespace BusinessLayer.Commentary.Services
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.DTOs.Input;
    using BusinessLayer.Commentary.DTOs.Output;
    using BusinessLayer.Commentary.Repositories.Models;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.Services.Workflow;

    using Serilog;

    [ApplicationService]
    public class CommentaryService : ICommentaryService
    {
        private readonly ICommentaryDomainService commentaryDomainService;

        private readonly ILogger logger;

        public CommentaryService(
            ILogger logger,
            ICommentaryDomainService commentaryDomainService)
        {
            this.commentaryDomainService = commentaryDomainService;
            this.logger = logger.ForContext<CommentaryService>();
        }

        public async Task<CommentaryOutput?> GetCommentary(string contentBlockId, DateTime assessedDateTime)
        {
            var localLogger = logger
               .ForContext("ContentBlockId", contentBlockId)
               .ForContext("AssessedDateTime", assessedDateTime);

            var commentary = await commentaryDomainService.GetCurrentCommentary(contentBlockId, assessedDateTime);
            if (commentary != null)
            {
                return new CommentaryOutput { CommentaryId = commentary.CommentaryId, Version = commentary.Version };
            }

            localLogger
               .ForContext("Scenario", "NotFound")
               .Debug($"Commentary not found for contentBlockId {contentBlockId} and date {assessedDateTime}");

            return null;
        }

        public async Task HandleDataPackageUpdatedEvent(WorkflowStatus dataPackageStatus, string contentBlockId, DateTime assessedDateTime)
        {
            if (dataPackageStatus == WorkflowStatus.Cancelled)
            {
                await commentaryDomainService.CommentaryCorrectionCancelled(
                    contentBlockId,
                    assessedDateTime);
            }
            else if (dataPackageStatus == WorkflowStatus.CorrectionPublished)
            {
                await commentaryDomainService.CommentaryCorrectionPublished(
                    contentBlockId,
                    assessedDateTime);
            }
        }

        public async Task<Commentary> SaveCommentary(CommentaryInput commentaryInput)
        {
            var id = Guid.NewGuid().ToString();

            var inputCommentary = new Commentary
            {
                Id = id,
                CommentaryId = commentaryInput.CommentaryId,
                ContentBlockId = commentaryInput.ContentBlockId,
                AssessedDateTime = commentaryInput.AssessedDateTime,
                Version = commentaryInput.Version
            };

            if (OperationType.Correction.Matches(commentaryInput.OperationType))
            {
                return await commentaryDomainService.SaveCorrectionCommentary(inputCommentary);
            }

            return await commentaryDomainService.SaveNormalCommentary(inputCommentary);
        }
    }
}
