namespace Authoring.Application.ContentBlock.DataLoaders
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using BusinessLayer.UserPreference.DTOs.Output;
    using BusinessLayer.UserPreference.Services;

    using GreenDonut;

    using Serilog;

    public class UserPreferencesBatchLoader : BatchDataLoader<string, UserPreference?>
    {
        private readonly IUserPreferenceService userPreferenceService;

        private readonly ILogger logger;

        public UserPreferencesBatchLoader(
            IUserPreferenceService userPreferenceService,
            ILogger logger,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            this.userPreferenceService = userPreferenceService;
            this.logger = logger.ForContext<UserPreferencesBatchLoader>();
        }

        protected override async Task<IReadOnlyDictionary<string, UserPreference?>> LoadBatchAsync(
            IReadOnlyList<string> contentBlockIds, CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, UserPreference?>();

            foreach (var contentBlockId in contentBlockIds)
            {
                var userPreference = await userPreferenceService.GetUserPreference(contentBlockId);
                result.Add(contentBlockId, userPreference);
            }

            return result;
        }
    }
}
