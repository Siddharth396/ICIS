namespace BusinessLayer.UserPreference.Mappers
{
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.Common.Mappers;
    using BusinessLayer.UserPreference.DTOs.Output;

    public class UserPreferencesMapper : IModelMapper<Repositories.Models.UserPreference?, DTOs.Output.UserPreference?>
    {
        private readonly IModelMapper<Repositories.Models.Column, DTOs.Output.Column> userPreferencesColumnMapper;

        private readonly IModelMapper<Repositories.Models.PriceSeriesGrid, DTOs.Output.PriceSeriesGrid>
            userPreferencesPriceSeriesGridMapper;

        public UserPreferencesMapper(
            IModelMapper<Repositories.Models.Column, DTOs.Output.Column> userPreferencesColumnMapper,
            IModelMapper<Repositories.Models.PriceSeriesGrid, DTOs.Output.PriceSeriesGrid>
                userPreferencesPriceSeriesGridMapper)
        {
            this.userPreferencesColumnMapper = userPreferencesColumnMapper;
            this.userPreferencesPriceSeriesGridMapper = userPreferencesPriceSeriesGridMapper;
        }

        public DTOs.Output.UserPreference? Map(Repositories.Models.UserPreference? model)
        {
            if (model == null)
            {
                return null;
            }

            return new UserPreference
            {
                ContentBlockId = model.ContentBlockId,
                UserId = model.UserId,
                PriceSeriesGrids = GetPriceSeriesGrids(model)
            };
        }

        private List<PriceSeriesGrid> GetPriceSeriesGrids(Repositories.Models.UserPreference model)
        {
            return model.PriceSeriesGrids.Select(userPreferencesPriceSeriesGridMapper.Map).ToList();
        }
    }
}
