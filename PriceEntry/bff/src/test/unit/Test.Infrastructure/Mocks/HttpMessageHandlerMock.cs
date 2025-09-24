namespace Test.Infrastructure.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpMessageHandlerMock : HttpMessageHandler
    {
        private readonly List<HttpRequestMessage> requests = new();

        private HttpResponseMessage? responseMessage;

        public IReadOnlyList<HttpRequestMessage> Requests => requests;

        public void SetResponseMessage(HttpResponseMessage httpResponseMessage)
        {
            responseMessage = httpResponseMessage;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            requests.Add(request);

            if (responseMessage == null)
            {
                throw new InvalidOperationException("Response message is not set");
            }

            return Task.FromResult(responseMessage);
        }
    }
}
