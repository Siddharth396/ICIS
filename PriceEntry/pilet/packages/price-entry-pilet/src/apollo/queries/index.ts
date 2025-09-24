import gql from 'graphql-tag';

export const GET_CONTENT_BLOCK = gql`
  query getContentBlock(
    $contentBlockId: String!
    $version: Int
    $assessedDateTime: DateTime!
    $includeNotStarted: Boolean!
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
            }
            type
            customConfig {
              priceDelta {
                priceField
                priceDeltaField
                precisionField
              }
            }
            tooltipField
            cellType
            hideable
            hidden
          }
          sort {
            name
            order
          }
          seriesItemTypeCode
        }
        priceSeries {
          id
          seriesName
          period
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
          }
          premiumDiscount
          validationErrors(includeNotStarted: $includeNotStarted)
          hasImpactedPrices
          publicationScheduleId
        }
      }
      commentary {
        commentaryId
        version
      }
      dataPackageId
      nextActions {
        name
        displayValue
        enabled
      }
      nmaEnabled
      workflowBusinessKey
    }
  }
`;

export const GET_EVENTS = gql`
  query schedules($scheduleId: UUID!, $startDate: Date!, $endDate: Date, $limit: Int) {
    events(scheduleId: $scheduleId, startDate: $startDate, endDate: $endDate, limit: $limit) {
      ... on ScheduleEvents {
        schedule {
          id
          name
        }
        events {
          eventTime
        }
      }
      ... on EventInputValidationError {
        message
        errors
      }
    }
  }
`;

export const GET_PRICE_DISPLAY_CONTENT_BLOCK = gql`
  query getContentBlockForDisplay($contentBlockId: String!, $version: Int) {
    contentBlockForDisplay(contentBlockId: $contentBlockId, version: $version) {
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
      priceDisplayGridConfiguration {
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
      }
      priceSeriesItemForDisplay {
        id
        priceSeriesName
        unitDisplay
        price
        priceLow
        priceLowDelta
        priceMid
        priceMidDelta
        priceHigh
        priceHighDelta
        status
        dataUsed
        assessmentMethod
        priceDelta
        assessedDateTime
        itemFrequencyName
        period
        seriesItemTypeCode
        lastModifiedDate
        priceDeltaType
        previousVersion {
          price
          priceHigh
          priceLow
          priceMid
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
  }
`;

export const GET_PRICE_DISPLAY_CONTENT_BLOCK_SUBSCRIBER = gql`
  query getContentBlockForDisplay(
    $contentBlockId: String!
    $version: Int
    $assessedDateTime: Long!
  ) {
    contentBlockForDisplay(contentBlockId: $contentBlockId, version: $version) {
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
      priceDisplayGridConfiguration {
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
      }
      priceSeriesItemForDisplay(assessedDateTime: $assessedDateTime) {
        id
        priceSeriesName
        unitDisplay
        price
        priceLow
        priceLowDelta
        priceMid
        priceMidDelta
        priceHigh
        priceHighDelta
        status
        dataUsed
        assessmentMethod
        priceDelta
        assessedDateTime
        itemFrequencyName
        period
        seriesItemTypeCode
        lastModifiedDate
        priceDeltaType
        previousVersion {
          price
          priceHigh
          priceLow
          priceMid
        }
      }
    }
  }
`;

export const GET_IMPACTED_PRICES = gql`
  query getImpactedPrices($priceSeriesId: String!, $assessedDateTime: DateTime!) {
    impactedPrices(priceSeriesId: $priceSeriesId, assessedDateTime: $assessedDateTime) {
      isSuccess
      errorCode
      impactedPrices {
        impactedDerivedPriceSeriesIds
        impactedReferencePriceSeriesIds
        impactedCalculatedPriceSeriesIds
      }
    }
  }
`;

export const GET_IMPACT_PRICE_DISPLAY_CONTENT_BLOCK = gql`
  query contentBlockFromInputParametersForDisplay(
    $seriesIds: [String!]!
    $assessedDateTime: DateTime!
  ) {
    contentBlockFromInputParametersForDisplay(
      seriesIds: $seriesIds
      assessedDateTime: $assessedDateTime
    ) {
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
      priceDisplayGridConfiguration {
        sort {
          name
          order
        }
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
  }
`;

export const GET_PRICE_SERIES = gql`
  query priceSeries(
    $commodityId: UUID!
    $priceCategoryId: UUID!
    $regionId: UUID!
    $priceSettlementTypeId: UUID!
    $itemFrequencyId: UUID!
  ) {
    priceSeries(
      commodityId: $commodityId
      priceCategoryId: $priceCategoryId
      regionId: $regionId
      priceSettlementTypeId: $priceSettlementTypeId
      itemFrequencyId: $itemFrequencyId
    ) {
      priceSeriesDetails {
        id
        name
        scheduleId
      }
      seriesItemTypeCode
    }
  }
`;

export const GET_PRICE_SERIES_FOR_PRICE_SERIES_DISPLAY_TOOL = gql`
  query priceSeriesForDisplayTool($commodities: [UUID!]!, $includeInactivePriceSeries: Boolean!) {
    priceSeriesForDisplayTool(
      commodities: $commodities
      includeInactivePriceSeries: $includeInactivePriceSeries
    ) {
      priceSeriesDetails {
        id
        itemFrequencyId
        regionId
        priceSettlementTypeId
        priceCategoryId
        name
        seriesItemTypeCode
      }
      regions {
        name
        id
      }
      priceCategories {
        name
        id
      }
      transactionTypes {
        name
        id
      }
      assessedFrequencies {
        name
        id
      }
    }
  }
`;

export const GET_COMMODITIES_FOR_PRICE_SERIES_DISPLAY_TOOL = gql`
  query commodities {
    commodities {
      id
      name
    }
  }
`;

export const UPDATE_PRICE_SERIES_DATA = gql`
  mutation updatePriceEntryGridData($priceItemInput: PriceItemInput!) {
    updatePriceEntryGridData(priceItemInput: $priceItemInput) {
      id
    }
  }
`;

export const UPDATE_CONTENT_BLOCK = gql`
  mutation saveContentBlock($contentBlockInput: ContentBlockInput!) {
    response: saveContentBlock(contentBlockInput: $contentBlockInput) {
      contentBlockId
      version
      errorCodes
      isValid
    }
  }
`;

export const UPDATE_PRICE_DISPLAY_CONTENT_BLOCK = gql`
  mutation saveContentBlockForDisplay($contentBlockInput: ContentBlockForDisplayInput!) {
    response: saveContentBlockForDisplay(contentBlockInput: $contentBlockInput) {
      contentBlockId
      version
    }
  }
`;

export const UPDATE_COMMENTARY = gql`
  mutation saveCommentary($commentaryInput: CommentaryInput!) {
    response: saveCommentary(commentaryInput: $commentaryInput) {
      id
    }
  }
`;

export const UPDATE_USER_PREFERENCE = gql`
  mutation saveUserPreference($userPreferenceInput: UserPreferenceInput!) {
    response: saveUserPreference(userPreferenceInput: $userPreferenceInput) {
      id
    }
  }
`;

export const INITIATE_CORRECTION = gql`
  mutation initiateCorrectionForDataPackage(
    $contentBlockId: String!
    $version: Int!
    $assessedDateTime: DateTime!
    $reviewPageUrl: String!
  ) {
    initiateCorrectionForDataPackage(
      contentBlockId: $contentBlockId
      version: $version
      assessedDateTime: $assessedDateTime
      reviewPageUrl: $reviewPageUrl
    )
  }
`;

export const DATA_PACKAGE_TRANSITION_TO_STATE = gql`
  mutation dataPackageTransitionToState(
    $contentBlockId: String!
    $version: Int!
    $assessedDateTime: DateTime!
    $nextState: String!
    $operationType: String!
    $reviewPageUrl: String!
  ) {
    dataPackageTransitionToState(
      contentBlockId: $contentBlockId
      version: $version
      assessedDateTime: $assessedDateTime
      nextState: $nextState
      operationType: $operationType
      reviewPageUrl: $reviewPageUrl
    ) {
      isSuccess
      errorCode
    }
  }
`;

export const LOAD_FILTERS = gql`
  query filters($selectedPriceSeriesIds: [String!]) {
    filters(selectedPriceSeriesIds: $selectedPriceSeriesIds) {
      name
      filterDetails {
        id
        name
        isDefault
      }
    }
  }
`;

export const TOGGLE_NON_MARKET_ADJUSTMENT = gql`
  mutation toggleNonMarketAdjustment(
    $contentBlockId: String!
    $version: Int!
    $assessedDateTime: DateTime!
    $enabled: Boolean!
  ) {
    toggleNonMarketAdjustment(
      contentBlockId: $contentBlockId
      version: $version
      assessedDateTime: $assessedDateTime
      enabled: $enabled
    )
  }
`;

export interface IPriceSeriesGrid {
  priceSeries: IPriceEntrySeriesItem[];
  id: string;
  seriesItemTypeCode: string;
  title: string;
  priceSeriesIds: string[];
  gridConfiguration: GridConfiguration;
}

type PeriodType = {
  name: string;
  code: string;
};
type FulfilmentPeriod = {
  name: string;
  code: string;
  type: PeriodType;
};
type Unit = {
  name: string;
  code: string;
};
type Currency = {
  name: string;
  code: string;
};
type Location = {
  name: string;
};
type Commodity = {
  name: string;
};
type SeriesItemName = {
  lang: string;
  value: string;
};

type PriceDisplayCommodity = {
  name: string;
  id: string;
};

export type SeriesItemInputType = {
  id: string;
  assessmentMethod: string;
  dataUsed: string;
  price: number;
  priceHigh: number;
  adjustedPriceDelta: number;
  adjustedPriceHighDelta: number;
  adjustedPriceLowDelta: number;
  priceLow: number;
  referenceMarketName: string;
  premiumDiscount: number;
};

export const SeriesItemInput = [
  'id',
  'assessmentMethod',
  'dataUsed',
  'price',
  'priceHigh',
  'priceLow',
  'referenceMarketName',
  'adjustedPriceLowDelta',
  'adjustedPriceDelta',
  'adjustedPriceHighDelta',
  'premiumDiscount',
];

type ValidationErrors = {
  [key: string]: string[];
};

export interface IBaseSeriesItem {
  id: string;
  name: [SeriesItemName];
  seriesItemTypeCode: string;
  seriesPeriodSetName: string;
  commodity: Commodity;
  location: Location;
  currency: Currency;
  unit: Unit;
  price: number;
  status: string;
  priceLow: number;
  priceHigh: number;
  priceHighDelta: number;
  priceLowDelta: number;
  priceMidDelta: number;
  priceDelta: number;
  referencePrice: number;
  valueDisplayPrecission: number;
  period: string;
  previousVersion: PriceSeriesItemVersion;
  priceDeltaType: string;
}

export type PriceSeriesItemVersion = {
  price: number;
  priceHigh: number;
  priceLow: number;
  priceMid: number;
};

export interface IPriceEntrySeriesItem extends IBaseSeriesItem {
  readOnly: boolean;
  isDerivedPriceSeries: boolean;
  fulfilmentPeriod: FulfilmentPeriod;
  lastAssessmentDate: string;
  lastAssessmentPrice: string;
  lastAssessmentPremiumDiscount: string;
  premiumDiscount: number;
  validationErrors: ValidationErrors;
  assessedDateTime: string;
  gridConfiguration: GridConfiguration;
  publicationScheduleId?: string;
}

export interface IPriceDisplaySeriesItem extends IBaseSeriesItem {
  assessedDateTime: string;
  itemFrequencyName: string;
  lastModifiedDate: string;
}

type EditableWhen = {
  field: string;
  value: string;
};

type PriceDeltaConfig = {
  priceField?: string;
  priceDeltaField?: string;
  precisionField?: string;
};

type ColumnCustomConfig = {
  priceDelta?: PriceDeltaConfig;
};

export type AlternateField = {
  seriesItemTypeCodes: [string];
  field: string;
  priceDeltaField?: string;
};

export type Column = {
  field: string;
  headerName: string;
  cellDataType: string;
  displayOrder: number;
  columnOrder: number;
  editable: boolean;
  hideable: boolean;
  hidden: boolean;
  values: string[];
  pinned: string;
  type: string;
  tooltipField: string;
  editableWhen: EditableWhen;
  customConfig?: ColumnCustomConfig;
  cellType: string;
  minWidth?: number;
  maxWidth?: number;
  width?: number;
  alternateFields?: AlternateField[];
  autoSize: boolean;
};

export type UserPreferenceColumnInput = {
  field: string;
  displayOrder: number;
  hidden: boolean;
};

export type RowInput = {
  priceSeriesId: string;
  displayOrder: number;
  seriesItemTypeCode: string;
};

export type RowInputPriceDisplayTable = {
  priceSeriesId: string;
  displayOrder: number;
  seriesItemTypeCode: string;
};

export type GridConfiguration = {
  columns: [Column];
};

export type EventTime = {
  eventTime: Date;
};

export type Events = {
  events: EventTime[];
};

export type PriceEntryGridData = {
  gridConfiguration: GridConfiguration;
  seriesItemTypeCode: string;
  priceSeries: [IPriceEntrySeriesItem];
};

export type GridData = {
  priceEntryGridData: [PriceEntryGridData];
};

export type UpdateContentBlockResponse = {
  contentBlockId: string;
  version: number;
  isValid: boolean;
  errorCodes: [string];
};

type UpdateCommenatryResponse = {
  id: string;
};

type UpdateUserPreferenceResponse = {
  id: string;
};

export interface NotifierProps {
  name: string;
  value: any;
}

export type UpdateContentBlock = {
  response: UpdateContentBlockResponse;
};

export type UpdateUserPreference = {
  response: UpdateUserPreferenceResponse;
};

export type ContentBlockInError = {
  contentBlockId: string;
  error: string;
};

export type DataPackageTransitionToState = {
  isSuccess: boolean;
  errorCode: string;
};

export type dataPackageTransitionToStateResponse = {
  dataPackageTransitionToState: DataPackageTransitionToState;
};

export type UpdateCommentary = {
  response: UpdateCommenatryResponse;
};

export type ContentBlockInput = {
  contentBlockId: string;
  title: string;
  priceSeriesIds: [string];
};

export type SelectedFilterInput = {
  isInactiveIncluded: boolean;
  selectedAssessedFrequencies: string[];
  selectedCommodities: string[];
  selectedPriceCategories: string[];
  selectedRegions: string[];
  selectedTransactionTypes: string[];
};
export type ContentBlockForDisplayInput = {
  contentBlockId: string;
  title: string;
  columns: [UserPreferenceColumnInput];
  rows: [RowInput];
  selectedFilters: SelectedFilterInput;
};

export type Commentary = {
  id: string;
  contentBlockId: string;
  commentaryId: string;
  version: string;
};

export type NextAction = {
  name: string;
  enabled: boolean;
  displayValue: string;
};

export interface BaseContentBlockDefinition {
  contentBlockId: string;
  publicationScheduleId: string;
  dataPackageId: string;
  version: number;
  title?: string;
  gridConfiguration: GridConfiguration;
  priceSeries: [IBaseSeriesItem];
  nmaEnabled?: boolean;
}

export interface ContentBlockDefinition extends BaseContentBlockDefinition {
  seriesItemTypeCode?: string;
  priceSeriesIds: [string];
  priceSeriesGrids: IPriceSeriesGrid[];
  commentary?: Commentary;
  workflowBusinessKey?: string;
  nextActions: [NextAction];
}

export type ContentBlockResponse = {
  contentBlock: ContentBlockDefinition;
};

export interface IPriceDisplayContentBlockDefinition extends BaseContentBlockDefinition {
  columns?: [UserPreferenceColumnInput];
  rows: [RowInput];
  priceDisplayGridConfiguration: GridConfiguration;
  priceSeriesItemForDisplay: [IPriceDisplaySeriesItem];
  selectedFilters: SelectedFilterInput;
}

export type PriceDisplayContentBlockResponse = {
  contentBlockForDisplay: IPriceDisplayContentBlockDefinition;
};

export type IImpactedPriceDisplayContentBlockResponse = {
  contentBlockFromInputParametersForDisplay: IPriceDisplayContentBlockDefinition;
};

export interface IImpactedPrices {
  impactedPrices: {
    impactedDerivedPriceSeriesIds: [string];
    impactedReferencePriceSeriesIds: [string];
    impactedCalculatedPriceSeriesIds: [string];
  };
  isSuccess: boolean;
  errorCode: string;
}

export type ImpactedPricesResponse = {
  impactedPrices: IImpactedPrices;
};

export type PriceDisplaySeriesResponse = {
  priceSeriesForDisplayTool: PriceSeriesForDisplayTool;
};

export type CommoditiesForPriceDisplayTool = {
  commodities: [PriceDisplayCommodity];
};

export type PriceSeriesResponse = {
  priceSeries: [PriceSeries];
};

export type PriceSeriesDetails = {
  id: string;
  name: string;
  scheduleId?: string;
  priceSeriesValidationResult?: PriceSeriesValidationResult;
};

export type PriceSeries = {
  priceSeriesDetails: PriceSeriesDetails[];
  seriesItemTypeCode: string;
};

export interface IPriceSeriesDetailForDisplay extends PriceSeriesDetails {
  itemFrequencyId: string;
  regionId: string;
  priceSettlementTypeId: string;
  priceCategoryId: string;
  seriesItemTypeCode: string;
}

export type PriceSeriesItemForDisplayTool = {
  id: string;
  itemFrequencyId: string;
  regionId: string;
  priceSettlementTypeId: string;
  priceCategoryId: string;
  name: string;
  seriesItemTypeCode: string;
};

export type PriceSeriesForDisplayTool = {
  priceSeriesDetails: [IPriceSeriesDetailForDisplay] | IPriceSeriesDetailForDisplay[];
  assessedFrequencies: [PriceSeriesDetails];
  priceCategories: [PriceSeriesDetails];
  regions: [PriceSeriesDetails];
  transactionTypes: [PriceSeriesDetails];
};

export type UpdateUserPreferenceInput = {
  updatedColumnConfigs: UserPreferenceColumnInput[];
  selectedSeriesIDs?: string[];
  gridId?: string;
  refetch?: boolean;
};
export interface PriceSeriesValidationResult {
  isValid: boolean;
  message?: string;
};

