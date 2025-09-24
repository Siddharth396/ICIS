namespace Test.Infrastructure.Mocks
{
    public class CoreServiceMockServer : MockServerBase
    {
        private const string BaseUrl = "/v1";

        private const int ServerPort = 9590;

        public CoreServiceMockServer()
            : base(ServerPort, BaseUrl)
        {
            UserLanguage = new UserLanguageMock(this);
        }

        public UserLanguageMock UserLanguage { get; }
    }
}