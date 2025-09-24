namespace Authoring.Tests
{
    using Test.Infrastructure.Stubs;

    public abstract class TestBase
    {
        protected TestBase(AuthoringStubFixture fixture)
        {
            AppFixture = fixture;
        }

        protected AuthoringStubFixture AppFixture { get; }

    }
}
