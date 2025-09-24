namespace BusinessLayer.Commentary.Services
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.Repositories;
    using BusinessLayer.Commentary.Repositories.Models;

    using Infrastructure.Attributes.BusinessAnnotations;

    [DomainService]
    public class CommentaryDomainService : ICommentaryDomainService
    {
        private readonly CommentaryRepository commentaryRepository;

        public CommentaryDomainService(CommentaryRepository commentaryRepository)
        {
            this.commentaryRepository = commentaryRepository;
        }

        public async Task CommentaryCorrectionCancelled(string contentBlockId, DateTime assessedDateTime)
        {
            var commentary = await GetCommentary(contentBlockId, assessedDateTime);
            if (commentary == null)
            {
                return;
            }

            if (commentary.Version == Commentary.DefaultVersion)
            {
                await commentaryRepository.Delete(commentary.ContentBlockId, commentary.CommentaryId);
            }
            else
            {
                await commentaryRepository.SetPendingChangesToNullOnCommentary(contentBlockId, commentary);
            }
        }

        public async Task CommentaryCorrectionPublished(string contentBlockId, DateTime assessedDateTime)
        {
            var commentary = await GetCommentary(contentBlockId, assessedDateTime);

            if (commentary?.PendingChanges == null)
            {
                return;
            }

            var newCommentary = commentary.PendingChanges;
            newCommentary.PendingChanges = null;
            newCommentary.Id = commentary.Id;

            await commentaryRepository.SaveCommentary(newCommentary);
        }

        public Task<Commentary?> GetCommentary(string contentBlockId, DateTime assessedDateTime)
        {
            return commentaryRepository.GetCommentary(contentBlockId, assessedDateTime);
        }

        public async Task<Commentary?> GetCurrentCommentary(string contentBlockId, DateTime assessedDateTime)
        {
            var commentary = await commentaryRepository.GetCommentary(contentBlockId, assessedDateTime);
            return commentary?.PendingChangesOrOriginal();
        }

        public async Task<Commentary> SaveCorrectionCommentary(Commentary commentary)
        {
            var currentCommentary = await GetCommentary(commentary.ContentBlockId, commentary.AssessedDateTime);

            if (currentCommentary == null)
            {
                var defaultCorrectionCommentary = new Commentary
                {
                    Id = commentary.Id,
                    ContentBlockId = commentary.ContentBlockId,
                    AssessedDateTime = commentary.AssessedDateTime,
                    CommentaryId = commentary.CommentaryId,
                    Version = Commentary.DefaultVersion,
                    PendingChanges = commentary
                };

                await commentaryRepository.SaveCommentary(defaultCorrectionCommentary);
                return defaultCorrectionCommentary;
            }

            if (currentCommentary.PendingChanges == null)
            {
                currentCommentary.PendingChanges = commentary;
            }
            else
            {
                currentCommentary.PendingChanges.Version = commentary.Version;
            }

            await commentaryRepository.SaveCommentary(currentCommentary);

            return currentCommentary;
        }

        public async Task<Commentary> SaveNormalCommentary(Commentary commentary)
        {
            var currentCommentary = await GetCommentary(commentary.ContentBlockId, commentary.AssessedDateTime);

            if (currentCommentary == null)
            {
                return await commentaryRepository.SaveCommentary(commentary);
            }

            currentCommentary.Version = commentary.Version;

            return await commentaryRepository.SaveCommentary(currentCommentary);
        }
    }
}
