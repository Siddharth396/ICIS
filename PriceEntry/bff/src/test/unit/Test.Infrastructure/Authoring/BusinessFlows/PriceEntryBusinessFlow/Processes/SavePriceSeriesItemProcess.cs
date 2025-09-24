namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;

    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    public class SavePriceSeriesItemProcess : IPriceEntryBusinessFlowProcess
    {
        private readonly PriceItemInput priceItemInput;

        public SavePriceSeriesItemProcess(PriceItemInput priceItemInput)
        {
            this.priceItemInput = priceItemInput;
        }

        public async Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var priceEntryClient = new PriceEntryGraphQLClient(new GraphQLClient(httpClient));
            return await priceEntryClient.SavePriceSeriesItem(priceItemInput);
        }
    }
}
