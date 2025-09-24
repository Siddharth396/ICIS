namespace Infrastructure.GraphQL.IntrospectionValidation
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using HotChocolate.Language;
    using HotChocolate.Validation;

    [ExcludeFromCodeCoverage]
    internal class DisableIntrospectionValidationRule : IDocumentValidatorRule
    {
        private static readonly IntrospectionDetectorVisitor Visitor = new ();

        /// <summary>
        /// Defines if the result of this rule can be cached and reused on consecutive
        /// validations of the same GraphQL request document.
        /// </summary>
        public bool IsCacheable => true;

        public void Validate(IDocumentValidatorContext context, DocumentNode document)
        {
            var errors = context.Errors.ToList();
            Visitor.Visit(document, new DisableIntrospectionContext(errors.Add));
        }
    }
}
