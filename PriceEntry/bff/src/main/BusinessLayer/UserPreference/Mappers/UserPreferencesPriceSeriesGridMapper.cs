namespace BusinessLayer.UserPreference.Mappers
{
    using System.Linq;

    using BusinessLayer.Common.Mappers;
    using BusinessLayer.UserPreference.DTOs.Output;

    public class UserPreferencesPriceSeriesGridMapper : IModelMapper<Repositories.Models.PriceSeriesGrid, DTOs.Output.PriceSeriesGrid>
    {
        private readonly IModelMapper<Repositories.Models.Column, DTOs.Output.Column> userPreferencesColumnMapper;

        public UserPreferencesPriceSeriesGridMapper(
            IModelMapper<Repositories.Models.Column, DTOs.Output.Column> userPreferencesColumnMapper)
        {
            this.userPreferencesColumnMapper = userPreferencesColumnMapper;
        }

        public DTOs.Output.PriceSeriesGrid Map(Repositories.Models.PriceSeriesGrid model)
        {
            return new PriceSeriesGrid
            {
                Id = model.PriceSeriesGridId,
                Columns = model.Columns.Select(userPreferencesColumnMapper.Map).ToList(),
                PriceSeriesIds = model.PriceSeriesIds,
            };
        }
    }
}
