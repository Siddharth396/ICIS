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

export type SeriesItemInputType = {
  id: string;
  assessmentMethod: string;
  dataUsed: string;
  price: number;
  priceHigh: number;
  priceLow: number;
  referencePrice: number;
  premiumDiscount: number;
};

type MeasureUnit = {
  name: string;
  code: string;
};

type LaunchDate = {
  $date: string;
};

type ItemValueDisplayPrecision = {
  field_name: string;
  number_of_fractional_digits: number;
};

type ItemFrequency = {
  code: string;
};
export type SeriesItem = {
  _id: string;
  name: [SeriesItemName];
  series_item_type_code: string;
  series_period_set_name?: string | null;
  commodity?: Commodity;
  location?: Location;
  currency?: Currency;
  unit?: Unit;
  price?: number;
  period?: string;
  status?: string;
  priceLow?: number;
  priceHigh?: number;
  fulfilment_period?: FulfilmentPeriod;
  valueDisplayPrecision?: number;
  lastAssessmentDate?: string;
  lastAssessmentPrice?: string;
  priceDelta?: number;
  premiumDiscount?: number;
  referencePrice?: number;
  valueDisplayPrecission?: number;
  validationErrors?: ValidationErrors;
  measure_unit?: MeasureUnit;
  launch_date?: LaunchDate;
  termination_date?: string | null;
  item_value_display_precision: [ItemValueDisplayPrecision];
  item_frequency?: ItemFrequency;
};

type ValidationErrors = {
  [key: string]: string[];
};

type EditableWhen = {
  field: string;
  value: string;
};

export type Column = {
  field: string;
  header_name: string;
  cell_data_type: string;
  display_order: number;
  editable: boolean;
  values?: string[];
  pinned: string;
  type?: string;
  tooltipField?: string;
  editableWhen?: EditableWhen;
  customConfig?: ColumnCustomConfig;
  cellType?: string;
};

type PriceDeltaConfig = {
  priceField?: string;
  priceDeltaField?: string;
  precisionField?: string;
};

type ColumnCustomConfig = {
  priceDelta?: PriceDeltaConfig;
};

export type GridConfigurationType = {
  columns: [Column];
  series_item_type_code: string;
};

export type ContentBlockDefinitionType = {
  _id: string;
  content_block_id: string;
  version: number;
  title: string;
  series_item_type_code: string;
  price_series_ids: [string];
  displayColumns: [string];
  gridConfiguration: GridConfigurationType;
  priceSeries: [SeriesItem];
};

export const ContentBlockDefinition: ContentBlockDefinitionType[] = [
  {
    _id: "2f9bcfe9-0991-4bde-a871-0594ce4dde08",
    content_block_id: "aa564879-4898-4ead-8b9b-77afa30dbe95",
    version: 2,
    title: "LNG China",
    // @ts-ignore
    price_series_ids: [
      "LNG_China_HM1",
      "LNG_China_HM2",
      "LNG_China_HM3",
      "LNG_China_HM4",
      "LNG_China_HM5",
    ],
    series_item_type_code: "single_value_with_reference",
  },
];
