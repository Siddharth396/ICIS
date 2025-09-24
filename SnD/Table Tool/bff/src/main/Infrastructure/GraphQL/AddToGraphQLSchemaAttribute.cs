namespace Infrastructure.GraphQL
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AddToGraphQLSchemaAttribute : Attribute
    {
    }
}
