namespace BusinessLayer.UserPreference.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Common.Mappers;
    using BusinessLayer.UserPreference.DTOs.Input;
    using BusinessLayer.UserPreference.DTOs.Output;
    using BusinessLayer.UserPreference.Repositories;

    using Serilog;

    using Subscriber.Auth;

    using Column = BusinessLayer.UserPreference.Repositories.Models.Column;
    using PriceSeriesGrid = BusinessLayer.UserPreference.Repositories.Models.PriceSeriesGrid;

    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly UserPreferenceRepository userPreferenceRepository;
        private readonly IUserContext userContext;
        private readonly ILogger logger;
        private readonly IModelMapper<Repositories.Models.UserPreference?, DTOs.Output.UserPreference?> userPreferenceMapper;

        public UserPreferenceService(
            UserPreferenceRepository userPreferenceRepository,
            IUserContext userContext,
            ILogger logger,
            IModelMapper<Repositories.Models.UserPreference?, DTOs.Output.UserPreference?> userPreferenceMapper)
        {
            this.userPreferenceRepository = userPreferenceRepository;
            this.userContext = userContext;
            this.logger = logger.ForContext<UserPreferenceService>();
            this.userPreferenceMapper = userPreferenceMapper;
        }

        public async Task<DTOs.Output.UserPreference?> GetUserPreference(string contentBlockId)
        {
            var userPreference = await userPreferenceRepository.GetUserPreference(
                                     userContext.UserIdEncrypted,
                                     contentBlockId);

            return userPreferenceMapper.Map(userPreference);
        }

        public async Task<UserPreferenceSaveResponse> SaveUserPreference(UserPreferenceInput userPreferenceInput)
        {
            var localLogger = logger.ForContext("ContentBlockId", userPreferenceInput.ContentBlockId);

            var userPreference = await userPreferenceRepository.GetUserPreference(userContext.UserIdEncrypted, userPreferenceInput.ContentBlockId);

            var preferenceForPriceSeriesGrid = MapUserPreferenceForPriceSeriesGrid(userPreferenceInput);

            if (userPreference is null)
            {
                localLogger.ForContext("Scenario", "AddUserPreference")
                            .Debug($"Adding user preference for contentBlockId {userPreferenceInput.ContentBlockId}");

                userPreference = new Repositories.Models.UserPreference
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentBlockId = userPreferenceInput.ContentBlockId,
                    UserId = userContext.UserIdEncrypted,
                    PriceSeriesGrids = new List<PriceSeriesGrid> { preferenceForPriceSeriesGrid },
                };
            }
            else
            {
                localLogger.ForContext("Scenario", "UpdateUserPreference")
                            .Debug($"Updating user preference for contentBlockId {userPreferenceInput.ContentBlockId}");

                var priceSeriesGrid = userPreference.PriceSeriesGrids.SingleOrDefault(x => x.PriceSeriesGridId == userPreferenceInput.PriceSeriesGridId);

                if (priceSeriesGrid == null)
                {
                    userPreference.PriceSeriesGrids.Add(preferenceForPriceSeriesGrid);
                }
                else
                {
                    priceSeriesGrid.PriceSeriesIds = preferenceForPriceSeriesGrid.PriceSeriesIds;
                    priceSeriesGrid.Columns = preferenceForPriceSeriesGrid.Columns;
                }
            }

            await userPreferenceRepository.SaveUserPreference(userPreference);

            return new UserPreferenceSaveResponse
            {
                Id = userPreference.Id
            };
        }

        private static PriceSeriesGrid MapUserPreferenceForPriceSeriesGrid(UserPreferenceInput userPreferenceInput)
        {
            var userPreference = new PriceSeriesGrid
            {
                PriceSeriesGridId = userPreferenceInput.PriceSeriesGridId,
                PriceSeriesIds = userPreferenceInput.PriceSeriesInput,
                Columns = userPreferenceInput.ColumnInput.Select(x => new Column()
                {
                    Field = x.Field,
                    DisplayOrder = x.DisplayOrder,
                    Hidden = x.Hidden
                }).ToList(),
            };

            return userPreference;
        }
    }
}
