namespace Test.Infrastructure.Mocks
{
    using Xunit;

    [CollectionDefinition("AllServices")]
    public class AllServicesCollection : ICollectionFixture<CoreServiceMockServer>,
                                         ICollectionFixture<DbApiMockServer>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}