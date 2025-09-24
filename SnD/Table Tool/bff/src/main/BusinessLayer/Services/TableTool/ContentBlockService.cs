namespace BusinessLayer.Services.TableTool
{
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.DTO;
    using BusinessLayer.Services.Models;

    using global::Common.Constants;
    using global::Common.Helpers;

    using Infrastructure.SQLDB.Models;
    using Infrastructure.SQLDB.Repositories;

    using Serilog;

    public class ContentBlockService : IContentBlockService
    {
        IGenericRepository<TableConfiguration> genericRepository;
        private readonly ILogger logger;
        private readonly IQueryable<TableConfiguration> query;
        public ContentBlockService(IGenericRepository<TableConfiguration> genericRepository, ILogger logger)
        {
            this.genericRepository = genericRepository;
            this.logger = logger;
        }
        public async Task<Response<ContentBlockResponse>> GetContentBlockConfiguration(string contentBlockId, string version)
        {
            var (majorVersion, minorVersion) = Helpers.SplitVersionString(version);

            var query = genericRepository.GetQueryable();

            var record = query.Where(x => x.ContentBlockId == contentBlockId && x.MajorVersion == majorVersion && x.MinorVersion == minorVersion);

            if (record is null || record.Count() <= 0)
            {
                logger.Error(string.Format(ErrorMessages.SNDTableToolContentBlockVersionNotFound, contentBlockId, version));

                return Response<ContentBlockResponse>.Failure(string.Format(ErrorMessages.SNDTableToolContentBlockVersionNotFound, contentBlockId, version),
                    StatusCode.NotFound);
            }

            var contentBlockRecord = record.FirstOrDefault();

            var contentBlock = new ContentBlockResponse()
            {
                ContentBlockId = contentBlockId,
                Version = Helpers.FormatVersionString(contentBlockRecord.MajorVersion, contentBlockRecord.MinorVersion),
                Filter = contentBlockRecord.Filter
            };

            return Response<ContentBlockResponse>.Success(contentBlock);
        }

        public async Task<Response<SaveContentBlockResponse>> SaveContentBlockConfiguration(SaveContentBlockRequest contentBlockInput)
        {

            var (majorVersion, minorVersion) = GetLatestVersion(contentBlockInput.ContentBlockId);

            var contentBlock = new TableConfiguration()
            {
                MajorVersion = majorVersion,
                MinorVersion = ++minorVersion,
                ContentBlockId = contentBlockInput.ContentBlockId,
                Filter = contentBlockInput.Filter,
                CreatedOn = DateTime.Now,
            };

            var status = await genericRepository.InsertAsync(contentBlock);
            if (status <= 0)
            {
                logger.Error(string.Format(ErrorMessages.SNDTableToolContentBlockSaveFailed, contentBlock.ContentBlockId));

                return Response<SaveContentBlockResponse>.Failure(string.Format(ErrorMessages.SNDTableToolContentBlockSaveFailed, contentBlock.ContentBlockId),
                    StatusCode.InternalServerError);
            }

            var response = new SaveContentBlockResponse()
            {
                ContentBlockId = contentBlock.ContentBlockId,
                Version = Helpers.FormatVersionString(contentBlock.MajorVersion, contentBlock.MinorVersion)
            };

            return Response<SaveContentBlockResponse>.Success(response);

        }

        private (int, int) GetLatestVersion(string contentBlockId)
        {
            var query = genericRepository.GetQueryable();

            var latestVersion = query.Where(x => x.ContentBlockId == contentBlockId)
                                     .OrderByDescending(x => x.MajorVersion)
                                     .ThenByDescending(x => x.MinorVersion)
                                     .FirstOrDefault();

            return latestVersion != null ? (latestVersion.MajorVersion, latestVersion.MinorVersion) : (0, 0);
        }
    }
}
