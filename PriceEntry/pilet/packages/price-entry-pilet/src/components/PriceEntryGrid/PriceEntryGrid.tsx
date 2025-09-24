// istanbul ignore file
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { AgGridReact, CustomCellRendererProps } from 'ag-grid-react';
import {
  BodyScrollEvent,
  CellStyleModule,
  CellValueChangedEvent,
  ClientSideRowModelModule,
  ColDef,
  ColumnApiModule,
  ColumnAutoSizeModule,
  ColumnMovedEvent,
  CustomEditorModule,
  GetRowIdParams,
  GridApi,
  GridOptions,
  GridReadyEvent,
  GridStateModule,
  ModuleRegistry,
  NumberEditorModule,
  RowApiModule,
  RowAutoHeightModule,
  RowDragEvent,
  RowDragModule,
  RowSelectionModule,
  RowStyleModule,
  SuppressKeyboardEventParams,
  TextEditorModule,
  TooltipModule,
  ValidationModule,
} from 'ag-grid-community';

import {
  Column,
  IBaseSeriesItem,
  IPriceSeriesGrid,
  SeriesItemInputType,
  UpdateUserPreferenceInput,
  UserPreferenceColumnInput,
} from 'apollo/queries';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { themeParams } from './agGridTheme';
import { cellStyle, PriceEntryGridContainer } from './PriceEntryGrid.styled';
import NumberCellEditor from 'components/NumberCellEditor';
import NumberCellRenderer from 'components/NumberCellEditor/NumberCellRenderer';
import PriceDeltaComponent from 'components/PriceDelta/PriceDelta';
import PriceSeriesItemStatusRenderer from 'components/PriceSeriesItemStatus';
import CustomHeader from 'components/Header/Header';
import SelectCellEditor from '../SelectCellEditor';
import SelectCellRenderer from '../SelectCellEditor/SelectCellRenderer';
import Tooltip from 'components/Tooltip';
import {
  CELL_DATA_TYPES,
  CELL_TYPES,
  FIELDS,
  getPriceEntryColumnFields,
  NMA_FIELDS,
  priceItemInputMapper,
} from './priceEntryGrid.utils';
import CorrectionsImpact from 'components/CorrectionsImpact';
import AdjustedPriceDeltaEditor from 'components/AdjustedPriceDelta/AdjustedPriceDeltaEditor';
import AdjustedPriceDeltaRenderer from 'components/AdjustedPriceDelta/AdjustedPriceDeltaRenderer';

ModuleRegistry.registerModules([
  RowSelectionModule,
  ClientSideRowModelModule,
  ColumnAutoSizeModule,
  CustomEditorModule,
  CellStyleModule,
  TooltipModule,
  RowAutoHeightModule,
  TextEditorModule,
  RowApiModule,
  RowStyleModule,
  ColumnApiModule,
  RowDragModule,
  ValidationModule,
  NumberEditorModule,
  GridStateModule,
]);

// Matches the interface from the original but for "PriceEntry" only.
interface IPriceEntryGridProps {
  /** The parent content block data which includes gridConfiguration & priceSeries */
  data: IPriceSeriesGrid;
  /** Current Grid API reference */
  gridApi: GridApi | null;
  /** Whether content is locked (non-editable) */
  isContentLocked?: boolean;
  /** Whether NMA is enabled */
  nmaEnabled: boolean;
  /** The selected date for the grid */
  selectedDate: Date;
  /** Setter for the gridApi */
  setGridApi: (api: GridApi) => void;
  /** Callback for price series updates */
  onUpdatePriceSeriesData?: (payload: SeriesItemInputType, seriesItemTypeCode: string) => void;
  /** Callback for user preference updates (column reorder, row reorder) */
  onUpdateUserPreference?: (params: UpdateUserPreferenceInput) => void;
  /** Called when a user starts editing (focus gained) */
  onStartEditing?: () => void;
  /** Called when a user finishes editing (focus lost) */
  onFinishEditing?: () => void;
}

const containerStyle = { width: '100%', height: '100%' };
const gridStyle = { height: '100%', width: '100%' };

const PriceEntryGrid: React.FC<IPriceEntryGridProps> = ({
  data,
  nmaEnabled,
  gridApi,
  isContentLocked,
  selectedDate,
  setGridApi,
  onUpdatePriceSeriesData,
  onUpdateUserPreference,
  onStartEditing,
  onFinishEditing,
}) => {
  const messages = useLocaleMessages();
  const gridRef = useRef<AgGridReact<IBaseSeriesItem>>(null);
  const [isAtLeftEdge, setIsAtLeftEdge] = useState(true);

  const columnDefs = useMemo(() => {
    const columnConfig = data?.gridConfiguration?.columns ?? [];
    const isNmaEnabled = nmaEnabled;

    // Sort columns by displayOrder
    const sortedCols = [...columnConfig].sort((a, b) => a.displayOrder - b.displayOrder);

    // Filter out NMA columns when NMA is not enabled
    const filteredCols = sortedCols.filter((config: Column) => {
      return !(NMA_FIELDS.includes(config.field) && !isNmaEnabled);
    });

    return filteredCols.map((config: Column) => {
      const { field, cellDataType, cellType, editableWhen, editable, type, values } = config;
      const isNMAField = NMA_FIELDS.includes(field);
      const baseCol = getPriceEntryColumnFields(config, isContentLocked, isNMAField);

      return {
        ...baseCol,
        headerComponent: CustomHeader,
        ...(cellDataType === CELL_DATA_TYPES.PRICE_DELTA && {
          cellRenderer: (params: CustomCellRendererProps) => (
            <PriceDeltaComponent data={params.data} columnConfig={config} isPriceEntry={true} />
          ),
          cellDataType: false,
        }),
        // If the column is status
        ...(field === FIELDS.LINKED_PRICES && {
          cellRenderer: (params: CustomCellRendererProps) => (
            <CorrectionsImpact data={params.data} snapshotDate={selectedDate} />
          ),
          cellDataType: false,
        }),
        // If the column is status
        ...(field === FIELDS.STATUS && {
          cellRenderer: PriceSeriesItemStatusRenderer,
        }),

        // 1. For "input" type columns that are not NMA fields
        ...(cellType === CELL_TYPES.INPUT &&
          !isNMAField && {
            cellEditor: NumberCellEditor,
            cellRenderer: NumberCellRenderer,
            cellDataType: false,
            cellEditorParams: {
              editableWhen,
              editable,
              type,
            },
          }),

        // 2. For adjusted price delta columns when NMA is enabled and this is an NMA field
        ...(isNmaEnabled &&
          isNMAField &&
          cellType === CELL_TYPES.INPUT && {
            cellEditor: AdjustedPriceDeltaEditor,
            cellRenderer: (props: CustomCellRendererProps) => (
              <AdjustedPriceDeltaRenderer columnConfig={config} {...(props as any)} />
            ),
            cellDataType: false,
            minWidth: 200,
            cellEditorParams: {
              editableWhen,
              editable,
              type,
              columnConfig: config,
            },
          }),

        // 3. If NMA is enabled, is an NMA field, and cellType is not defined or null => use only the renderer
        ...(isNmaEnabled &&
          isNMAField &&
          !editable && {
            cellRenderer: (props: CustomCellRendererProps) => (
              <AdjustedPriceDeltaRenderer columnConfig={config} {...(props as any)} />
            ),
            cellDataType: false,
            minWidth: 200,
          }),

        // 4. For "select" type columns
        ...(cellType === CELL_TYPES.SELECT && {
          cellEditor: SelectCellEditor,
          cellRenderer: SelectCellRenderer,
          cellDataType: false,
          cellEditorParams: {
            options: values,
          },
          suppressKeyboardEvent: (params: SuppressKeyboardEventParams) => {
            const KEY_ENTER = 13;
            const { event, editing } = params;
            return editing && event.which === KEY_ENTER;
          },
        }),
      };
    });
  }, [data, isContentLocked]);

  /** The row data to render */
  const rowData = useMemo(() => data?.priceSeries ?? [], [data]);

  const [gridState, setGridState] = useState({
    rowData,
    columnDefs: columnDefs as unknown as ColDef<any, any>[],
  });

  /** Default column props for Price Entry */
  const defaultColDef = useMemo(
    () => ({
      flex: 1,
      resizable: false,
      sortable: false,
      cellStyle,
      wrapHeaderText: true,
      autoHeaderHeight: true,
      tooltipComponent: Tooltip,
    }),
    [],
  );

  /** Called on cell value change. We map updated data to the shape required. */
  const handleCellValueChanged = useCallback(
    (params: CellValueChangedEvent) => {
      if (params.oldValue === params.newValue) return;
      if (!params.data) return;

      const { gridConfiguration, seriesItemTypeCode } = data || {};
      const { columns = [] } = gridConfiguration || {};
      const { referencePrice, ...rest } = params.data;

      // Create final payload
      const payload = {
        ...rest,
        referenceMarketName:
          typeof referencePrice === 'object' ? referencePrice?.market : referencePrice,
      };

      onUpdatePriceSeriesData?.(
        priceItemInputMapper(payload, columns),
        seriesItemTypeCode as string,
      );
    },
    [data, onUpdatePriceSeriesData],
  );

  useEffect(() => {
    if (gridApi) {
      setGridState({ rowData, columnDefs: columnDefs as unknown as ColDef<any, any>[] });
    }
  }, [gridApi, rowData, columnDefs]);

  /** Return a row's unique ID (since we do row reordering). */
  const getRowId = useCallback((params: GetRowIdParams) => {
    return params?.data?.id;
  }, []);

  /** Called when user reorders columns. */
  const handleColumnMoved = useCallback(
    (event: ColumnMovedEvent) => {
      if (!event.finished || event.source !== 'uiColumnMoved') return;

      const updatedFields = event.api.getAllGridColumns().map((col) => col.getColId());
      const updatedColumnConfigs: UserPreferenceColumnInput[] = updatedFields.map((field, i) => {
        const orig = data?.gridConfiguration?.columns.find((c) => c.field === field);
        return {
          field,
          displayOrder: i,
          hidden: orig ? orig.hidden : false,
        };
      });
      onUpdateUserPreference?.({
        updatedColumnConfigs,
        refetch: true,
      });
    },
    [data, onUpdateUserPreference],
  );

  /** Called when user row-drags (if rowDragManaged = true) */
  const handleRowDragEnd = useCallback(
    (event: RowDragEvent) => {
      const newSeriesOrder: string[] = [];
      event.api.forEachNode((node) => newSeriesOrder.push(node.data.id));
      onUpdateUserPreference?.({
        updatedColumnConfigs: [],
        selectedSeriesIDs: newSeriesOrder,
        refetch: true,
      });
    },
    [onUpdateUserPreference],
  );

  /** Lock/unlock content block on cell focus. */
  const handleCellEditingStarted = useCallback(() => {
    onStartEditing?.();
  }, [onStartEditing]);

  const handleCellEditingStopped = useCallback(() => {
    onFinishEditing?.();
  }, [onFinishEditing]);

  const handleBodyScroll = (event: BodyScrollEvent) => {
    setIsAtLeftEdge(event.left === 0);
  };

  /** For column sizing, no-rows overlay, etc. */
  const onGridReady = useCallback(
    (params: GridReadyEvent) => {
      setGridApi(params.api);
      params.api.sizeColumnsToFit();
      // After initial fit, auto-size flagged columns so long values (e.g., last assessment price) are fully visible
      const allCols = params.api.getColumns?.() || [];
      const autoSizeCols = allCols
        .filter((c) => !!(c.getColDef() as any).autoSize)
        .map((c) => c.getId());
      if (autoSizeCols.length && params.api.autoSizeColumns) {
        params.api.autoSizeColumns(autoSizeCols, false);
      }
    },
    [setGridApi],
  );

  /** Build AG GridOptions specifically for Price Entry usage */
  const gridOptions = useMemo<GridOptions<IBaseSeriesItem>>(
    () => ({
      theme: themeParams,
      singleClickEdit: true,
      stopEditingWhenCellsLoseFocus: true,
      overlayNoRowsTemplate: messages.General.NoRecordsFound,
      tooltipShowDelay: 0,
      rowDragManaged: true,
      onGridReady,
      onBodyScroll: handleBodyScroll,
      onColumnMoved: handleColumnMoved,
      onRowDragEnd: handleRowDragEnd,
      onCellValueChanged: handleCellValueChanged,
      onCellEditingStarted: handleCellEditingStarted,
      onCellEditingStopped: handleCellEditingStopped,
    }),
    [
      messages,
      handleColumnMoved,
      handleRowDragEnd,
      handleCellValueChanged,
      handleCellEditingStarted,
      handleCellEditingStopped,
      onGridReady,
    ],
  );

  return (
    <div style={containerStyle}>
      <PriceEntryGridContainer
        style={gridStyle}
        data-testid='price-entry-grid-container'
        isScrollAtLeftMostPosition={isAtLeftEdge}>
        <AgGridReact<IBaseSeriesItem>
          {...gridOptions}
          ref={gridRef}
          suppressDragLeaveHidesColumns
          rowData={gridState.rowData}
          columnDefs={gridState.columnDefs}
          defaultColDef={defaultColDef}
          getRowId={getRowId}
          domLayout='autoHeight'
        />
      </PriceEntryGridContainer>
    </div>
  );
};

export default PriceEntryGrid;
