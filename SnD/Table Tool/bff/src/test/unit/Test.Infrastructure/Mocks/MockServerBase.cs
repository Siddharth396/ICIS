namespace Test.Infrastructure.Mocks
{
    using System;
    using System.Net;

    using Test.Infrastructure.Extensions;

    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using WireMock.Server;
    using WireMock.Settings;

    public abstract class MockServerBase : IDisposable
    {
        private readonly string basePath;

        private bool disposed;

        protected MockServerBase(int serverPort, string basePath)
        {
            this.basePath = basePath.TrimEnd('/');
            Server = WireMockServer.Start(new WireMockServerSettings { Port = serverPort });
        }

        protected WireMockServer Server { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void InternalServerError(string endpoint)
        {
            Server.Reset();
            Server.Given(Request.Create().WithPath(BuildPath(endpoint)).UsingGet())
               .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.InternalServerError));
        }

        public void OkAsJson(
            string endpoint,
            object response,
            params (string Name, string Value)[] parameters)
        {
            Server.Given(Request.Create().WithPath(BuildPath(endpoint)).WithParams(parameters).UsingGet())
               .RespondWith(
                    Response.Create()
                       .WithStatusCode(HttpStatusCode.OK)
                       .WithHeader("Content-Type", "application/json")
                       .WithBodyAsJson(response));
        }

        public void Reset(bool reset)
        {
            if (reset)
            {
                Server.Reset();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Server?.Stop();
                Server?.Dispose();
            }

            disposed = true;
        }

        private string BuildPath(string endpoint)
        {
            return $"{basePath}/{endpoint.TrimStart('/')}";
        }
    }
}