namespace Test.Infrastructure.TestData
{
    using System;

    public static class TestData
    {
        public static readonly Guid UserGuid = new("1f2a74bc-f5a5-46f2-aad3-a2916827be18");

        public static readonly DateTime Now = new(2024, 01, 10, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime GasPricePublishedDateTime = new(2017, 08, 14, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime GasPriceFulfilmentFromDateTime = new(2017, 09, 01, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime GasPriceFulfilmentUntilDateTime = new(2017, 09, 30, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime TerminatedSeriesAssessmentDate = new(2019, 12, 20, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime TerminatedSeriesTerminationDate = new(2019, 12, 24, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime PreLaunchMelamineSeriesDate = new(2019, 12, 24, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime PreLaunchMelamineChinaWeeklyDate = new(2001, 12, 24, 0, 0, 0, DateTimeKind.Utc);

        public static readonly Guid CommodityLng = new Guid("fa627c9b-3d8d-4ff5-b9d9-1abca5049d5f");
        public static readonly Guid CommodityMelamine = new Guid("9261bd6e-bfe6-4d5f-805d-2966bd41a06b");
        public static readonly Guid PriceCategoryAssessed = new Guid("0bb95d7a-db25-4cb7-8c2a-a35994a000ee");
        public static readonly Guid RegionAsiaPacific = new Guid("26494825-d4ba-4f88-b855-24e4b1b79f2b");
        public static readonly Guid RegionWorld = new Guid("fdd61dcc-5143-42d8-9404-cbba2b60824c");
        public static readonly Guid RegionEurope = new Guid("636bd012-eadf-4c22-a4d7-5578de437ec6");
        public static readonly Guid TransactionTypeSpot = new Guid("8cd8f6cf-a7fb-4afa-a421-d05c5855df22");
        public static readonly Guid TransactionTypeNotApplicable = Guid.Empty;
        public static readonly Guid TransactionTypeContract = new Guid("1600a1f5-bb8f-4af0-9e2c-550dda299207");
        public static readonly Guid FrequencyDaily = new Guid("de0f5c7f-c082-445b-a1d2-89e3a350766a");
        public static readonly Guid FrequencyQuarterly = new Guid("9929ba54-a743-4e1c-8ad0-aec3ddfc0afe");
        public static readonly Guid FrequencyWeekly = new Guid("7c7cb127-5828-4923-8f43-f37b86696c2c");

        public static readonly Guid MelamineOldWhichDoesNotExistGuid = new("67fa1b8d-b6b1-4d8b-8d56-41ab8e7844ae");

        public static string NonMarketAdjustment = "NONMKTADJ";

        public static string CommentaryId = "92206d33-b512-49a2-941f-f566af6d3468";
    }
}