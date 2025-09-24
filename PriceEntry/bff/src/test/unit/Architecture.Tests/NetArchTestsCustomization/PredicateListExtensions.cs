namespace Architecture.Tests.NetArchTestsCustomization
{
    using System.Linq;

    using NetArchTest.Rules;

    public static class PredicateListExtensions
    {
        public static string?[] GetTypesFullNames(this PredicateList predicateList)
        {
            return predicateList.GetTypes().Select(x => x.FullName).ToArray();
        }
    }
}
