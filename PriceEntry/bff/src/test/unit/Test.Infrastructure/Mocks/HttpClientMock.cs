namespace Test.Infrastructure.Mocks
{
    using System;
    using System.Net.Http;

    public static class HttpClientMock
    {
        public static HttpClient GetHttpClientWithResponse(string baseUrl, HttpResponseMessage responseMessage)
        {
            var handlerMock = new HttpMessageHandlerMock();
            handlerMock.SetResponseMessage(responseMessage);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri(baseUrl) };
            return httpClient;
        }
    }
}