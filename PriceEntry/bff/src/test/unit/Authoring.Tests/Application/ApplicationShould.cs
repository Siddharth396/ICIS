namespace Authoring.Tests.Application
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ApplicationShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "ContentBlockId";

        public ApplicationShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Support_Concurrent_Requests_And_Retry_In_Case_Of_Transient_Errors_In_Mongo()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var tasks = new List<Task<HttpResponseMessage>>(100);
            var now = TestData.Now.AddYears(-1);
            for (var i = 0; i < 100; i++)
            {
                var assessedDateTime = now.AddDays(i);

                tasks.Add(
                    new PriceEntryBusinessFlow(
                            ContentBlockId + i,
                            new List<string> { TestSeries.LNG_Dubai_MM1 },
                            HttpClient)
                       .SaveContentBlockDefinition()
                       .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Dubai_MM1, assessedDateTime, seriesItem)
                       .Execute());
            }

            await Task.WhenAll(tasks);

            foreach (var responseMessage in tasks)
            {
                var json = await responseMessage.GetResponseAsJsonDocument();
                var hasErrors = json.RootElement.TryGetProperty("errors", out _);
                hasErrors.Should().BeFalse();

                var hasData = json.RootElement.TryGetProperty("data", out _);
                hasData.Should().BeTrue();
            }
        }
    }
}
