namespace Infrastructure.Services.Version
{
    using System.Reflection;

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