namespace BusinessLayer.UserPreference.Mappers
{
    using BusinessLayer.Common.Mappers;
    using BusinessLayer.UserPreference.DTOs.Output;

    public class UserPreferencesColumnMapper : IModelMapper<Repositories.Models.Column, DTOs.Output.Column>
    {
        public Column Map(Repositories.Models.Column model)
        {
            return new Column
            {
                Field = model.Field,
                DisplayOrder = model.DisplayOrder,
                Hidden = model.Hidden,
            };
        }
    }
}
