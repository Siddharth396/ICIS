namespace Test.Infrastructure.Mocks
{
    public class DbApiMockServer : MockServerBase
    {
        private const string BaseUrl = "/v1";

        private const int ServerPort = 9599;

        public DbApiMockServer()
            : base(ServerPort, BaseUrl)
        {
            Series = new SeriesMock(this);
        }

        public SeriesMock Series { get; }
    }
}