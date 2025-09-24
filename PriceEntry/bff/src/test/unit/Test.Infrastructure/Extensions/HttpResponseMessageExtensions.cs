namespace Test.Infrastructure.Extensions
{
    using System.Net.Http;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class HttpResponseMessageExtensions
    {
        public static async Task<string> GetRawResponse(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var jsonDocument = JsonDocument.Parse(jsonResponse);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return JsonSerializer.Serialize(jsonDocument.RootElement, options);
        }

        public static async Task<string> GetRawResponse(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;
            return await response.GetRawResponse();
        }

        public static async Task<JsonDocument> GetResponseAsJsonDocument(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);

            return jsonDocument;
        }
    }
}
