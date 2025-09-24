namespace Test.Infrastructure.GraphQL
{
    public static class QueriesForDisplay
    {
        public const string GridConfigurationFragment = @"fragment GridConfigurationFragment on PriceDisplayGridConfiguration {
            sort{
                name
                order
            },
            columns {
                field
                headerName
                cellDataType
                displayOrder
                columnOrder
                hidden
                hideable
                values
                pinned
                type
                tooltipField
                customConfig {
                    priceDelta {
                        priceField
                        priceDeltaField
                        precisionField
                    }
                }
             cellType
             minWidth
             maxWidth
             width
             alternateFields {
               seriesItemTypeCodes
               field
               priceDeltaField
             }
             autoSize
            }
        }";

        public const string GetAuthoringContentBlockForDisplay = @"query getContentBlockForDisplay($contentBlockId:String!, $version:Int){
            contentBlockForDisplay(contentBlockId:$contentBlockId, version:$version){
                contentBlockId
                version
                title
                columns {
                    field
                    displayOrder
                    hidden
                }
                rows {
                    priceSeriesId
                    displayOrder
                    seriesItemTypeCode
                }
                selectedFilters {
                   isInactiveIncluded
                   selectedAssessedFrequencies
                   selectedCommodities
                   selectedPriceCategories
                   selectedRegions
                   selectedTransactionTypes
                }
            }
        }";

        public const string GetContentBlockFromInputParametersForDisplay = @"query GetContentBlockFromInputParametersForPriceDisplay($seriesIds:[String!]!, $assessedDateTime:DateTime!){
            contentBlockFromInputParametersForDisplay(seriesIds:$seriesIds, assessedDateTime:$assessedDateTime){
                contentBlockId
                version
                title
                columns {
                    field
                    displayOrder
                    hidden
                }
                rows {
                    priceSeriesId
                    displayOrder
                    seriesItemTypeCode
                }
                priceSeriesItemForDisplay {
                    id
                    priceSeriesName
                    period
                    unitDisplay
                    itemFrequencyName
                    dataUsed
                    price
                    priceLow        
                    priceHigh
                    priceMid
                    priceDelta
                    priceLowDelta
                    priceHighDelta
                    priceMidDelta
                    status
                    assessedDateTime
                    lastModifiedDate
                    seriesItemTypeCode
                }
            }
        }";

        public const string GetSubscriberContentBlockForDisplay = @"query getContentBlockForDisplay($contentBlockId:String!, $version:Int){
            contentBlockForDisplay(contentBlockId:$contentBlockId, version:$version){
                contentBlockId
                version
                title
                columns {
                    field
                    displayOrder
                    hidden
                }
                rows {
                    priceSeriesId
                    displayOrder
                    seriesItemTypeCode
                }
                selectedFilters {
                   isInactiveIncluded
                   selectedAssessedFrequencies
                   selectedCommodities
                   selectedPriceCategories
                   selectedRegions
                   selectedTransactionTypes
                }
            }
        }";

        public const string GetAuthoringContentBlockWithGridConfigurationOnly = $@"query getContentBlockForDisplay($contentBlockId:String!, $version:Int){{
            contentBlockForDisplay(contentBlockId:$contentBlockId, version:$version){{
                contentBlockId
                version
                title
                columns {{
                    field
                    displayOrder
                    hidden
                }}
                rows {{
                    priceSeriesId
                    displayOrder
                    seriesItemTypeCode
                }}
                priceDisplayGridConfiguration {{
                    ... GridConfigurationFragment
                }}
                selectedFilters {{
                   isInactiveIncluded
                   selectedAssessedFrequencies
                   selectedCommodities
                   selectedPriceCategories
                   selectedRegions
                   selectedTransactionTypes
                }}
            }}
        }}
        {GridConfigurationFragment}
        ";

        public const string GetSubscriberContentBlockWithGridConfigurationOnly = $@"query getContentBlockForDisplay($contentBlockId:String!, $version:Int){{
            contentBlockForDisplay(contentBlockId:$contentBlockId, version:$version){{
                contentBlockId
                version
                title
                columns {{
                    field
                    displayOrder
                    hidden
                }}
                rows {{
                    priceSeriesId
                    displayOrder
                    seriesItemTypeCode
                }}
                priceDisplayGridConfiguration {{
                    ... GridConfigurationFragment
                }}
                selectedFilters {{
                    isInactiveIncluded
                    selectedAssessedFrequencies
                    selectedCommodities
                    selectedPriceCategories
                    selectedRegions
                    selectedTransactionTypes
                }}
            }}
        }}
        {GridConfigurationFragment}
        ";

        public const string GetAuthoringPriceSeriesForDisplayWithFilters = @"query GetPriceSeriesForDisplayWithFilters (
           $commodities: [UUID!]!,
           $includeInactivePriceSeries: Boolean!) 
           {
             priceSeriesForDisplayTool(
                  commodities: $commodities,
                   includeInactivePriceSeries: $ includeInactivePriceSeries) 
                  {
                       priceSeriesDetails {
                         id
                         name
                         seriesItemTypeCode
                         priceCategoryId
                         itemFrequencyId
                         priceSettlementTypeId
                         regionId
                       }
                       priceCategories {
                         id
                         name
                       }
                       assessedFrequencies {
                         id
                         name
                       }
                       regions {
                         id
                         name
                       }
                       transactionTypes {
                         id
                         name
                       }
                  }
          }";

        public const string GetAuthoringContentBlockForDisplayWithPriceSeriesOnly = @"query getContentBlockForDisplay($contentBlockId:String!, $version:Int){
            contentBlockForDisplay(contentBlockId:$contentBlockId, version:$version){
                contentBlockId
                version
                title
                columns {
                    field
                    displayOrder
                    hidden
                }
                rows {
                    priceSeriesId
                    displayOrder
                    seriesItemTypeCode
                }
                priceSeriesItemForDisplay {
                    id
                    priceSeriesName
                    period
                    unitDisplay
                    itemFrequencyName
                    dataUsed
                    priceDeltaType
                    price
                    priceLow        
                    priceHigh
                    priceMid
                    priceDelta
                    priceLowDelta
                    priceHighDelta
                    priceMidDelta
                    status
                    assessedDateTime
                    lastModifiedDate
                    seriesItemTypeCode
                    previousVersion {
                        price
                        priceLow
                        priceMid
                        priceHigh
                    }
                }
                selectedFilters {
                    isInactiveIncluded
                    selectedAssessedFrequencies
                    selectedCommodities
                    selectedPriceCategories
                    selectedRegions
                    selectedTransactionTypes
                }
            }
        }";

        public const string GetSubscriberContentBlockForDisplayWithPriceSeriesOnly = @"query getContentBlockForDisplay($contentBlockId:String!, $version:Int, $assessedDateTime: Long!){
            contentBlockForDisplay(contentBlockId:$contentBlockId, version:$version){
                contentBlockId
                version
                title
                columns {
                    field
                    displayOrder
                    hidden
                }
                rows {
                    priceSeriesId
                    displayOrder
                    seriesItemTypeCode
                }
                priceSeriesItemForDisplay(assessedDateTime: $assessedDateTime) {
                    id
                    priceSeriesName
                    period
                    unitDisplay
                    itemFrequencyName
                    dataUsed
                    priceDeltaType
                    price
                    priceLow        
                    priceHigh
                    priceMid
                    priceDelta
                    priceLowDelta
                    priceHighDelta
                    priceMidDelta
                    status
                    assessedDateTime
                    lastModifiedDate
                    seriesItemTypeCode
                    previousVersion {
                        price
                        priceLow
                        priceMid
                        priceHigh
                    }
                }
                selectedFilters {
                    isInactiveIncluded
                    selectedAssessedFrequencies
                    selectedCommodities
                    selectedPriceCategories
                    selectedRegions
                    selectedTransactionTypes
                }
            }
        }";

        public const string GetCommodities = @"query query {
        commodities {
                      id
                      name
                    }
        }";
    }
}
