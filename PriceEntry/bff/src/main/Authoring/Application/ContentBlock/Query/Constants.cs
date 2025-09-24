namespace Authoring.Application.ContentBlock.Query
{
    public static class Constants
    {
        public static class ContentBlock
        {
            public const string AssessedDateTimeParameter = "assessedDateTime";

            public const string ContentBlockField = "contentBlock";

            public const string GridConfigurationField = "gridConfiguration";

            public const string PriceSeriesField = "priceSeries";

            public const string CommentaryField = "commentary";

            public const string DataPackageId = "dataPackageId";

            public const string WorkflowBusinessKey = "workflowBusinessKey";

            public const string NextActions = "nextActions";

            public const string NonMarketAdjustmentEnabled = "nmaEnabled";

            public const string PublicationScheduleId = "publicationScheduleId";
        }

        public static class PriceSeries
        {
            public const string PeriodField = "period";

            public const string ReadOnlyField = "readOnly";

            public const string ValidationErrorsField = "validationErrors";

            public const string HasImpactedPricesField = "hasImpactedPrices";
        }

        public static class ScopedContext
        {
            public const string ContentBlockDefinition = "contentBlockDefinition";

            public const string IsReviewMode = "isReviewMode";

            public const string DataPackageStatus = "dataPackageStatus";
        }

        public static class ImpactedPrices
        {
            public const string ImpactedPricesField = "impactedPrices";
        }
    }
}
