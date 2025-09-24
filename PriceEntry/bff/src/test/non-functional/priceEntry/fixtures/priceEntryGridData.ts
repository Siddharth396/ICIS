// These queries are outdated and need to be updated

// GraphQL query to fetch content block grid data and price series using seriesIds and assessedDateTime
export const GetContentBlock = `
query getContentBlock(
$contentBlockId: String!, 
$version: Int, 
$assessedDateTime: DateTime!, 
$includeNotStarted: Boolean!, 
$isReviewMode: Boolean
) {
  contentBlock(
    contentBlockId: $contentBlockId
    version: $version
    assessedDateTime: $assessedDateTime
    isReviewMode: $isReviewMode
  ) {
    contentBlockId
    version
    title
    priceSeriesIds
    publicationScheduleId
    priceSeriesGrids {
      id
      title
      priceSeriesIds
      seriesItemTypeCode
      gridConfiguration {
        columns {
          field
          headerName
          cellDataType
          displayOrder
          columnOrder
          editable
          values
          pinned
          editableWhen {
            field
            value
            __typename
          }
          type
          customConfig {
            priceDelta {
              priceField
              priceDeltaField
              precisionField
              __typename
            }
            __typename
          }
          tooltipField
          cellType
          hideable
          hidden
          __typename
        }
        sort {
          name
          order
          __typename
        }
        seriesItemTypeCode
        __typename
      }
      priceSeries {
        id
        seriesName
        period
        seriesShortName
        seriesItemTypeCode
        commodity {
          name
          __typename
        }
        location {
          name
          region
          __typename
        }
        currencyUnit {
          name
          code
          __typename
        }
        measureUnit {
          name
          symbol
          __typename
        }
        launchDate
        terminationDate
        relativeFulfilmentPeriod {
          name
          code
          periodType {
            name
            code
            __typename
          }
          __typename
        }
        priceSeriesName
        unitDisplay
        seriesItemId
        seriesId
        assessedDateTime
        dataUsed
        price
        priceLow
        priceLowDelta
        adjustedPriceDelta
        adjustedPriceHighDelta
        adjustedPriceLowDelta
        adjustedPriceMidDelta
        lastAssessmentPriceValue
        lastAssessmentPriceHighValue
        lastAssessmentPriceLowValue
        lastAssessmentPriceMidValue
        priceMid
        priceMidDelta
        priceHigh
        readOnly
        isDerivedPriceSeries
        priceHighDelta
        adjustedPriceHighDelta
        status
        dataUsed
        lastAssessmentDate
        lastAssessmentPrice
        lastAssessmentPremiumDiscount
        assessmentMethod
        priceDelta
        referencePrice {
          market
          price
          datetime
          seriesName
          periodLabel
          __typename
        }
        premiumDiscount
        validationErrors(includeNotStarted: $includeNotStarted)
        hasImpactedPrices
        __typename
      }
      __typename
    }
    commentary {
      commentaryId
      version
      __typename
    }
    dataPackageId
    nmaEnabled
    workflowBusinessKey
    __typename
  }
}
`;
