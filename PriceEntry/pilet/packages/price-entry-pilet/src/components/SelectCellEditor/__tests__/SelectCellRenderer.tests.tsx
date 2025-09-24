import React from 'react';
import { render, screen } from '@testing-library/react';
import SelectCellRenderer from '../SelectCellRenderer';

describe('SelectCellRenderer', () => {
  it('renders correctly with the provided value', () => {
    const props = {
      value: 'Option 1',
      colDef: { cellEditorParams: { editable: true }, field: 'assessmentMethod' },
      data: { validationErrors: { assessmentMethod: 'Error message' } },
    };

    // @ts-ignore
    const { getByText } = render(<SelectCellRenderer {...props} />);

    // Check if the provided value is rendered
    expect(getByText('Option 1')).toBeInTheDocument();
  });

  it('does not render tooltip and info icon when error exists for referencePrice', () => {
    const props = {
      value: 'Test Value',
      colDef: { cellEditorParams: { editable: true }, field: 'referencePrice' },
      data: {
        validationErrors: { referencePrice: 'Error message' }, // has error, so haserror === true
        id: '1',
        referencePrice: { price: 100, seriesName: 'Test Series', periodLabel: 'Q1' },
      },
    };

    // @ts-ignore
    render(<SelectCellRenderer {...props} />);
    expect(screen.queryByText(/Price Series/)).toBeNull();
  });

  it('renders tooltip and info icon when no error exists for referencePrice', async () => {
    const props = {
      value: 'Test Value',
      colDef: { cellEditorParams: { editable: true }, field: 'referencePrice' },
      data: {
        validationErrors: {},
        id: '2',
        referencePrice: { price: 100, seriesName: 'Test Series', periodLabel: 'Q1' },
      },
    };

    // @ts-ignore
    render(<SelectCellRenderer {...props} />);
    expect(await screen.findByText(/Price Series/)).toBeInTheDocument();
  });
});
