import React, { useRef, useMemo, useCallback, useEffect, useState } from 'react';
import ReactDOM from 'react-dom';
import { AgGridReact } from 'ag-grid-react';
import { PriceSeriesDetails, RowInput } from 'apollo/queries';
import {
  CellStyleModule,
  ClientSideRowModelModule,
  ColDef,
  GridReadyEvent,
  ModuleRegistry,
  RowNode,
  RowSelectionModule,
  CellStyle,
  RowSelectionOptions,
  RenderApiModule,
  SelectionColumnDef,
  CellClassParams,
} from 'ag-grid-community';
import { theme } from '@icis/ui-kit';
import InfoTooltip from 'components/InfoTooltip/InfoTooltip';

ModuleRegistry.registerModules([
  ClientSideRowModelModule,
  RowSelectionModule,
  CellStyleModule,
  RenderApiModule,
]);

interface PESeriesListProps {
  width: string;
  id: string;
  seriesData: PriceSeriesDetails[] | undefined;
  selectedNodes: RowInput[] | undefined;
  lockedScheduleId?: string | null;
  onSeriesSelect: (selectedSeries: RowInput[], gridId: string) => void;
  onGridReady: (gridId: string, api: any) => void;
  clearLock?: () => void; // new: allow parent to clear schedule lock immediately
}

function getSeriesItemType(id: string, count: number) {
  switch (id) {
    case 'pi-single-with-ref':
      return 'Single value with reference (' + count + ')';
    case 'pi-single':
    case 'cri-single':
      return 'Single value (' + count + ')';
    case 'pi-range':
      return 'Price range (' + count + ')';
    default:
      return id;
  }
}

const PESeriesList: React.FC<PESeriesListProps> = ({
  width,
  id,
  seriesData,
  selectedNodes,
  lockedScheduleId,
  clearLock,
  onSeriesSelect,
  onGridReady,
}) => {
  const gridRef = useRef<any>(null);

  const isPriceSeriesValid = (priceSeriesDetail: PriceSeriesDetails | undefined): boolean => {
    return priceSeriesDetail?.priceSeriesValidationResult?.isValid ?? true;
  }

  const DisabledCell: React.FC<any> = (params) => {
    const isValid = isPriceSeriesValid(params.data);
    const message = params.data?.priceSeriesValidationResult?.message;

    const [pos, setPos] = useState<{ x: number; y: number } | null>(null);
    const lastUpdate = useRef<number>(0);

    // istanbul ignore next
    const onMove = (e: React.MouseEvent) => {
      if (isValid) return;
      const now = performance.now();
      if (now - lastUpdate.current > 30) {
        lastUpdate.current = now;
        setPos({ x: e.clientX, y: e.clientY });
      }
    };
    // istanbul ignore next
    const onLeave = () => !isValid && setPos(null);
    return (
      <>
        <div
          onMouseMove={onMove}
          onMouseLeave={onLeave}
          style={{
            display: 'flex',
            alignItems: 'center',
            width: '100%',
            height: '100%',
            cursor: !isValid ? 'not-allowed' : 'default',
            color: !isValid ? theme.colours.NEUTRALS_8 : undefined,
            opacity: !isValid ? 0.55 : 1,
          }}>
          <span style={{ flex: 1 }}>{params.value}</span>
        </div>
        {!isValid && message &&
          pos &&
          /* istanbul ignore next */ ReactDOM.createPortal(
            <InfoTooltip
              id={`disabled-series-${params.data?.id}`}
              message={message}
              followPosition={pos}
            />,
            document.body,
          )}
      </>
    );
  };

  const defaultColDef = useMemo<ColDef<PriceSeriesDetails>>(() => {
    return {
      sortable: false,
      resizable: false,
      suppressMovable: true,
    };
  }, []);

  // istanbul ignore next
  const onSelectionChanged = useCallback(() => {
    const api = gridRef.current?.api;
    if (!api) return;
    const allSelectedNodes: RowNode[] = api.getSelectedNodes();
    const selectedSeriesIds: string[] = allSelectedNodes
      .filter((node: RowNode<PriceSeriesDetails>) => {
        return isPriceSeriesValid(node.data);
      })
      .map((n) => n.data.id);

    if (selectedSeriesIds.length === 0 && lockedScheduleId && clearLock) {
      // Header checkbox (deselect all) or user cleared manually: unlock immediately
      clearLock();
    }

    onSeriesSelect(
      selectedSeriesIds.map((seriesId, index) => ({
        priceSeriesId: seriesId,
        seriesItemTypeCode: id,
        displayOrder: index,
      })),
      id,
    );
  }, [id, onSeriesSelect, clearLock, lockedScheduleId]);

  const getRowId = useMemo(() => (params: any) => params.data.id, []);

  // istanbul ignore next
  useEffect(() => {
    if (!gridRef.current || !gridRef.current.api) return;
    const api = gridRef.current.api;

    // If a lock is active, ensure no incompatible rows remain selected
    api.forEachNode((node: RowNode<PriceSeriesDetails>) => {
      if (
        node.isSelected() &&
        !isPriceSeriesValid(node.data)
      ) {
        node.setSelected(false);
      }
    });

    // Refresh only if underlying mock / real api supports it (tests may provide partial mock)
    if (typeof api.refreshCells === 'function') api.refreshCells({ force: true });
    if (typeof api.redrawRows === 'function') api.redrawRows();
  }, [lockedScheduleId]);

  const onFirstDataRendered = useCallback(
    (params: any) => {
      const nodesToSelect: RowNode[] = [];
      params.api.forEachNode((node: RowNode<PriceSeriesDetails>) => {
        if (
          node.data &&
          selectedNodes &&
          selectedNodes.some((x) => x.priceSeriesId === node.data!.id) &&
          isPriceSeriesValid(params.data)
        ) {
          nodesToSelect.push(node);
        }
      });
      params.api.setNodesSelected({ nodes: nodesToSelect, newValue: true });
    },
    [selectedNodes, lockedScheduleId],
  );

  const handleGridReady = (params: GridReadyEvent) => onGridReady(id, params.api);

  const columnDefs: ColDef[] = useMemo(
    () => [
      {
        flex: 1,
        headerValueGetter: () => getSeriesItemType(id, seriesData?.length || 0),
        field: 'name',
        cellRenderer: DisabledCell,
        cellStyle: (params: CellClassParams<PriceSeriesDetails>): CellStyle | undefined => {
          return !isPriceSeriesValid(params.data)
            ? { opacity: 0.55, color: theme.colours.NEUTRALS_8, cursor: 'not-allowed' }
            : undefined;
        }
      },
    ],
    [id, seriesData, lockedScheduleId],
  );

  const selectionColumnDef = useMemo<SelectionColumnDef>(
    () => ({
      width: 42,
      resizable: false,
      sortable: false,
      tooltipField: '',
      cellStyle: (params: CellClassParams<PriceSeriesDetails>): CellStyle | undefined => {
        return !isPriceSeriesValid(params.data)
          ? { opacity: 0.55, cursor: 'not-allowed', filter: 'grayscale(35%)' }
          : undefined;
      },
    }),
    [lockedScheduleId],
  );

  const isRowSelectable = (params: any): boolean => {
    return isPriceSeriesValid(params.data);
  };

  const rowSelection: RowSelectionOptions<PriceSeriesDetails> = {
    mode: 'multiRow',
    checkboxes: true,
    hideDisabledCheckboxes: false,
    enableClickSelection: true,
    enableSelectionWithoutKeys: true,
    isRowSelectable: isRowSelectable,
  };

  return (
    <div style={{ height: '100%', width }}>
      <AgGridReact
        ref={gridRef}
        defaultColDef={defaultColDef}
        columnDefs={columnDefs}
        rowData={seriesData}
        rowSelection={rowSelection}
        selectionColumnDef={selectionColumnDef}
        getRowId={getRowId}
        onSelectionChanged={onSelectionChanged}
        onFirstDataRendered={onFirstDataRendered}
        onGridReady={handleGridReady}
      />
    </div>
  );
};

export default PESeriesList;
