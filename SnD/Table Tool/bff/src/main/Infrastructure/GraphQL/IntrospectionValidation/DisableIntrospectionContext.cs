namespace Infrastructure.GraphQL.IntrospectionValidation
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using HotChocolate;

    [ExcludeFromCodeCoverage]
    internal class DisableIntrospectionContext
    {
        public DisableIntrospectionContext(Action<IError> reportError)
        {
            ReportError = reportError;
        }

        public Action<IError> ReportError { get; }
    }
}
