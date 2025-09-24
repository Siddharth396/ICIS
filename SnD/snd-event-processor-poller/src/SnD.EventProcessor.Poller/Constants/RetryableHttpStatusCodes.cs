using System.Net;

namespace SnD.EventProcessor.Poller.Constants
{
    public class RetryableHttpStatusCodes
    {
        public static HttpStatusCode[] Codes =
        {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout, // 504
            HttpStatusCode.NotFound, // 404
        };

        public static HttpStatusCode[] KPCodes =
        {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout, // 504
        };
    }
}
