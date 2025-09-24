namespace Infrastructure.Logging
{
    using System.Diagnostics.CodeAnalysis;

    using Elastic.Apm;

    using HotChocolate.Execution;
    using HotChocolate.Execution.Instrumentation;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DiagnosticAdapter;

    [ExcludeFromCodeCoverage]
    public class GraphQLLogger : ExecutionDiagnosticEventListener
    {
        [DiagnosticName("HotChocolate.Execution.Query.Start")]
        public override void AddedOperationToCache(IRequestContext context)
        {
            LogOperationName(context);

            base.AddedOperationToCache(context);
        }

        [DiagnosticName("HotChocolate.Execution.Query.Start")]
        public override void RetrievedOperationFromCache(IRequestContext context)
        {
            LogOperationName(context);

            base.AddedOperationToCache(context);
        }

        private static void LogOperationName(IRequestContext context)
        {
            var transaction = Agent.Tracer.CurrentTransaction;
            var httpContext = (HttpContext?)context.ContextData["HttpContext"];
            if (transaction != null && httpContext != null)
            {
                transaction.Name = $"{httpContext.Request.Method.ToUpper()} {httpContext.Request.Path.ToString().ToLower()} {context.Operation?.Name}";
            }
        }
    }
}