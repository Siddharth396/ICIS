namespace BusinessLayer.UserPreference.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.UserPreference.DTOs.Input;
    using BusinessLayer.UserPreference.DTOs.Output;

    public interface IUserPreferenceService
    {
        Task<UserPreferenceSaveResponse> SaveUserPreference(UserPreferenceInput userPreferenceInput);

        Task<DTOs.Output.UserPreference?> GetUserPreference(string contentBlockId);
    }
}
