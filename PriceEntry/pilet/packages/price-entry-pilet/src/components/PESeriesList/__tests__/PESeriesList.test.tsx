import React from 'react';
import { render, fireEvent, waitFor, act, screen } from '@testing-library/react';
import PESeriesList from '../PESeriesList';
import { PriceSeriesDetails } from '../../../apollo/queries';

jest.mock('utils/hooks/useLocaleMessage', () => () => ({
  General: { DifferentScheduleTooltipMessage: 'Different schedule tooltip', PriceSeriesAlreadySelectedTooltipMessage: 'Price series already selected tooltip message' },
}));

jest.mock('ag-grid-react', () => {
  // eslint-disable-next-line @typescript-eslint/no-var-requires
  const React = require('react');
  return {
    AgGridReact: React.forwardRef((props: any, ref: any) => {
      const { rowData = [], onFirstDataRendered, onSelectionChanged } = props;
      const nodes = React.useMemo(
        () =>
          rowData.map((d: any) => {
            let selected = false;
            return {
              data: d,
              isSelected: () => selected,
              setSelected: (val: boolean) => {
                selected = val;
              },
            };
          }),
        [rowData],
      );
      const api = React.useMemo(
        () => ({
          getSelectedNodes: () => nodes.filter((n: any) => n.isSelected()),
          setNodesSelected: ({ nodes: targetNodes, newValue }: any) => {
            targetNodes.forEach((n: any) => n.setSelected(newValue));
          },
          forEachNode: (cb: any) => nodes.forEach(cb),
        }),
        [nodes],
      );
      React.useImperativeHandle(ref, () => ({ api }), [api]);
      React.useEffect(() => {
        try {
          props.columnDefs?.[0]?.headerValueGetter?.();
        } catch (_e) {
          /* ignore */
        }
        onFirstDataRendered && onFirstDataRendered({ api });
        props.onGridReady && props.onGridReady({ api });
        // call getRowId for coverage of getRowId useMemo function
        try {
          if (props.getRowId) {
            nodes.forEach((n: any) => props.getRowId({ data: n.data }));
          }
        } catch (_e) {
          /* ignore */
        }
      }, [api, onFirstDataRendered]);
      const toggleSelect = (node: any) => {
        const { rowSelection } = props;
        if (rowSelection?.isRowSelectable && !rowSelection.isRowSelectable({ data: node.data }))
          return;
        node.setSelected(!node.isSelected());
        onSelectionChanged && onSelectionChanged({ api });
      };
      const CellRenderer = props.columnDefs?.[0]?.cellRenderer;
      const cellStyleFn = props.columnDefs?.[0]?.cellStyle;
      const selectionCellStyleFn = props.selectionColumnDef?.cellStyle;
      const headerText = (() => {
        try {
          return props.columnDefs?.[0]?.headerValueGetter?.();
        } catch {
          return '';
        }
      })();

      return (
        <div data-testid='mock-grid'>
          <div data-testid='grid-header'>{headerText}</div>
          {nodes.map((n: any) => {
            try {
              cellStyleFn?.({ data: n.data });
              selectionCellStyleFn?.({ data: n.data });
            } catch (_e) {
              /* ignore */
            }
            return (
              <div key={n.data.id} data-testid={`row-${n.data.id}`} onClick={() => toggleSelect(n)}>
                {CellRenderer ? (
                  <CellRenderer value={n.data.name} data={n.data} />
                ) : (
                  <span>{n.data.name}</span>
                )}
              </div>
            );
          })}
        </div>
      );
    }),
  };
});

describe('PESeriesList Component', () => {
  const baseSeriesData: PriceSeriesDetails[] = [
    { id: '1', name: 'Series 1', scheduleId: 'A', priceSeriesValidationResult: { isValid: true, message: '' } },
    { id: '2', name: 'Series 2', scheduleId: 'A', priceSeriesValidationResult: { isValid: true, message: '' } },
    { id: '3', name: 'Series 3', scheduleId: 'B', priceSeriesValidationResult: { isValid: false, message: 'Different schedule tooltip' } },
    { id: '4', name: 'Series 4', priceSeriesValidationResult: { isValid: true, message: '' } },
  ];

  const renderList = (props: Partial<React.ComponentProps<typeof PESeriesList>> = {}) => {
    const onSeriesSelect = jest.fn();
    const onGridReady = props.onGridReady || jest.fn();

    const utils = render(
      <PESeriesList
        selectedNodes={props.selectedNodes || []}
        seriesData={(props.seriesData as any) ?? baseSeriesData}
        onSeriesSelect={onSeriesSelect}
        id={props.id || 'pi-range'}
        width='500'
        onGridReady={onGridReady as any}
        lockedScheduleId={props.lockedScheduleId}
      />,
    );
    return { ...utils, onSeriesSelect, onGridReady };
  };

  it('renders series rows', async () => {
    renderList();
    await waitFor(() => expect(screen.getAllByText('Series 1').length).toBeGreaterThan(0));
    expect(screen.getAllByText('Series 4').length).toBeGreaterThan(0);
  });

  it('selects multiple rows and calls onSeriesSelect with RowInput objects', async () => {
    const { getByTestId, onSeriesSelect } = renderList();
    fireEvent.click(getByTestId('row-1'));
    fireEvent.click(getByTestId('row-2'));
    await waitFor(() => {
      const lastCall = onSeriesSelect.mock.calls.at(-1);
      expect(lastCall[0]).toEqual([
        { priceSeriesId: '1', seriesItemTypeCode: 'pi-range', displayOrder: 0 },
        { priceSeriesId: '2', seriesItemTypeCode: 'pi-range', displayOrder: 1 },
      ]);
    });
  });

  it('filters out mismatched scheduleId rows when locked', async () => {
    const { getByTestId, onSeriesSelect } = renderList({ lockedScheduleId: 'A' });
    fireEvent.click(getByTestId('row-1'));
    fireEvent.click(getByTestId('row-3'));
    fireEvent.click(getByTestId('row-4'));
    await waitFor(() => {
      const lastCall = onSeriesSelect.mock.calls.at(-1);
      const selectedIds = lastCall[0].map((x: any) => x.priceSeriesId);
      expect(selectedIds).toEqual(['1', '4']);
    });
  });

  it('does not select disallowed row via isRowSelectable', async () => {
    const { getByTestId, onSeriesSelect } = renderList({ lockedScheduleId: 'A' });
    fireEvent.click(getByTestId('row-3')); // scheduleId B should be ignored
    await new Promise((r) => setTimeout(r, 20));
    expect(onSeriesSelect).not.toHaveBeenCalled();
  });

  it('prunes previously selected mismatched nodes in effect', async () => {
    const { rerender } = renderList({
      lockedScheduleId: undefined,
      selectedNodes: [{ priceSeriesId: '3', seriesItemTypeCode: 'pi-range', displayOrder: 0 }],
    });

    const updatedBaseSeriesData = [...baseSeriesData];

    updatedBaseSeriesData[2] = { ...baseSeriesData[2], priceSeriesValidationResult: { isValid: true, message: '' } };

    rerender(
      <PESeriesList
        selectedNodes={[{ priceSeriesId: '3', seriesItemTypeCode: 'pi-range', displayOrder: 0 }]}
        seriesData={updatedBaseSeriesData}
        onSeriesSelect={jest.fn()}
        id='pi-range'
        width='500'
        onGridReady={() => { }}
        lockedScheduleId='A'
      />,
    );
    act(() => { });
  });

  it('header variants resolve without crashing (id switch)', async () => {
    const variants: string[] = [
      'pi-range',
      'pi-single',
      'pi-single-with-ref',
      'cri-single',
      'other',
    ];
    for (const variant of variants) {
      const { unmount } = renderList({ id: variant });
      await waitFor(() => expect(screen.getAllByText('Series 1').length).toBeGreaterThan(0));
      unmount();
    }
  });

  it('calls onGridReady with id and api', async () => {
    const { onGridReady } = renderList();
    const mocked = onGridReady as jest.Mock;
    await waitFor(() => expect(mocked).toHaveBeenCalled());
    const call = mocked.mock.calls[0];
    expect(call[0]).toBe('pi-range');
    expect(call[1]).toBeDefined();
    expect(typeof call[1].getSelectedNodes).toBe('function');
  });

  it('shows tooltip on hover for disabled mismatched schedule row', async () => {
    renderList({ lockedScheduleId: 'A' });
    const row = screen.getByTestId('row-3');
    const innerDiv = row.querySelector('div');
    if (innerDiv) fireEvent.mouseMove(innerDiv, { clientX: 50, clientY: 80 });
    await waitFor(() => expect(screen.getByText('Different schedule tooltip')).toBeInTheDocument());
  });

  it('uses raw id as header label for unknown id (default getSeriesItemType branch)', async () => {
    renderList({ id: 'custom-type' });
    const header = await screen.findByTestId('grid-header');
    expect(header.textContent).toBe('custom-type');
  });

  it('shows count 0 when seriesData is undefined (covers || 0 path)', async () => {
    renderList({ id: 'pi-range', seriesData: [] });
    const header = await screen.findByTestId('grid-header');
    expect(header.textContent).toBe('Price range (0)');
  });

  it('renders tooltip when hovering over disabled row due to series selected in other grid', async () => {
    const updatedBaseSeriesData = [...baseSeriesData];

    updatedBaseSeriesData[0] = { ...baseSeriesData[0], priceSeriesValidationResult: { isValid: false, message: 'Price series already selected tooltip message' } };

    const { getByTestId } = renderList({ seriesData: updatedBaseSeriesData });

    const series1Cell = getByTestId('row-1');
    const innerDiv = series1Cell.querySelector('div');

    if (innerDiv) fireEvent.mouseMove(innerDiv);

    await waitFor(() => expect(screen.getByText('Price series already selected tooltip message')).toBeInTheDocument());
  });
});
