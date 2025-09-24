namespace Test.Infrastructure.Authoring.ApiClients
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.DTOs.Input;

    using Test.Infrastructure.TestData;

    public class ReferencePriceApiClient
    {
        private static readonly long AssessedDateTimestamp = TestData.GasPricePublishedDateTime.ToUnixTimeMilliseconds();

        private static readonly long FulfilmentFromTimestamp = TestData.GasPriceFulfilmentFromDateTime.ToUnixTimeMilliseconds();

        private static readonly long FulfilmentUntilTimestamp = TestData.GasPriceFulfilmentUntilDateTime.ToUnixTimeMilliseconds();

        private readonly HttpClient httpClient;

        public ReferencePriceApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GasPricePublished()
        {
            var input = new GasPricePayload()
            {
                MarketCode = "TTF",
                AssessedDateTimestamp = AssessedDateTimestamp,
                FulfilmentFromTimestamp = FulfilmentFromTimestamp,
                FulfilmentUntilTimestamp = FulfilmentUntilTimestamp,
                Mid = 1989M,
            };

            return await httpClient.PostAsJsonAsync("/v1/referenceprice/gas/published", input);
        }
    }
}
