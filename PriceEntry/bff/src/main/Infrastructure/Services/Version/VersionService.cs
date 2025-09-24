namespace Infrastructure.Services.Version
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    [ExcludeFromCodeCoverage]
    public class VersionService
    {
        public VersionInfo Get(Assembly assembly)
        {
            return new VersionInfo
            {
                Version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
                Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product
            };
        }
    }
}