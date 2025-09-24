namespace Authoring.Application.UserPreference.Mutation
{
    using System.Threading.Tasks;

    using BusinessLayer.UserPreference.DTOs.Input;
    using BusinessLayer.UserPreference.DTOs.Output;

    using BusinessLayer.UserPreference.Services;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Language;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Mutation)]
    public class UserPreferenceMutations
    {
        [GraphQLName("saveUserPreference")]
        public async Task<UserPreferenceSaveResponse> SaveUserPreference(
                       [GraphQLNonNullType] UserPreferenceInput userPreferenceInput,
                       [Service] IUserPreferenceService userPreferenceService,
                       [Service] ILogger logger)
        {
            using (LogContext.PushProperty("Flow", nameof(SaveUserPreference)))
            {
                var localLogger = logger.ForContext<UserPreferenceMutations>();

                localLogger.Debug("START : Saving user preference");

                var response = await userPreferenceService.SaveUserPreference(userPreferenceInput);

                localLogger.Debug("END : Saving user preference");

                return response;
            }
        }
    }
}
