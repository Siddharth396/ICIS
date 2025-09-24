namespace Authoring.Api.Controllers
{
    using System.Reflection;

    using global::Infrastructure.Services.Version;

    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class VersionController
    {
        private readonly VersionService versionService;

        public VersionController(VersionService versionService)
        {
            this.versionService = versionService;
        }

        [HttpGet]
        public VersionInfo Get()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return versionService.Get(assembly);
        }
    }
}
