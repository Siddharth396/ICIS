namespace Architecture.Tests.BusinessLayerRules
{
    using System.Linq;

    using Architecture.Tests.NetArchTestsCustomization;

    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.PriceEntry.Services;

    using Infrastructure.Attributes.BusinessAnnotations;

    using NetArchTest.Rules;

    using Xunit;

    public sealed class ApplicationServicesShould
    {
        [Fact]
        public void Not_Depend_On_Repositories()
        {
            var repositoryTypes = Types
               .InAssembly(SolutionAssemblies.BusinessLayer)
               .That()
               .MeetCustomRule(IsRepository.Rule)
               .GetTypesFullNames();

            Types
               .InAssembly(SolutionAssemblies.BusinessLayer)
               .That()
               .HaveCustomAttribute(typeof(ApplicationServiceAttribute))
               .And()
                //// The following are exceptions for now and should be refactored.
               .DoNotHaveName(nameof(PriceEntryService))
               .Should()
               .NotHaveDependencyOnAny(repositoryTypes)
               .GetResult()
               .ShouldBeSuccessful();
        }

        [Fact]
        public void Not_Depend_On_Other_Application_Services()
        {
            var applicationServices = Types
               .InAssembly(SolutionAssemblies.BusinessLayer)
               .That()
               .HaveCustomAttribute(typeof(ApplicationServiceAttribute))
               .And()
               //// The following are exceptions for now and should be refactored.
               .DoNotHaveName(nameof(DataPackageService))
               .GetTypes()
               .ToList();

            var servicesWithKnownTypes = applicationServices.ToDictionary(
                service => service,
                service => service.GetInterfaces().Append(service).Select(i => i.FullName).ToArray());

            foreach (var applicationService in applicationServices)
            {
                var otherTypesExceptItself = servicesWithKnownTypes
                    .Where(pair => pair.Key != applicationService)
                    .SelectMany(x => x.Value)
                    .ToArray();

                Types
                   .InAssembly(SolutionAssemblies.BusinessLayer)
                   .That()
                   .HaveName(applicationService.Name)
                   .Should()
                   .NotHaveDependencyOnAny(otherTypesExceptItself)
                   .GetResult()
                   .ShouldBeSuccessful();
            }
        }
    }
}
