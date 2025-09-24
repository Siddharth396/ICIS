namespace Subscriber.Tests
{
    using Test.Infrastructure.Stubs;

    public abstract class TestBase
    {
        protected TestBase(SubscriberStubFixture fixture)
        {
            AppFixture = fixture;
        }

        protected SubscriberStubFixture AppFixture { get; }


    }
}
