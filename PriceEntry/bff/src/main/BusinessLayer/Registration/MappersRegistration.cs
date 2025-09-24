namespace BusinessLayer.Registration
{
    using System.Linq;
    using System.Reflection;

    using BusinessLayer.Common.Mappers;

    using Microsoft.Extensions.DependencyInjection;

    public static class MappersRegistration
    {
        public static void RegisterMappers(this IServiceCollection service)
        {
            var types = Assembly
               .GetExecutingAssembly()
               .GetTypes()
               .Where(t => t is
                {
                    IsClass: true,
                    IsAbstract: false
                })
               .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
               .Where(
                    x => x.Interface.IsGenericType
                         && x.Interface.GetGenericTypeDefinition() == typeof(IModelMapper<,>));

            foreach (var type in types)
            {
                service.AddTransient(type.Interface, type.Implementation);
            }
        }
    }
}
