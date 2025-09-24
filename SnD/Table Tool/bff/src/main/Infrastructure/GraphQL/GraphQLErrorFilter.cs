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
            if (error is CustomError)
            {
                return error;
            }

            logger.Error(error.Exception, "Error while executing GraphQL request for {ExecutionPath}. {ErrorMessage}", error.Path, error.Message);

            return error.WithMessage("Exception occurred while executing the GraphQL request")
               .WithCode("EXCEPTION_THROWN");
        }
    }
}
