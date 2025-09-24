namespace Architecture.Tests.NetArchTestsCustomization
{
    using System.Linq;

    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    using NetArchTest.Rules;

    public sealed class DefaultConstructorsOnly : ICustomRule
    {
        public static readonly DefaultConstructorsOnly Rule = new DefaultConstructorsOnly();

        private DefaultConstructorsOnly()
        {
        }

        public bool MeetsRule(TypeDefinition type)
        {
            var constructors = type.GetConstructors().ToList();

            if (constructors.Count > 1)
            {
                return false;
            }

            return !constructors.Single().HasParameters;
        }
    }
}
