import camelcaseKeys from "camelcase-keys";
import { priceSeries } from "../__mocks__/priceSeries";
import { priceEntryTableConfig } from "../__mocks__/gridConfig";
import delayResponse from "../utils/delayResponse";
import validateEntitlements from "../utils/validateEntitlements";
import {
  ContentBlockDefinition,
  ContentBlockDefinitionType,
} from "../__mocks__/contentBlockDefinition";
import { GridConfiguration } from "../__mocks__/gridConfiguration";

export const definitions = `
  type BasePriceItem {
    id: String!
    seriesId: String!
    appliesFrom: DateTime!
    creationDateTime: DateTime!
  }

  type Column {
    field: String!
    headerName: String!
    cellDataType: String!
    displayOrder: Int!
    editable: Boolean!
    values: [String!]
    pinned: String
    editableWhen: ColumnEditableWhen
    type: String
    customConfig: CustomConfig
    tooltipField: String
    cellType: String
  }
  

  type ColumnEditableWhen {
    field: String!
    value: String!
  }

  type Commodity {
    name: String!
  }

  type Currency {
    name: String!
    code: String!
  }

  type CustomConfig {
    priceDelta: PriceDelta
  }

  type FulfilmentPeriod {
    name: String!
    code: String!
    periodType: PeriodType!
  }

  type GridConfiguration {
    columns: [Column!]!
    sort: Sort!
    seriesItemTypeCode: String!
  }

  type ItemValueDisplayPrecision {
    fieldName: String!
    numberOfFractionalDigits: Int!
  }

  type Location {
    name: String!
  }

  type MeasureUnit {
    name: String!
    code: String!
  }

  type Name {
    lang: String!
    value: String!
  }

  type PeriodType {
    name: String!
    code: String!
  }

  type PriceDelta {
    priceField: String!
    priceDeltaField: String!
    precisionField: String!
  }

  type PriceEntry {
    priceSeries: [PriceSeries!]!
    gridConfiguration: GridConfiguration!
    seriesItemTypeCode: String!
  }

  type PriceSeries {
    id: String!
    name: [Name!]!
    seriesPeriodSetName: String
    seriesItemTypeCode: String!
    commodity: Commodity!
    location: Location!
    currency: Currency!
    measureUnit: MeasureUnit!
    launchDate: DateTime!
    terminationDate: Date
    fulfilmentPeriod: FulfilmentPeriod
    itemValueDisplayPrecision: [ItemValueDisplayPrecision!]!
    priceSeriesName: String!
    unitDisplay: String!
    seriesItemId: String!
    seriesId: String!
    creationDatetime: DateTime
    appliesFromDatetime: DateTime
    dataUsed: String
    price: Decimal
    priceLow: Decimal
    priceHigh: Decimal
    priceMid: Decimal
    lastAssessmentDate: String
    lastAssessmentPrice: String
    priceDelta: Decimal
    valueDisplayPrecision: Int
    assessmentMethod: String
    referencePrice: Decimal
    premiumDiscount: Decimal
    priceLowDelta: Decimal
    priceMidDelta: Decimal
    priceHighDelta: Decimal
    status: String
    period: String
    validationErrors: [String]
  }

  type PriceSeriesSelectionItem {
    id: String!
    name: String!
    seriesItemTypeCode: String!
  }

  type CommentaryOutput {
    commentaryId: String
    version: String
  }

  type ContentBlockDefinition {
    contentBlockId: String!
    version: Int
    title: String
    seriesItemTypeCode: String
    priceSeriesIds: [String!]
    displayColumns: [String!]
    gridConfiguration: GridConfiguration
    priceSeries(appliesFrom: Long): [PriceSeries!]
    commentary(appliesFrom: Long!): CommentaryOutput
  }
  
  type Sort {
    name: String!
    order: String!
  }

  type ContentBlockSaveResponse {
    contentBlockId: String!
    version: Int!
    errorCodes: [String!]
  }

  type Commentary {
    id: String!
    contentBlockId: String!
    commentaryId: String!
    version: String!
    appliesFrom: DateTime!
  }

  input PriceItemInput {
    seriesId: String!
    seriesItemTypeCode: String!
    appliesFrom: Long!
    seriesItem: SeriesItemInput!
  }

  input SeriesItemInput {
    assessmentMethod: String
    dataUsed: String
    price: Decimal
    priceDeltaPercentage: Decimal
    priceHigh: Decimal
    priceLow: Decimal
    referencePrice: Decimal
    premiumDiscount: Decimal
  }

  input ContentBlockInput {
    contentBlockId: String!
    title: String
    priceSeriesIds: [String!]
    displayColumns: [String!]
  }

  input CommentaryInput {
    contentBlockId: String!
    commentaryId: String!
    version: String!
    appliesFrom: Long!
  }
  
`;

function configsResolver(seriesItemTypeCode: string | undefined) {
  const config = GridConfiguration.find((c) => c.series_item_type_code === seriesItemTypeCode);
  return config;
}

const camelCaseResolver =
  (resolver: (...args: any[]) => Promise<any>) =>
  async (...args: any[]) => {
    const result = await resolver(...args);
    return camelcaseKeys(result, { deep: true });
  };

const contentBlockResolver = camelCaseResolver(async (args: any) => {
  const { contentBlockId, version } = args;
  const contentBlock: ContentBlockDefinitionType | undefined = ContentBlockDefinition.find(
    (c: ContentBlockDefinitionType) => c.content_block_id === contentBlockId
  );
  return {
    ...contentBlock,
    contentBlockId: contentBlockId,
    version: version,
    gridConfiguration: configsResolver(
      contentBlock ? contentBlock.series_item_type_code : undefined
    ),
    priceSeries: contentBlock
      ? contentBlock.price_series_ids.map((id) => {
          const series = priceSeries.find((p) => p._id === id);
          if (series && series._id) {
            return {
              ...series,
              id,
              priceSeriesName: series.name.filter((p) => p.lang == "en").map((n) => n.value)[0],
              unitDisplay:
                series.currency && series.measure_unit
                  ? `${series.currency.code}/${series.measure_unit.code}`
                  : "",
            };
          }
        })
      : [],
  };
});

// function priceEntryGridDataResolver(args: any) {
//   const { commodity } = args;
//   const seriesData = priceSeriesResolver(commodity);
//   const seriesItemTypeCode = seriesData[0].seriesItemTypeCode;
//   const gridConfiguration = configsResolver(seriesItemTypeCode);
//   if (seriesData && gridConfiguration) {
//     return [
//       {
//         priceSeries: [...seriesData],
//         gridConfiguration,
//         seriesItemTypeCode,
//       },
//     ];
//   }
//   return {};
// }

export const updatePriceEntryGridDataResolver = (obj: any, args: any, context: any, info: any) => {
  // const { seriesId, appliesFrom, seriesItem } = args;
  // const seriesItemIndex = priceSeries.findIndex((p) => p.id === seriesId);
  // priceSeries[seriesItemIndex] = {
  //   ...priceSeries[seriesItemIndex],
  //   ...seriesItem,
  //   appliesFrom,
  // };
  // return priceSeries.find((p) => p.id === seriesId);
};

export const query = `
  priceSeries(commodityName: String!): [PriceSeriesSelectionItem!]!
  contentBlock(contentBlockId: String!, version: Int): ContentBlockDefinition
`;

export const mutation = `
  updatePriceEntryGridData(priceItemInput: PriceItemInput!): BasePriceItem!
  saveContentBlock(
    contentBlockInput: ContentBlockInput!
  ): ContentBlockSaveResponse!
  saveCommentary(commentaryInput: CommentaryInput!): Commentary!

`;

export const resolvers = {
  queries: {
    contentBlock: (_: any, args: any) =>
      delayResponse(validateEntitlements(args, contentBlockResolver(args)), 1500),
  },
  mutations: {
    updatePriceEntryGridData: (_: any, args: any) =>
      delayResponse(updatePriceEntryGridDataResolver(undefined, args, undefined, undefined), 500),
  },
};
