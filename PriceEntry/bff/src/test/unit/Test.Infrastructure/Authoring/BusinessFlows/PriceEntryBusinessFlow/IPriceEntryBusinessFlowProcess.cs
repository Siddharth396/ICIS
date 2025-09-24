namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IPriceEntryBusinessFlowProcess
    {
        Task<HttpResponseMessage> Execute(HttpClient httpClient);
    }
}
