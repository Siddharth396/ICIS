namespace Architecture.Tests.BusinessLayerRules
{
    using System.Linq;

    using Architecture.Tests.NetArchTestsCustomization;

    using Infrastructure.Attributes.BusinessAnnotations;

    using NetArchTest.Rules;

    using Xunit;

    public sealed class DomainServicesShould
    {
        [Fact]
        public void Not_Depend_On_Application_Services()
        {
            var applicationServices = Types
               .InAssembly(SolutionAssemblies.BusinessLayer)
               .That()
               .HaveCustomAttribute(typeof(ApplicationServiceAttribute))
               .GetTypes()
               .ToList();

            var servicesWithKnownTypes = applicationServices
               .SelectMany(service => service.GetInterfaces().Append(service).Select(i => i.FullName))
               .ToArray();

            Types
               .InAssembly(SolutionAssemblies.BusinessLayer)
               .That()
               .HaveCustomAttribute(typeof(DomainServiceAttribute))
               .Should()
               .NotHaveDependencyOnAny(servicesWithKnownTypes)
               .GetResult()
               .ShouldBeSuccessful();
        }
    }
}
