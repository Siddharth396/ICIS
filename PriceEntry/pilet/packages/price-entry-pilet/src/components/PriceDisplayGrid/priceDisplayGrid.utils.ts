// istanbul ignore file
import {
  CellClassParams,
  ValueGetterParams,
  RowClassParams,
  ValueFormatterParams,
} from 'ag-grid-community';
import { Column, PriceSeriesItemVersion } from 'apollo/queries';
import { GRID_CLASS_NAMES } from 'constants/gridClassName';
import { PUBLISH_WORKFLOW_STATUS, SERIES_ITEM_TYPE_CODE, WORKFLOW_ACTIONS } from 'utils/constants';

export const CELL_DATA_TYPES = {
  OBJECT: 'object',
  PRICE_DELTA: 'priceDelta',
  NUMBER: 'number',
} as const;

export const FIELDS = {
  STATUS: 'status',
  PRICE: 'price',
  PRICE_DELTA: 'priceDelta',
  PRICE_LOW: 'priceLow',
  PRICE_HIGH: 'priceHigh',
  PRICE_LOW_DELTA: 'priceLowDelta',
  PRICE_HIGH_DELTA: 'priceHighDelta',
  PRICE_MID: 'priceMid',
  PRICE_MID_DELTA: 'priceMidDelta',
  PRICE_DELTA_TYPE: 'priceDeltaType',
} as const;

export const PRICE_DELTA_FIELDS: string[] = [FIELDS.PRICE_DELTA, FIELDS.PRICE_HIGH_DELTA, FIELDS.PRICE_LOW_DELTA, FIELDS.PRICE_MID_DELTA];
export const PRICE_FIELDS: string[] = [FIELDS.PRICE,FIELDS.PRICE_LOW, FIELDS.PRICE_HIGH, FIELDS.PRICE_MID, ...PRICE_DELTA_FIELDS];

export const CELL_TYPES = {
  INPUT: 'input',
  SELECT: 'select',
} as const;

function getCellClassRules(columnObj: Column) {
  return {
    'ag-cell--right-aligned-cell': () => columnObj.type === 'rightAligned',
    'ag-cell--left-aligned-cell': () =>
      (!columnObj.type || columnObj.type === 'leftAligned') && columnObj.displayOrder !== 0,
    'ag-cell--fixed-error-cell': (params: CellClassParams) =>
      columnObj.displayOrder === 0 && params?.data?.validationErrors,
    'ag-cell--correction-published-cell': (params: CellClassParams) => {
      const field = params.colDef.field || '';
      const previousVersion: PriceSeriesItemVersion = params.data?.previousVersion

      if (!PRICE_FIELDS.includes(field) || !previousVersion || params.data.status !== WORKFLOW_ACTIONS.CORRECTION_PUBLISHED) return false;

      const dependentField = PRICE_DELTA_FIELDS.includes(field) ? field.replace('Delta', '') : field;

      if (dependentField === FIELDS.PRICE_MID) {
        const priceSeriesType = params.data?.seriesItemTypeCode;
        switch (priceSeriesType) {
          case SERIES_ITEM_TYPE_CODE.PI_RANGE:
            return previousVersion && ((previousVersion[FIELDS.PRICE_LOW] !== (params.data[FIELDS.PRICE_LOW])) || (previousVersion[FIELDS.PRICE_HIGH] !== (params.data[FIELDS.PRICE_HIGH])))
          default:
            return previousVersion && (previousVersion[FIELDS.PRICE] !== (params.data[FIELDS.PRICE]))
        }
      }
      return previousVersion && (previousVersion[dependentField as keyof PriceSeriesItemVersion] !== params.data[dependentField])
    }
  }
}

export function getPriceDisplayColumnFields(
  columnObj: Column,
  isAuthoring: boolean,
) {
  const {
    field,
    hidden,
    headerName,
    cellDataType,
    pinned,
    type,
    displayOrder,
    minWidth,
    width,
    maxWidth,
  } = columnObj;
  const isFirstColumn = displayOrder === 0;
  const isReferencePriceCell = field === 'referencePrice';

  return {
    field: field,
    headerName,
    cellDataType,
    type,
    hide: hidden,
    minWidth: minWidth ?? 100,
    width: width,
    maxWidth: maxWidth,
    headerClass: columnObj.type === 'rightAligned' ? 'ag-header-right' : '',
    lockPosition: isAuthoring ? false : true,
    ...(isFirstColumn && {
      minWidth: minWidth,
      autoHeight: true,
      rowDrag: isAuthoring ? true : false,
      lockPosition: true,
      pinned,
      cellStyle: {
        whiteSpace: 'normal',
        overflowWrap: 'break-word',
      },
    }),
    lockPinned: true,
    valueFormatter: (params: ValueFormatterParams) => {
      if (!params.value) {
        return '--';
      }
    },
    cellClass: (params: CellClassParams) => `cell-${params.rowIndex}-${params.colDef.field}`,
    cellClassRules: getCellClassRules(columnObj),
    ...(isReferencePriceCell && {
      valueGetter: (params: ValueGetterParams) =>
        params.data.referencePrice?.market
          ? params.data.referencePrice.market
          : params.data.referencePrice,
    }),
  };
}

export const getRowClass = (params: RowClassParams): string | undefined => {
  if (!params?.data) return undefined;
  if (params.data.status != null && !Object.keys(PUBLISH_WORKFLOW_STATUS).includes(params.data.status)) return GRID_CLASS_NAMES.ROW_PREPUBLISH;
  return undefined;
};