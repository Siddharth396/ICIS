namespace Authoring.Application.ContentBlock.Query
{
    using System.Threading.Tasks;

    using BusinessLayer.DTO;
    using BusinessLayer.DTOs;
    using BusinessLayer.Services.TableTool;
    using BusinessLayer.Validation;

    using global::Infrastructure.GraphQL;

    using HotChocolate;
    using HotChocolate.Resolvers;
    using HotChocolate.Types;

    [AddToGraphQLSchema]
    [ExtendObjectType("Query")]
    public class ContentBlockQueries
    {
        private readonly IErrorReporter errorReporter;

        public ContentBlockQueries(IErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
        }

        public async Task<ContentBlockResponse?> GetContentBlock(
            IResolverContext resolverContext,
            [GraphQLNonNullType] ContentBlockRequest contentBlockRequest,
            [Service] IContentBlockService service,
            [Service] IContentBlockValidationService contentBlockValidationService)
        {
            var validationResult = contentBlockValidationService.ValidateContentBlockRequest(contentBlockRequest);
            if (!validationResult.Status)
            {
                errorReporter.ReportCustomError(
                 resolverContext,
                 validationResult.StatusCode,
                 validationResult.ValidationMessage);
                return null;
            }

            var response = await service.GetContentBlockConfiguration(contentBlockRequest.ContentBlockId, contentBlockRequest.Version);
            if (response.IsFailure)
            {
                errorReporter.ReportCustomError(
                resolverContext,
                response.Error.Code,
                response.Error.Message);
            }

            return response.Data!;
        }
    }
}
