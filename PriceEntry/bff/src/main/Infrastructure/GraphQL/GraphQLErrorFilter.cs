namespace Infrastructure.GraphQL
{
    using System.Diagnostics.CodeAnalysis;

    using HotChocolate;

    using Serilog;

    /// <summary>
    /// https://josiahmortenson.dev/blog/2020-06-05-hotchocolate-graphql-errors
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GraphQLErrorFilter : IErrorFilter
    {
        private readonly ILogger logger;

        public GraphQLErrorFilter()
        {
            logger = Log.ForContext<GraphQLErrorFilter>();
        }

        public IError OnError(IError error)
        {
            LogGraphQLError(error);

            switch (error)
            {
                case CustomError _:
                    return error;
                case HotChocolate.Error _:
                    return CreateCustomError(error.Code, error.Message);
                default:
                    return CreateCustomError("EXCEPTION_THROWN", "Exception occurred while executing the GraphQL request");
            }
        }

        private IError CreateCustomError(string? code, string message)
        {
            return new CustomError(code, message);
        }

        private void LogGraphQLError(IError error)
        {
            logger.Error(error.Exception, "Error while executing GraphQL request for {ExecutionPath}. {ErrorMessage}", error.Path, error.Exception?.Message);
        }
    }
}
