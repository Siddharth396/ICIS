import React from 'react';
import { render } from '@testing-library/react';
import NumberCellRenderer from '../NumberCellRenderer';

describe('NumberCellRenderer', () => {
  it('renders the value in a NumberCellRendererWrapper when editable', () => {
    const props = {
      value: 'test',
      colDef: { cellEditorParams: { editable: true }, field: 'assessmentMethod' },
      data: { validationErrors: { assessmentMethod: 'Error message' } },
    };

    // @ts-ignore
    const { getByText } = render(<NumberCellRenderer {...props} />);
    expect(getByText('test')).toBeInTheDocument();
  });

  it('renders the placeholder value when editable and with empty value', () => {
    const props = {
      value: undefined,
      colDef: { cellEditorParams: { editable: true }, field: 'premiumDiscount' },
      data: { lastAssessmentPremiumDiscount: '-2' },
    };

    // @ts-ignore
    const { getByText } = render(<NumberCellRenderer {...props} />);
    expect(getByText('-2')).toBeInTheDocument();
  });

  it('renders the placeholder value when editable and with null value', () => {
    const props = {
      value: null,
      colDef: { cellEditorParams: { editable: true }, field: 'premiumDiscount' },
      data: { lastAssessmentPremiumDiscount: '-2' },
    };

    // @ts-ignore
    const { getByText } = render(<NumberCellRenderer {...props} />);
    expect(getByText('-2')).toBeInTheDocument();
  });

  it('renders the value as is when editable but read only', () => {
    const props = {
      value: 'test',
      colDef: { cellEditorParams: { editable: true } },
      data: { readOnly: true },
    };

    // @ts-ignore
    const { getByText } = render(<NumberCellRenderer {...props} />);
    expect(getByText('test')).toBeInTheDocument();
  });

  it('renders the value in a NumberCellRendererWrapper when conditionally editable', () => {
    const props = {
      value: 'test',
      colDef: { cellEditorParams: { editableWhen: { field: 'field', value: 'value' } } },
      data: { field: 'value' },
    };

    // @ts-ignore
    const { getByText } = render(<NumberCellRenderer {...props} />);
    expect(getByText('test')).toBeInTheDocument();
  });

  it('renders the value as is when not editable', () => {
    const props = {
      value: 'test',
      colDef: { cellEditorParams: { editable: false } },
      data: {},
    };

    // @ts-ignore
    const { getByText } = render(<NumberCellRenderer {...props} />);
    expect(getByText('test')).toBeInTheDocument();
  });
});
