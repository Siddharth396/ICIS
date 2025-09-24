namespace Authoring.Application.ContentBlock.Mutation
{
    using global::Infrastructure.GraphQL;
    using HotChocolate.Resolvers;
    using HotChocolate;
    using System.Threading.Tasks;
    using HotChocolate.Types;
    using BusinessLayer.Services.TableTool;
    using BusinessLayer.DTO;
    using BusinessLayer.Validation;

    [AddToGraphQLSchema]
    [ExtendObjectType("Mutation")]

    public class ContentBlockMutations
    {
        private readonly IErrorReporter errorReporter;

        public ContentBlockMutations(IErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
        }

        public async Task<SaveContentBlockResponse?> SaveContentBlock(IResolverContext resolverContext, [GraphQLNonNullType] SaveContentBlockRequest contentBlockRequest, [Service] IContentBlockService service, [Service] IContentBlockValidationService contentBlockValidationService)
        {
            var validationResult = contentBlockValidationService.ValidateContentBlockInput(contentBlockRequest);

            if (!validationResult.Status)
            {
                errorReporter.ReportCustomError(
                resolverContext,
                validationResult.StatusCode,
                validationResult.ValidationMessage);
                return null;
            }

            var response = await service.SaveContentBlockConfiguration(contentBlockRequest);
            if (response.IsFailure)
            {
                errorReporter.ReportCustomError(
                resolverContext,
                response.Error.Code,
                response.Error.Message);
            }

            return response.Data;
        }
    }
}
