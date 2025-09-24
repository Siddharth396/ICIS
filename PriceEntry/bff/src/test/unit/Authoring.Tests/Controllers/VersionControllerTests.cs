namespace Authoring.Tests.Controllers
{
    using global::Authoring.Api.Controllers;
    using global::Infrastructure.Services.Version;

    using Xunit;

    public class VersionControllerTests
    {
        private readonly VersionController controller;

        public VersionControllerTests()
        {
            controller = new VersionController(new VersionService());
        }

        [Fact]
        public void Get_VersionSet_VersionInfoReturned()
        {
            // Act
            var versionInfo = controller.Get();

            // Assert
            Assert.Equal("1.0.0-local-test+0", versionInfo.Version);
            Assert.Equal("Authoring-1.0.0-pre-test.0", versionInfo.Product);
        }
    }
}
