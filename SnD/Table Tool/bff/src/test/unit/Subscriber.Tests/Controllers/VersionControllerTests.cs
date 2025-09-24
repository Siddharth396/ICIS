namespace Subscriber.Tests.Controllers
{
    using global::Infrastructure.Services.Version;
    using Subscriber.Controllers;

    using Xunit;

    [Collection("Subscriber stub collection")]
    public class VersionControllerTests
    {
        private readonly VersionService service;
        private readonly VersionController controller;

        public VersionControllerTests(SubscriberStubFixture fixture)
        {
            service = fixture.GetService<VersionService>();
            controller = new VersionController(service);
        }

        [Fact]
        public void Get_VersionSet_VersionInfoReturned()
        {
            // Act
            var versionInfo = controller.Get();

            // Assert
            Assert.Contains("1.0.0-local-test+0", versionInfo.Version);
            Assert.Equal("Subscriber-1.0.0-pre-test.0", versionInfo.Product);
        }
    }
}
