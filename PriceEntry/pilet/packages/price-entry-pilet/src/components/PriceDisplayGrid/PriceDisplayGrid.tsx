// istanbul ignore file
import React, { useCallback, useMemo, useRef, useState } from 'react';
import { AgGridReact } from 'ag-grid-react';
import {
  BodyScrollEvent,
  ColumnMovedEvent,
  GridApi,
  GridReadyEvent,
  GridSizeChangedEvent,
  GridStateModule,
  RowDragEvent,
  RowSelectionModule,
  themeAlpine,
  TooltipModule,
  ClientSideRowModelModule,
  ColumnAutoSizeModule,
  CustomEditorModule,
  CellStyleModule,
  RowAutoHeightModule,
  TextEditorModule,
  RowApiModule,
  RowStyleModule,
  ColumnApiModule,
  RowDragModule,
  ValidationModule,
  NumberEditorModule,
  ColDef,
  ModuleRegistry,
  CellValueChangedEvent,
} from 'ag-grid-community';
import {
  BaseContentBlockDefinition,
  Column,
  IBaseSeriesItem,
  UserPreferenceColumnInput,
} from 'apollo/queries';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import PriceDeltaComponent from 'components/PriceDelta/PriceDelta';
import PriceSeriesItemStatusRenderer from 'components/PriceSeriesItemStatus';
import CustomHeader from 'components/Header/Header';
import Tooltip from 'components/Tooltip';
import {
  CELL_DATA_TYPES,
  FIELDS,
  getRowClass,
  getPriceDisplayColumnFields,
} from './priceDisplayGrid.utils';
import { cellStyle, PriceDisplayGridContainer } from './PriceDisplayGrid.styled';
import PriceComponent from '../Price/Price';
import AdjustedDeltaDisplayRenderer from 'components/AdjustedDeltaDisplayRenderer';
import { PRICE_DELTA_TYPES } from 'utils/constants';

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

interface IPriceDisplayGridProps {
  data: BaseContentBlockDefinition;
  gridApi: GridApi | null;
  isAuthoring?: boolean;
  setGridApi: (api: GridApi) => void;
  onStartEditing?: () => void;
  onFinishEditing?: () => void;
  onUpdateUserPreference?: (
    updatedColumnConfigs: UserPreferenceColumnInput[],
    selectedSeriesIDs?: string[],
    refetch?: boolean,
  ) => void;
}

const containerStyle = { width: '100%', height: '100%' };
const gridStyle = { height: '100%', width: '100%' };

const PriceDisplayGrid: React.FC<IPriceDisplayGridProps> = ({
  data,
  isAuthoring = false,
  setGridApi,
  onUpdateUserPreference,
}) => {
  const messages = useLocaleMessages();
  const gridRef = useRef<AgGridReact<IBaseSeriesItem>>(null);

  const [isAtLeftEdge, setIsAtLeftEdge] = useState(true);
  const [scrollWidth, setScrollWidth] = useState(0);

  const columnDefs = useMemo(() => {
    const columnConfig = data?.gridConfiguration?.columns ?? [];
    const sortedCols = [...columnConfig].sort((a, b) => a.displayOrder - b.displayOrder);
    const isNmaEnabled = data?.priceSeries.some((series) => series.priceDeltaType === PRICE_DELTA_TYPES.NON_MARKET_ADJUSTMENT);

    const filteredCols = sortedCols.filter((config: Column) => {
      return !(FIELDS.PRICE_DELTA_TYPE === config.field && !isNmaEnabled);
    });

    return filteredCols?.map?.((config: Column) => {
      const { field, cellDataType } = config;

      const priceFieldWithDeltaColumn = sortedCols.find(
        (column) =>
          column.cellDataType === CELL_DATA_TYPES.PRICE_DELTA &&
          field === column.customConfig?.priceDelta?.priceField,
      );

      return {
        ...getPriceDisplayColumnFields(config, isAuthoring ?? false),
        headerComponent: CustomHeader,
        ...(cellDataType === CELL_DATA_TYPES.PRICE_DELTA && {
          cellRenderer: (params: CellValueChangedEvent) => (
            <PriceDeltaComponent data={params.data} columnConfig={config} isPriceEntry={false} />
          ),
          cellDataType: false,
        }),
        ...(priceFieldWithDeltaColumn && {
          cellRenderer: (params: CellValueChangedEvent) => (
            <PriceComponent rowData={params.data} priceDeltaColumn={priceFieldWithDeltaColumn} />
          ),
          cellDataType: false,
        }),
        ...(field === FIELDS.STATUS && {
          cellRenderer: PriceSeriesItemStatusRenderer,
        }),
        ...(field === FIELDS.PRICE_DELTA_TYPE && {
          cellRenderer: AdjustedDeltaDisplayRenderer,
        }),
      };
    });
  }, [data?.gridConfiguration?.columns, isAuthoring]);

  // Build row data
  const rowData = useMemo(() => data?.priceSeries ?? [], [data]);

  const defaultColDef = useMemo(
    () => ({
      flex: 1,
      sortable: false,
      resizable: false,
      cellStyle: { ...cellStyle },
      wrapHeaderText: true,
      autoHeaderHeight: true,
      tooltipComponent: Tooltip,
    }),
    [],
  );

  // Event handlers
  const handleColumnMoved = useCallback(
    (event: ColumnMovedEvent) => {
      if (event.finished && event.source === 'uiColumnMoved') {
        const updatedColumns = event.api.getAllGridColumns().map((column) => column.getColId());
        const updatedColumnConfigs: UserPreferenceColumnInput[] = updatedColumns.map(
          (column, index) => {
            const correspondingColumn = data?.gridConfiguration?.columns.find((col) => {
              const alternateField = col.alternateFields?.find((af) => af.field === column);
              return col.field === column || alternateField?.field === column;
            });
            return {
              field: correspondingColumn!.field,
              displayOrder: index,
              hidden: correspondingColumn ? correspondingColumn.hidden : false,
            };
          },
        );
        if (onUpdateUserPreference) {
          onUpdateUserPreference(updatedColumnConfigs, undefined, true);
        }
      }
    },
    [data, onUpdateUserPreference],
  );

  const handleRowDragEnd = useCallback(
    (event: RowDragEvent) => {
      const newIds: string[] = [];
      event.api.forEachNode((node) => newIds.push(node.data.id));
      onUpdateUserPreference?.([], newIds, true);
    },
    [onUpdateUserPreference],
  );

  const handleGridReady = useCallback(
    (params: GridReadyEvent) => {
      setGridApi(params.api);
      params.api.sizeColumnsToFit();
    },
    [setGridApi],
  );

  const handleBodyScroll = useCallback((event: BodyScrollEvent<any>) => {
    setIsAtLeftEdge(event.left === 0);
  }, []);

  const getTotalColumnsWidth = (api: GridApi<any>) => {
    return api
      .getColumnState()
      .reduce((total, column) => (column.hide ? total : total + (column.width ?? 0)), 0);
  };

  const handleStateUpdated = useCallback((event: any) => {
    if (
      event.sources.some(
        (source: string) =>
          source === 'columnVisibility' ||
          source === 'gridInitializing' ||
          source === 'columnSizing',
      )
    ) {
      const totalColumnsWidth = getTotalColumnsWidth(event.api);
      setScrollWidth(totalColumnsWidth);
    }
  }, []);

  const getColIds = useMemo(() => {
    return data.gridConfiguration.columns
      .filter((column) => column.autoSize)
      .flatMap((column) => {
        const alternateFields = column.alternateFields?.flatMap((af) => af.field);
        return alternateFields && alternateFields.length > 0 ? alternateFields : [column.field];
      });
  }, [data]);

  const resizeColumns = useCallback(
    (api: GridApi<any>) => {
      api?.autoSizeColumns(getColIds, true);
      const totalColumnsWidth = getTotalColumnsWidth(api);
      const gridWidth = document.querySelector('.ag-body-viewport')!.clientWidth;
      if (totalColumnsWidth < gridWidth) {
        const lastColumn = api
          .getColumnState()
          .find((_, index, array) => index === array.length - 1)!;
        api?.setColumnWidths([
          {
            key: lastColumn!.colId,
            newWidth: gridWidth - totalColumnsWidth + lastColumn.width!,
          },
        ]);
        api?.sizeColumnsToFit();
      }
    },
    [getColIds],
  );

  const handleGridSizeChanged = useCallback(
    (event: GridSizeChangedEvent<any>) => {
      resizeColumns(event.api);
    },
    [resizeColumns],
  );

  const handleGridColumnsChanged = useCallback(
    (event: any) => {
      resizeColumns(event.api);
    },
    [resizeColumns],
  );

  return (
    <div style={containerStyle}>
      <PriceDisplayGridContainer
        style={gridStyle}
        data-testid='price-display-grid-container'
        isScrollAtLeftMostPosition={isAtLeftEdge}
        scrollWidth={scrollWidth}>
        <AgGridReact<IBaseSeriesItem>
          ref={gridRef}
          rowDragManaged
          rowData={rowData}
          columnDefs={columnDefs as ColDef<IBaseSeriesItem>[]}
          defaultColDef={defaultColDef}
          getRowId={(params) => params?.data?.id}
          domLayout='autoHeight'
          theme={themeAlpine}
          overlayNoRowsTemplate={messages.General.NoRecordsFound}
          getRowClass={getRowClass}
          onGridReady={handleGridReady}
          onColumnMoved={handleColumnMoved}
          onRowDragEnd={handleRowDragEnd}
          onBodyScroll={handleBodyScroll}
          onGridSizeChanged={handleGridSizeChanged}
          onGridColumnsChanged={handleGridColumnsChanged}
          onStateUpdated={handleStateUpdated}
          suppressDragLeaveHidesColumns
          suppressFocusAfterRefresh
        />
      </PriceDisplayGridContainer>
    </div>
  );
};

export default PriceDisplayGrid;
