namespace Authoring.Application.ContentBlock.Helpers
{
    using System;

    using Authoring.Application.ContentBlock.Query;

    using BusinessLayer.ContentBlock.DTOs.Output;

    using global::Infrastructure.Services.Workflow;

    using HotChocolate.Resolvers;

    public static class ResolverContextHelper
    {
        public static DateTime GetAssessedDateTimeVariable(this IResolverContext resolverContext)
        {
            // By default, HotChocolate is parsing input parameters as DateTimeOffset, and it can convert it to DateTime in resolvers
            // but from the resolver context, we can get only as DateTimeOffset, and we need to convert it to DateTime explicitly
            return resolverContext.Variables
               .GetVariable<DateTimeOffset>(Constants.ContentBlock.AssessedDateTimeParameter)
               .UtcDateTime;
        }

        public static WorkflowStatus GetDataPackageWorkflowStatusFromScopedContext(this IResolverContext resolverContext)
        {
            return (resolverContext.ScopedContextData[Constants.ScopedContext.DataPackageStatus] as WorkflowStatus)!;
        }

        public static ContentBlockDefinition GetContentBlockDefinitionFromScopedContext(this IResolverContext resolverContext)
        {
            return (resolverContext.ScopedContextData[Constants.ScopedContext.ContentBlockDefinition] as ContentBlockDefinition)!;
        }

        public static bool GetIsReviewModeFromScopedContext(this IResolverContext resolverContext)
        {
            return resolverContext.ScopedContextData[Constants.ScopedContext.IsReviewMode] as bool? ?? false;
        }
    }
}
