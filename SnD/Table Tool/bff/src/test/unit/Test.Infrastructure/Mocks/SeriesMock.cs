namespace Test.Infrastructure.Mocks
{
    public class SeriesMock
    {
        private const string SeriesDomainsEndpoint = "/series/domains";

        private readonly DbApiMockServer mockServer;

        public SeriesMock(DbApiMockServer mockServer)
        {
            this.mockServer = mockServer;
        }

        public void Ok(object response, bool reset = true)
        {
            mockServer.Reset(reset);
            mockServer.OkAsJson(SeriesDomainsEndpoint, response);
        }

        public void Error()
        {
            mockServer.InternalServerError(SeriesDomainsEndpoint);
        }
    }
}