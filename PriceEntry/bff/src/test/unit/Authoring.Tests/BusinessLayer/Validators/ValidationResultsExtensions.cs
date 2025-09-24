namespace Authoring.Tests.BusinessLayer.Validators
{
    using System;
    using System.Linq.Expressions;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Validators;

    public static class ValidationResultsExtensions
    {
        public static void ShouldHaveValidationErrorFor(
            this ValidationResult result,
            string propertyName,
            string errorMessage)
        {
            result.ValidationErrors.Should().ContainKey(propertyName).WhoseValue.Should().Contain(errorMessage);
        }

        public static void ShouldHaveValidationErrorFor<T>(
            this ValidationResult result,
            Expression<Func<T, object?>> propertyExpression,
            string errorMessage)
        {
            var propertyName = GetPropertyName(propertyExpression);
            propertyName = ToCamelCase(propertyName);
            result.ValidationErrors.Should().ContainKey(propertyName).WhoseValue.Should().Contain(errorMessage);
        }

        private static string GetPropertyName<T>(Expression<Func<T, object?>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            if (expression.Body is UnaryExpression { Operand: MemberExpression } unaryExpression)
            {
                return ((MemberExpression)unaryExpression.Operand).Member.Name;
            }

            throw new ArgumentException("Invalid expression");
        }

        private static string ToCamelCase(string propertyName)
        {
            propertyName = char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
            return propertyName;
        }
    }
}
