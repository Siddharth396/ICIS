// istanbul ignore file
import { SeriesItemInput, SeriesItemInputType, Column, IBaseSeriesItem } from 'apollo/queries';
import { CellValueChangedEvent, ValueGetterParams } from 'ag-grid-community';
import { getStatus } from 'components/PriceSeriesItemStatus/PriceSeriesItemStatusRenderer';

/**
 * Common string constants used by the Price Entry grid
 * (You can also move these to a separate constants file if you want them shared.)
 */
export const CELL_DATA_TYPES = {
  OBJECT: 'object',
  PRICE_DELTA: 'priceDelta',
  NUMBER: 'number',
  ADJUSTED_PRICE_DELTA: 'adjustedPriceDelta',
} as const;

export const NMA_FIELDS = [
  'adjustedPriceHighDelta',
  'adjustedPriceLowDelta',
  'adjustedPriceDelta',
  'adjustedPriceMidDelta',
];

export const FIELDS = {
  STATUS: 'status',
  LINKED_PRICES: 'linkedPricesButton',
  PREMIUM_DISCOUNT: 'premiumDiscount',
  ASSESSMENT_METHOD: 'assessmentMethod',
  LAST_ASSESSMENT_PRICE: 'lastAssessmentPrice',
} as const;

export const CELL_TYPES = {
  INPUT: 'input',
  SELECT: 'select',
} as const;

export const PRICE_DECIMAL_PRECISION: number = 5;

/**
 * Return an object of cell style rules for Price Entry columns.
 */
function getCellClassRules(columnObj: Column) {
  return {
    'ag-cell--disabled-background': (params: CellValueChangedEvent) => {
      if (!params?.data) return false;
      // If readOnly or certain conditions based on editableWhen
      // You can expand logic as needed
      return (
        params.data[columnObj.editableWhen?.field] &&
        columnObj?.editableWhen &&
        params.data[columnObj.editableWhen?.field] !== columnObj.editableWhen?.value
      );
    },
    'ag-cell--right-aligned-cell': () => columnObj.type === 'rightAligned',
    'ag-cell--left-aligned-cell': () =>
      (!columnObj.type || columnObj.type === 'leftAligned') && columnObj.displayOrder !== 0,
    'ag-cell--fixed-error-cell': (params: CellValueChangedEvent) =>
      columnObj.displayOrder === 0 && params.data?.validationErrors,
  };
}

/**
 * Build a column definition object specific to Price Entry.
 */
export function getPriceEntryColumnFields(
  columnObj: Column,
  isContentLocked?: boolean,
  isNMAField?: boolean,
) {
  const {
    editableWhen,
    field,
    editable,
    hidden,
    headerName,
    cellDataType,
    pinned,
    type,
    cellType,
    tooltipField,
    displayOrder,
  } = columnObj;

  const isFirstColumn = displayOrder === 0;
  const isInputCell = cellType === CELL_TYPES.INPUT;
  const isReferencePriceCell = field === 'referencePrice';
  const isChangeColumn = /Delta$/i.test(field);
  const isLastAssessmentPrice = field === FIELDS.LAST_ASSESSMENT_PRICE;
  const shouldAutoSize = (isChangeColumn || isLastAssessmentPrice) && !editable;
  return {
    field,
    headerName,
    hide: hidden,
    pinned,
    minWidth: isFirstColumn ? 260 : 130,
    lockPinned: true,
    headerTooltip: headerName,
    cellDataType,
    type,
    ...(shouldAutoSize && {
      autoSize: true,
      suppressSizeToFit: true,
      flex: undefined as any,
    }),

    // If content is locked or row is readOnly, disable editing
    editable: (params: any) => {
      if (!params.data || isContentLocked) return false;
      if (params.data.readOnly) return false;
      // If the row is a derived price series, disable editing unless NMA is enabled.
      if (params.data.isDerivedPriceSeries && !isNMAField) return false;
      if (editableWhen && params.data[editableWhen.field] === editableWhen.value) {
        return true;
      }
      return editable;
    },

    // For tooltip display
    ...((tooltipField || field === 'status') && {
      tooltipValueGetter: (params: CellValueChangedEvent) => {
        const validationMessage = params?.data?.validationErrors
          ? // @ts-ignore
            params.data?.validationErrors[params.column.colId]
          : null;

        if (field === 'status' && params.data?.status) {
          return getStatus(params.data?.status);
        }

        return params.value && !isInputCell
          ? isReferencePriceCell
            ? validationMessage
            : params.value
          : validationMessage;
      },
    }),
    // Add special styling rules
    cellClassRules: getCellClassRules(columnObj),

    // If pinned first column, allow rowDrag if needed
    ...(isFirstColumn && {
      rowDrag: true,
      autoHeight: true,
      cellStyle: {
        whiteSpace: 'normal',
        overflowWrap: 'break-word',
      },
    }),

    // Example: if the column is "referencePrice" but is an object
    // handle a custom valueGetter to display only market, etc.
    ...(field === 'referencePrice' && {
      valueGetter: (params: ValueGetterParams) =>
        params.data.referencePrice?.market
          ? params.data.referencePrice.market
          : params.data.referencePrice,
    }),
  };
}

/**
 * Convert row data from the Price Entry grid into the shape needed
 * for saving to your backend (e.g. calling an API).
 *
 * @param gridData The row data from AG Grid (partial)
 * @param columnConfig All columns from the content block config
 */
export const priceItemInputMapper = (gridData: IBaseSeriesItem, columnConfig: Column[] = []) => {
  const priceItemInput: Record<string, any> = {};
  const fields = Array.isArray(SeriesItemInput) ? SeriesItemInput : Object.keys(SeriesItemInput);
  for (const field of fields) {
    if (gridData && Object.prototype.hasOwnProperty.call(gridData, field)) {
      const fieldObj = columnConfig.find((config) => config.field === field);
      const value = gridData[field as keyof IBaseSeriesItem];
      if (fieldObj && fieldObj.cellDataType === 'number') {
        priceItemInput[field] =
          value === undefined || value === null || value === '' ? null : Number(value);
      } else {
        priceItemInput[field] = value;
      }
    }
  }
  return priceItemInput as SeriesItemInputType;
};

/**
 * Get placeholder value for a specific field
 */
export const placeHolderValue = (data: any, field?: string) => {
  switch (field) {
    case FIELDS.PREMIUM_DISCOUNT: {
      return data?.lastAssessmentPremiumDiscount;
    }
    default: {
      return undefined;
    }
  }
};
