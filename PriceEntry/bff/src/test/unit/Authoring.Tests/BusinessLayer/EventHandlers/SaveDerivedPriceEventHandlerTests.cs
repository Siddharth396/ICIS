namespace Authoring.Tests.BusinessLayer.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using Snapshooter;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class SaveDerivedPriceEventHandlerTests : WebApplicationTestBase
    {
        private static readonly List<string> GetHalfMonthInputSeriesIds =
        [
            TestSeries.LNG_China_HM1,
            TestSeries.LNG_China_HM2,
            TestSeries.LNG_China_HM3,
            TestSeries.LNG_China_HM4,
            TestSeries.LNG_China_HM5,

            TestSeries.LNG_Japan_HM1,
            TestSeries.LNG_Japan_HM2,
            TestSeries.LNG_Japan_HM3,
            TestSeries.LNG_Japan_HM4,
            TestSeries.LNG_Japan_HM5,

            TestSeries.LNG_Taiwan_HM1,
            TestSeries.LNG_Taiwan_HM2,
            TestSeries.LNG_Taiwan_HM3,
            TestSeries.LNG_Taiwan_HM4,
            TestSeries.LNG_Taiwan_HM5,

            TestSeries.LNG_South_Korea_HM1,
            TestSeries.LNG_South_Korea_HM2,
            TestSeries.LNG_South_Korea_HM3,
            TestSeries.LNG_South_Korea_HM4,
            TestSeries.LNG_South_Korea_HM5
        ];

        private static readonly List<string> GetFullMonthInputSeriesIds =
        [
            TestSeries.LNG_China_MM1,
            TestSeries.LNG_China_MM2,
            TestSeries.LNG_Japan_MM1,
            TestSeries.LNG_Japan_MM2,
            TestSeries.LNG_Taiwan_MM1,
            TestSeries.LNG_Taiwan_MM2,
            TestSeries.LNG_South_Korea_MM1,
            TestSeries.LNG_South_Korea_MM2
        ];

        private static readonly List<string> GetEaxHalfMonthInputFortnightIds =
        [
            TestSeries.LNG_EAX_Index_HM2,
            TestSeries.LNG_EAX_Index_HM3,
            TestSeries.LNG_EAX_Index_HM4,
            TestSeries.LNG_EAX_Index_HM5,
            TestSeries.LNG_EAX_Index_HM6
        ];

        private static readonly List<string> GetEaxFullMonthInputIds =
        [
            TestSeries.LNG_EAX_Index_MM1,
            TestSeries.LNG_EAX_Index_MM2,
        ];

        private readonly DateTime assessedDateTime = new(2025, 01, 01, 0, 0, 0);

        private readonly ContentBlockGraphQLClient contentBlockClient;

        private readonly DataPackageWorkflowServiceStub dataPackageWorkflowServiceStub;

        public SaveDerivedPriceEventHandlerTests(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);

            dataPackageWorkflowServiceStub = GetService<DataPackageWorkflowServiceStub>();
            dataPackageWorkflowServiceStub.ClearRequests();
        }

        [Fact(Skip = "skipping test until LNG set to advanced workflow")]
        public async Task Should_Correction_Update_Derived_Prices_On_Correction_Update_Of_Input_Price()
        {
            const string HalfMonthInputsContentBlockId = "a7436dfb-ce01-459e-92a1-a82b18017625";
            const string FullMonthDerivedContentBlockId = "ae99fbf9-81dc-46c2-aa97-4e159de7db79";
            const string EaxHalfMonthFortnightDerivedContentBlockId = "b7280eec-4aa4-4959-8b2f-0d64b9ce39cd";
            const string EaxDerivedContentBlockId = "3b0f5b49-f40b-442c-913f-a0d30d3f05c0";

            await PublishHalfMonthInputs(HalfMonthInputsContentBlockId, assessedDateTime);

            await PublishDerivedContentBlock(FullMonthDerivedContentBlockId, assessedDateTime, GetFullMonthInputSeriesIds);
            await PublishDerivedContentBlock(EaxHalfMonthFortnightDerivedContentBlockId, assessedDateTime, GetEaxHalfMonthInputFortnightIds);
            await PublishDerivedContentBlock(EaxDerivedContentBlockId, assessedDateTime, GetEaxFullMonthInputIds);

            // Act
            var correctInputsBusinessFlow =
                new PriceEntryBusinessFlow(
                        HalfMonthInputsContentBlockId,
                        GetHalfMonthInputSeriesIds,
                        HttpClient)
                   .InitiateCorrection(assessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(100),
                        "correction");

            await correctInputsBusinessFlow.Execute();

            // Assert
            var contentBlocksToAssert = new List<(string description, string contentBlockId)>()
            {
                ("half-month-inputs", HalfMonthInputsContentBlockId),
                ("full-month-derived", FullMonthDerivedContentBlockId),
                ("eax-half-month-derived", EaxHalfMonthFortnightDerivedContentBlockId),
                ("eax-full-month-derived", EaxDerivedContentBlockId)
            };

            foreach (var (description, contentBlockId) in contentBlocksToAssert)
            {
                var response = await contentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(contentBlockId, assessedDateTime)
                                   .GetRawResponse();

                response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create($"{description}-prices"));
            }
        }

        [Fact(Skip = "skipping test until LNG set to advanced workflow")]
        public async Task Should_Correction_Update_Derived_Prices_On_Second_Correction_Update_Of_Input_Price()
        {
            // Arrange
            const string HalfMonthInputsContentBlockId = "24844d2a-0ed5-4d85-b0f7-ffa56f74208b";
            const string FullMonthDerivedContentBlockId = "ff054f0a-2844-4844-9055-42f8a8a270f4";
            const string EaxHalfMonthFortnightDerivedContentBlockId = "1ed1bb2f-322a-45db-b619-1dddf9769bef";
            const string EaxDerivedContentBlockId = "55dfd717-7e1e-4d10-b1d2-0acd089c3606";

            await PublishHalfMonthInputs(HalfMonthInputsContentBlockId, assessedDateTime);

            await PublishDerivedContentBlock(FullMonthDerivedContentBlockId, assessedDateTime, GetFullMonthInputSeriesIds);
            await PublishDerivedContentBlock(EaxHalfMonthFortnightDerivedContentBlockId, assessedDateTime, GetEaxHalfMonthInputFortnightIds);
            await PublishDerivedContentBlock(EaxDerivedContentBlockId, assessedDateTime, GetEaxFullMonthInputIds);

            // Act
            var correctInputsBusinessFlow =
                new PriceEntryBusinessFlow(
                        HalfMonthInputsContentBlockId,
                        GetHalfMonthInputSeriesIds,
                        HttpClient)
                   .InitiateCorrection(assessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(100),
                        "correction")
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(200),
                        "correction");

            await correctInputsBusinessFlow.Execute();

            // Assert
            var contentBlocksToAssert = new List<(string description, string contentBlockId)>()
            {
                ("half-month-inputs", HalfMonthInputsContentBlockId),
                ("full-month-derived", FullMonthDerivedContentBlockId),
                ("eax-half-month-derived", EaxHalfMonthFortnightDerivedContentBlockId),
                ("eax-full-month-derived", EaxDerivedContentBlockId)
            };

            foreach (var (description, contentBlockId) in contentBlocksToAssert)
            {
                var response = await contentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(contentBlockId, assessedDateTime)
                                   .GetRawResponse();

                response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create($"{description}-prices"));
            }
        }

        [Fact(Skip = "skipping test until LNG set to advanced workflow")]
        public async Task Should_Not_Correction_Update_Derived_Prices_On_Correction_Workflow_Status_Updates()
        {
            // Arrange
            const string HalfMonthInputsContentBlockId = "21351096-b592-4393-9975-ab43a5dffa8a";
            const string FullMonthDerivedContentBlockId = "c406d02f-baf9-4099-a7b6-065a1bc51f7a";
            const string EaxHalfMonthFortnightDerivedContentBlockId = "020d7ebb-ef45-4fe1-8b9e-8826b76a35f6";
            const string EaxDerivedContentBlockId = "359b29a9-d2cf-4d06-956d-ef79ca260de1";

            await PublishHalfMonthInputs(HalfMonthInputsContentBlockId, assessedDateTime);

            await PublishDerivedContentBlock(FullMonthDerivedContentBlockId, assessedDateTime, GetFullMonthInputSeriesIds);
            await PublishDerivedContentBlock(EaxHalfMonthFortnightDerivedContentBlockId, assessedDateTime, GetEaxHalfMonthInputFortnightIds);
            await PublishDerivedContentBlock(EaxDerivedContentBlockId, assessedDateTime, GetEaxFullMonthInputIds);

            // Act
            var correctInputsBusinessFlow =
                new PriceEntryBusinessFlow(HalfMonthInputsContentBlockId, GetHalfMonthInputSeriesIds, HttpClient)
                   .InitiateCorrection(assessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(100),
                        "correction")
                   .AdvancedCorrectionInitiateSendForReview(assessedDateTime)
                   .AdvancedCorrectionAcknowledgeSendForReview(assessedDateTime);

            await correctInputsBusinessFlow.Execute();

            // Assert
            var contentBlocksToAssert = new List<(string description, string contentBlockId)>()
            {
                ("half-month-inputs", HalfMonthInputsContentBlockId),
                ("full-month-derived", FullMonthDerivedContentBlockId),
                ("eax-half-month-derived", EaxHalfMonthFortnightDerivedContentBlockId),
                ("eax-full-month-derived", EaxDerivedContentBlockId)
            };

            foreach (var (description, contentBlockId) in contentBlocksToAssert)
            {
                var response = await contentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(contentBlockId, assessedDateTime)
                                   .GetRawResponse();

                response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create($"{description}-prices"));
            }
        }

        [Fact(Skip = "skipping test until LNG set to advanced workflow")]
        public async Task Should_Correction_Update_Derived_Prices_On_Correction_Update_In_Review_Of_Input_Price()
        {
            const string HalfMonthInputsContentBlockId = "a7436dfb-ce01-459e-92a1-a82b18017625";
            const string FullMonthDerivedContentBlockId = "ae99fbf9-81dc-46c2-aa97-4e159de7db79";
            const string EaxHalfMonthFortnightDerivedContentBlockId = "b7280eec-4aa4-4959-8b2f-0d64b9ce39cd";
            const string EaxDerivedContentBlockId = "3b0f5b49-f40b-442c-913f-a0d30d3f05c0";

            await PublishHalfMonthInputs(HalfMonthInputsContentBlockId, assessedDateTime);

            await PublishDerivedContentBlock(FullMonthDerivedContentBlockId, assessedDateTime, GetFullMonthInputSeriesIds);
            await PublishDerivedContentBlock(EaxHalfMonthFortnightDerivedContentBlockId, assessedDateTime, GetEaxHalfMonthInputFortnightIds);
            await PublishDerivedContentBlock(EaxDerivedContentBlockId, assessedDateTime, GetEaxFullMonthInputIds);

            // Act
            var correctInputsBusinessFlow =
                new PriceEntryBusinessFlow(HalfMonthInputsContentBlockId, GetHalfMonthInputSeriesIds, HttpClient)
                   .InitiateCorrection(assessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(100),
                        "correction")
                   .AdvancedCorrectionInitiateSendForReview(assessedDateTime)
                   .AdvancedCorrectionAcknowledgeSendForReview(assessedDateTime)
                   .AdvancedCorrectionInitiateStartReview(assessedDateTime)
                   .AdvancedCorrectionAcknowledgeStartReview(assessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(300),
                        "correction");

            await correctInputsBusinessFlow.Execute();

            // Assert
            var contentBlocksToAssert = new List<(string description, string contentBlockId)>()
            {
                ("half-month-inputs", HalfMonthInputsContentBlockId),
                ("full-month-derived", FullMonthDerivedContentBlockId),
                ("eax-half-month-derived", EaxHalfMonthFortnightDerivedContentBlockId),
                ("eax-full-month-derived", EaxDerivedContentBlockId)
            };

            foreach (var (description, contentBlockId) in contentBlocksToAssert)
            {
                var response = await contentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(contentBlockId, assessedDateTime)
                                   .GetRawResponse();

                response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create($"{description}-prices"));
            }
        }

        [Fact(Skip = "skipping test until LNG set to advanced workflow")]
        public async Task Should_Allow_Manual_Correction_With_Existing_Calculations_If_Workflow_Fails_Repeatedly()
        {
            const string HalfMonthInputsContentBlockId = "a7436dfb-ce01-459e-92a1-a82b18017625";
            const string FullMonthDerivedContentBlockId = "ae99fbf9-81dc-46c2-aa97-4e159de7db79";
            const string EaxHalfMonthFortnightDerivedContentBlockId = "b7280eec-4aa4-4959-8b2f-0d64b9ce39cd";
            const string EaxDerivedContentBlockId = "3b0f5b49-f40b-442c-913f-a0d30d3f05c0";

            await PublishHalfMonthInputs(HalfMonthInputsContentBlockId, assessedDateTime);

            await PublishDerivedContentBlock(FullMonthDerivedContentBlockId, assessedDateTime, GetFullMonthInputSeriesIds);
            await PublishDerivedContentBlock(EaxHalfMonthFortnightDerivedContentBlockId, assessedDateTime, GetEaxHalfMonthInputFortnightIds);
            await PublishDerivedContentBlock(EaxDerivedContentBlockId, assessedDateTime, GetEaxFullMonthInputIds);

            // Act
            var startCorrectionOnInputsBusinessFlow = new PriceEntryBusinessFlow(
                HalfMonthInputsContentBlockId,
                GetHalfMonthInputSeriesIds,
                HttpClient).InitiateCorrection(assessedDateTime);

            await startCorrectionOnInputsBusinessFlow.Execute();

            // Workflow to fail for all following calls
            dataPackageWorkflowServiceStub.FailToStartWorkflowOnce();

            var correctionUpdateOnInputBusinessFlow = new PriceEntryBusinessFlow(
                HalfMonthInputsContentBlockId,
                GetHalfMonthInputSeriesIds,
                HttpClient)
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_China_HM1,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(100),
                    "correction");

            await correctionUpdateOnInputBusinessFlow.Execute();

            dataPackageWorkflowServiceStub.ClearRequests();

            await CorrectDerivedContentBlock(FullMonthDerivedContentBlockId, assessedDateTime, GetFullMonthInputSeriesIds);
            await CorrectDerivedContentBlock(EaxHalfMonthFortnightDerivedContentBlockId, assessedDateTime, GetEaxHalfMonthInputFortnightIds);
            await CorrectDerivedContentBlock(EaxDerivedContentBlockId, assessedDateTime, GetEaxFullMonthInputIds);

            // Assert
            var contentBlocksToAssert = new List<(string description, string contentBlockId)>()
            {
                ("half-month-inputs", HalfMonthInputsContentBlockId),
                ("full-month-derived", FullMonthDerivedContentBlockId),
                ("eax-half-month-derived", EaxHalfMonthFortnightDerivedContentBlockId),
                ("eax-full-month-derived", EaxDerivedContentBlockId)
            };

            foreach (var (description, contentBlockId) in contentBlocksToAssert)
            {
                var response = await contentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(contentBlockId, assessedDateTime)
                                   .GetRawResponse();

                response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create($"{description}-prices"));
            }
        }

        private async Task PublishHalfMonthInputs(string inputsContentBlockId, DateTime assessedDateTime)
        {
            var publishHalfMonthInputsBusinessFlow =
                new PriceEntryBusinessFlow(
                        inputsContentBlockId,
                        GetHalfMonthInputSeriesIds,
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime);

            await publishHalfMonthInputsBusinessFlow.Execute();
        }

        private async Task PublishDerivedContentBlock(string derivedContentBlockId, DateTime assessedDateTime, List<string> derivedPriceSeriesIds)
        {
            var setupDerivedContentBlockBusinessFlow = new PriceEntryBusinessFlow(
                    derivedContentBlockId,
                    derivedPriceSeriesIds,
                    HttpClient)
               .SaveContentBlockDefinition()
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);

            await setupDerivedContentBlockBusinessFlow.Execute();
        }

        private async Task CorrectDerivedContentBlock(string derivedCopntentBlockId, DateTime assessedDateTime, List<string> derivedPriceSeriesIds)
        {
            var cancelAndCorrectBusinessFlow =
                new PriceEntryBusinessFlow(derivedCopntentBlockId, derivedPriceSeriesIds, HttpClient)
                   .InitiateCorrection(assessedDateTime);

            await cancelAndCorrectBusinessFlow.Execute();
        }
    }
}
