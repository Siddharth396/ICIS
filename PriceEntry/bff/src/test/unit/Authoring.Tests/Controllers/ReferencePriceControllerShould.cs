namespace Authoring.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.Repositories;
    using global::BusinessLayer.PriceEntry.Repositories.Models;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::Infrastructure.Services.PeriodGenerator;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.ApiClients;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Mongo.Repositories;
    using Test.Infrastructure.Services;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ReferencePriceControllerShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private const string PriceSeriesId = TestSeries.LNG_Dubai_MM1;

        private static readonly DateTime AssessedDateTime = TestData.GasPricePublishedDateTime;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        public ReferencePriceControllerShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            var testClock = GetService<TestClock>();
            testClock.SetUtcNow(AssessedDateTime);
        }

        [Fact]
        public async Task Update_Reference_Price_When_Gas_Price_Is_Published()
        {
            // Arrange
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(
            [
                new AbsolutePeriod
                {
                    Code = "MM1709",
                    PeriodCode = "MM1",
                    Label = "September '17",
                    FromDate = DateOnly.FromDateTime(TestData.GasPriceFulfilmentFromDateTime),
                    UntilDate = DateOnly.FromDateTime(TestData.GasPriceFulfilmentUntilDateTime)
                }

            ]);

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/Offer"
            };

            var gasPriceSeriesItem = new GasPriceSeriesItem
            {
                Id = "267-M1-20170814",
                SpecificationName = "ICIS Heren® Natural Gas EoD Assessment - TTF Conv. USD/MMBtu",
                MarketCode = "TTF",
                AssessedDateTime = TestData.GasPricePublishedDateTime,
                Status = "Published",
                PeriodLabel = "September '17",
                FulfilmentFromDate = TestData.GasPriceFulfilmentFromDateTime,
                FulfilmentUntilDate = TestData.GasPriceFulfilmentUntilDateTime,
                Mid = 1989M,
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, AssessedDateTime, seriesItem);

            await businessFlow.Execute();

            var gasPriceSeriesItemTestService = GetService<GasPriceSeriesItemTestService>();

            await gasPriceSeriesItemTestService.SaveGasPrice(gasPriceSeriesItem);

            var referencePriceApiClient = new ReferencePriceApiClient(HttpClient);

            // Act
            await referencePriceApiClient.GasPricePublished();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            periodGeneratorServiceStub.ClearAbsolutePeriods();

            // Assert
            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Not_Throw_Exception_When_It_Can_Not_Deserialize_Unknown_SeriesItemTypeCode()
        {
            const string SeriesItemId = "f651b76f-d147-4dcd-9978-354d70daff89";
            var repository = GetService<GenericRepository>();
            var document = $$"""
                                {
                                    "_id": "{{SeriesItemId}}",
                                    "assessed_datetime": {
                                     	"$date": "{{TestData.GasPricePublishedDateTime:yyyy-MM-ddTHH:mm:ssZ}}"
                                    },
                                    "created": {
                                     	"timestamp": {
                                     		"$date": "2025-03-26T11:23:40Z"
                                     	},
                                     	"user": ""
                                    },
                                    "dlh_record_source": "lng-arbitrage-uat",
                                    "last_modified": {
                                     	"timestamp": {
                                     		"$date": "2025-03-26T11:23:40Z"
                                     	},
                                     	"user": ""
                                    },
                                    "price_delta": 0.00000,
                                    "price_delta_type_guid": "fe446f10-4901-4661-a281-44d2f9ba26e4",
                                    "price_value": 1.16180,
                                    "release_datetime": {
                                     	"$date": "2024-12-31T00:00:00Z"
                                    },
                                    "series_id": "0093b487-9589-4fed-8c98-592bcd45359b",
                                    "series_item_type_code": "unknown-code",
                                    "status": "PUBLISHED"
                               }
                           """;
            await repository.InsertRawDocument(PriceSeriesItemRepository.CollectionName, document);

            var referencePriceApiClient = new ReferencePriceApiClient(HttpClient);

            // Act
            var response = await referencePriceApiClient.GasPricePublished();

            await repository.DeleteDocumentById(PriceSeriesItemRepository.CollectionName, SeriesItemId);

            Action assert = () => response.EnsureSuccessStatusCode();
            assert.Should().NotThrow<HttpRequestException>("Unknown series item type codes should be excluded");
        }
    }
}
