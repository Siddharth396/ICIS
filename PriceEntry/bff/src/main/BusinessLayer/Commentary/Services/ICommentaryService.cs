namespace BusinessLayer.Commentary.Services
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.DTOs.Input;
    using BusinessLayer.Commentary.DTOs.Output;
    using BusinessLayer.Commentary.Repositories.Models;

    using Infrastructure.Services.Workflow;

    public interface ICommentaryService
    {
        Task<CommentaryOutput?> GetCommentary(string contentBlockId, DateTime assessedDateTime);

        Task HandleDataPackageUpdatedEvent(
            WorkflowStatus dataPackageStatus,
            string contentBlockId,
            DateTime assessedDateTime);

        Task<Commentary> SaveCommentary(CommentaryInput commentaryInput);
    }
}
