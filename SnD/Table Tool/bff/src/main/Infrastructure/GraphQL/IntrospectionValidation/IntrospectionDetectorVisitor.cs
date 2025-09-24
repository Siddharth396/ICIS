namespace Infrastructure.GraphQL.IntrospectionValidation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using HotChocolate;
    using HotChocolate.Language;

    [ExcludeFromCodeCoverage]
    internal class IntrospectionDetectorVisitor : QuerySyntaxWalker<DisableIntrospectionContext>
    {
        private static readonly List<string> IntrospectionTypes =
            new List<string>
            {
                "__Directive",
                "__DirectiveLocation",
                "__EnumValue",
                "__Field",
                "__InputValue",
                "__Schema",
                "__Type",
                "__TypeKind"
            };

        protected override void VisitName(NameNode node, DisableIntrospectionContext context)
        {
            //if (IntrospectionTypes.Any(x => x.Equals(node.Value, StringComparison.OrdinalIgnoreCase)))
            //{
            //    context.ReportError(
            //        new CustomError("INTROSPECTION_QUERY_NOT_ALLOWED", "Introspection query is not allowed")
            //           .WithPath(Path.New(new NameString(node.Value))));
            //}
        }
    }
}
