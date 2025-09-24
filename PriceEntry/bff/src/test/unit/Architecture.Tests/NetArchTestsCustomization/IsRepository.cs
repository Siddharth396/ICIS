namespace Architecture.Tests.NetArchTestsCustomization
{
    using Infrastructure.MongoDB.Repositories;

    using Mono.Cecil;

    using NetArchTest.Rules;

    public sealed class IsRepository : ICustomRule
    {
        public static readonly IsRepository Rule = new();

        private IsRepository()
        {
        }

        public bool MeetsRule(TypeDefinition type)
        {
            // Check if the type name ends with "Repository"
            if (type.Name.EndsWith("Repository"))
            {
                return true;
            }

            // Check if the type extends BaseRepository
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.FullName == typeof(BaseRepository<>).FullName)
                {
                    return true;
                }

                baseType = baseType.Resolve().BaseType;
            }

            return false;
        }
    }
}
