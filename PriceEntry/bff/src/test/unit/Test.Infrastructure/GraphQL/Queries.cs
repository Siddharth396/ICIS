namespace Test.Infrastructure.GraphQL
{
    public static class Queries
    {
        public const string GridConfigurationFragment = @"fragment GridConfigurationFragment on GridConfiguration {
                sort{
                    name
                    order
                },
                columns{
                    field
                    headerName
                    cellDataType
                    displayOrder
                    editable
                    values
                    pinned
                    editableWhen{
                        field
                        value
                    }
                    type
                    customConfig{
                        priceDelta{
                            priceField
                            priceDeltaField
                            precisionField
                        }
                    }
                    tooltipField
                    cellType
                    columnOrder
                    hideable
                    hidden
                },
                seriesItemTypeCode
            }";

        public const string PriceSeriesFragment = @"fragment PriceSeriesFragment on PriceSeries {
                id
                seriesName
                seriesShortName
                seriesItemTypeCode
                commodity {
                    name
                }
                location {
                    name
                    region
                }
                currencyUnit {
                    name
                    code
                }
                measureUnit {
                    name
                    symbol
                }
                launchDate 
                terminationDate
                relativeFulfilmentPeriod {
                    name
                    code
                    periodType {
                        name
                        code
                    }
                }
                priceSeriesName
                unitDisplay
                seriesId
                seriesItemId
                assessedDateTime
                dataUsed
                status
                price
                priceDelta
                priceLow
                priceHigh
                priceMid
                lastAssessmentDate
                lastAssessmentPrice
                lastAssessmentPriceValue
                lastAssessmentPriceLowValue
                lastAssessmentPriceHighValue
                lastAssessmentPriceMidValue
                lastAssessmentPremiumDiscount
                assessmentMethod
                premiumDiscount
                referencePrice {
                    market
                    price
                    datetime
                    seriesName
                    periodLabel 
                }
                priceLowDelta
                priceMidDelta
                priceHighDelta
                period
                readOnly
                hasImpactedPrices
                adjustedPriceDelta
                adjustedPriceHighDelta
                adjustedPriceLowDelta
                adjustedPriceMidDelta
                publicationScheduleId
            }";

        public const string GetContentBlock = $@"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!){{
            contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime){{
                contentBlockId
                version
                title
                publicationScheduleId
                priceSeriesGrids {{
                  gridConfiguration {{
                    ... GridConfigurationFragment
                  }}  
                  priceSeries{{
                    ... PriceSeriesFragment
                  }}
                }}
            }}
        }}
        {GridConfigurationFragment}
        {PriceSeriesFragment}
        ";

        public const string GetContentBlockOnly = @"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!){
            contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime){
                contentBlockId
                version
                title
            }
        }";

        public const string GetContentBlockWithGridConfigurationOnly = $@"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!){{
            contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime){{
                contentBlockId
                version
                title
                priceSeriesGrids {{
                    gridConfiguration {{
                        ... GridConfigurationFragment
                    }}
                }}
                
            }}
        }}
        {GridConfigurationFragment}
        ";

        public const string GetContentBlockWithWorkflowBusinessKeyOnly = @"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!){
            contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime){
                contentBlockId
                version
                workflowBusinessKey
            }
        }";

        public const string GetContentBlockWithPriceSeriesOnly = $@"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!, $isReviewMode:Boolean){{
            contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime, isReviewMode:$isReviewMode){{
                contentBlockId
                version
                priceSeriesGrids {{
                  priceSeries{{
                    ... PriceSeriesFragment
                  }}
                }}
            }}
        }}
        {PriceSeriesFragment}
        ";

        public const string GetContentBlockWithPriceSeriesValidationErrorsOnly =
            @"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!, $includeNotStarted:Boolean!){
                contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime){
                    contentBlockId
                    version
                    priceSeriesGrids {
                        priceSeries {
                            id
                            validationErrors(includeNotStarted: $includeNotStarted)
                        }
                    }
                }
            }";

        public const string GetContentBlockWithCommentaryOnly =
            @"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!){
                contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime:$assessedDateTime){
                    contentBlockId
                    version
                    commentary{
                      commentaryId
                      version
                    }
                }
            }";

        public const string GetPriceSeriesWithFilters = @"query getPriceSeriesWithFilters (
                $commodityId:UUID!,
                $priceCategoryId:UUID!
                $regionId:UUID!
                $priceSettlementTypeId:UUID!
                $itemFrequencyId:UUID!
            ){
            priceSeries(
                commodityId: $commodityId,
                priceCategoryId: $priceCategoryId,
                regionId: $regionId,
                priceSettlementTypeId: $priceSettlementTypeId,
                itemFrequencyId: $itemFrequencyId
            ) {
                   priceSeriesDetails {
                      id
                      name
                      scheduleId
                }
                seriesItemTypeCode
            }
        }";

        public const string GetFilters = @"query getFilters ($selectedPriceSeriesIds: [String!]){
            filters(selectedPriceSeriesIds: $selectedPriceSeriesIds) {
                   name
                   filterDetails {
                      id
                      name
                      isDefault
                }
            }
        }";

        public const string GetContentBlockWithNextActionsOnly = @"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!, $isReviewMode:Boolean){
            contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime:$assessedDateTime, isReviewMode:$isReviewMode){
                contentBlockId
                version
                nextActions{
                    name
                    enabled
                    displayValue
                }
            }
        }";

        public const string GetContentBlockWithDataPackageIdOnly = @"query getContentBlock($contentBlockId:String!, $version:Int, $assessedDateTime:DateTime!){
            contentBlock(contentBlockId:$contentBlockId, version:$version, assessedDateTime: $assessedDateTime){
                contentBlockId
                version
                dataPackageId()
            }
        }";

        public const string GetImpactedPrices = @"query getImpactedPrices($priceSeriesId:String!, $assessedDateTime:DateTime!){
            impactedPrices(priceSeriesId:$priceSeriesId, assessedDateTime:$assessedDateTime){
                isSuccess
                errorCode
                impactedPrices {
                    seriesName
                    impactedDerivedPriceSeriesIds
                    impactedReferencePriceSeriesIds
                    impactedCalculatedPriceSeriesIds
                }
            }
        }";
    }
}
