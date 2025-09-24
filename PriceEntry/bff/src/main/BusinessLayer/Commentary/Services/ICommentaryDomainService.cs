namespace BusinessLayer.Commentary.Services
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.Repositories.Models;

    public interface ICommentaryDomainService
    {
        Task CommentaryCorrectionCancelled(string contentBlockId, DateTime assessedDateTime);

        Task CommentaryCorrectionPublished(string contentBlockId, DateTime assessedDateTime);

        Task<Commentary?> GetCommentary(string contentBlockId, DateTime assessedDateTime);

        Task<Commentary?> GetCurrentCommentary(string contentBlockId, DateTime assessedDateTime);

        Task<Commentary> SaveCorrectionCommentary(Commentary commentary);

        Task<Commentary> SaveNormalCommentary(Commentary commentary);
    }
}
