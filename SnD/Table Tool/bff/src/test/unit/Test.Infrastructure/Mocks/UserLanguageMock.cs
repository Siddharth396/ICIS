namespace Test.Infrastructure.Mocks
{
    public class UserLanguageMock
    {
        private const string UserLanguageEndpoint = "/user/language";

        private readonly CoreServiceMockServer mockServer;

        public UserLanguageMock(CoreServiceMockServer mockServer)
        {
            this.mockServer = mockServer;
        }

        public void Ok(object response, bool reset = true)
        {
            mockServer.Reset(reset);
            mockServer.OkAsJson(UserLanguageEndpoint, response);
        }

        public void Error()
        {
            mockServer.InternalServerError(UserLanguageEndpoint);
        }
    }
}