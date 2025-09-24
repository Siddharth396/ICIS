import React, { useRef, useMemo, useCallback } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { PriceSeriesDetails, RowInputPriceDisplayTable } from 'apollo/queries';
import {
  CellClassParams,
  CellStyleModule,
  ClientSideRowModelModule,
  ColDef,
  ModuleRegistry,
  RowNode,
  RowSelectionOptions,
} from 'ag-grid-community';

interface PDSeriesListProps {
  width: string;
  seriesData: PriceSeriesDetails[] | undefined;
  selectedNodes: RowInputPriceDisplayTable[] | undefined;
  onSeriesSelect: (selectedSeries: RowInputPriceDisplayTable[]) => void;
}

ModuleRegistry.registerModules([ClientSideRowModelModule, CellStyleModule]);

const PDSeriesList: React.FC<PDSeriesListProps> = ({
  width,
  seriesData,
  selectedNodes,
  onSeriesSelect,
}) => {
  const columnDefs: ColDef[] = [
    {
      flex: 1,
      headerValueGetter: () => {
        return 'Price Series';
      },
      field: 'name',
      cellClass: (params: CellClassParams) => `cell-${params.rowIndex}-${params.colDef.field}`,
    },
  ];
  const gridRef = useRef<any>(null);

  const defaultColDef = useMemo(() => {
    return {
      sortable: false,
      resizable: false,
      suppressMovable: true,
    };
  }, []);

  // istanbul ignore next
  const onSelectionChanged = () => {
    const newSelectedNodes = gridRef.current.api.getSelectedNodes();
    const selectedSeriesIds: any[] = newSelectedNodes.map((node: RowNode) => node.data);
    onSeriesSelect(
      selectedSeriesIds.map((seriesId, index) => ({
        priceSeriesId: seriesId?.id,
        displayOrder: index,
        seriesItemTypeCode: seriesId?.seriesItemTypeCode,
      })),
    );
  };

  const getRowId = useMemo(() => {
    return (params: any) => {
      const data = params.data;
      return data.id;
    };
  }, []);

  // istanbul ignore next
  const onFirstDataRendered = useCallback((params: any) => {
    const nodesToSelect: RowNode[] = [];
    params.api.forEachNode((node: RowNode) => {
      if (
        node.data &&
        selectedNodes &&
        selectedNodes.some((x) => x.priceSeriesId == node.data.id)
      ) {
        nodesToSelect.push(node);
      }
    });
    params.api.setNodesSelected({ nodes: nodesToSelect, newValue: true });
  }, []);

  const rowSelection = useMemo<RowSelectionOptions | 'single' | 'multiple'>(() => {
    return {
      mode: 'multiRow',
      enableClickSelection: true,
      enableSelectionWithoutKeys: true,
    };
  }, []);

  return (
    <div style={{ height: '100%', width }}>
      <AgGridReact
        ref={gridRef}
        rowBuffer={999}
        defaultColDef={defaultColDef}
        columnDefs={columnDefs}
        rowData={seriesData}
        rowSelection={rowSelection}
        getRowId={getRowId}
        onSelectionChanged={onSelectionChanged}
        onFirstDataRendered={onFirstDataRendered}
      />
    </div>
  );
};

export default PDSeriesList;
