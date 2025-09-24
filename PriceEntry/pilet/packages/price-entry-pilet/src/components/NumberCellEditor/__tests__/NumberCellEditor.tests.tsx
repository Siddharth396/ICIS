import React from 'react';
import { render, fireEvent } from '@testing-library/react';
import NumberCellEditor from '../NumberCellEditor';

describe('NumberCellEditor', () => {
  it('renders correctly', () => {
    const { getByTestId } = render(
      // @ts-ignore
      <NumberCellEditor
        value='123'
        colDef={{ cellEditorParams: { editable: true } }}
        onValueChange={() => {}}
      />,
    );
    const input = getByTestId('text-box') as HTMLInputElement;
    expect(input).toBeInTheDocument();
  });

  it('renders value when not editable', () => {
    const { getByText } = render(
      // @ts-ignore
      <NumberCellEditor
        value='123'
        colDef={{ cellEditorParams: { editable: false } }}
        onValueChange={() => {}}
      />,
    );
    const value = getByText('123');
    expect(value).toBeInTheDocument();
  });

  it('renders as editable when condition is met', () => {
    const { getByTestId } = render(
      // @ts-ignore
      <NumberCellEditor
        value='123'
        data={{ conditionField: 'conditionValue', valueDisplayPrecision: 3 }}
        colDef={{
          cellEditorParams: { editableWhen: { field: 'conditionField', value: 'conditionValue' } },
        }}
        onValueChange={() => {}}
      />,
    );
    const input = getByTestId('text-box') as HTMLInputElement;
    expect(input).toBeInTheDocument();
  });

  it('renders 0 value correctly', () => {
    const { getByTestId } = render(
      // @ts-ignore
      <NumberCellEditor
        value={0}
        colDef={{ cellEditorParams: { editable: true } }}
        onValueChange={() => {}}
      />,
    );
    const input = getByTestId('text-box') as HTMLInputElement;
    expect(input.value).toBe('0');
  });

  it('renders null value correctly', () => {
    const { getByTestId } = render(
      // @ts-ignore
      <NumberCellEditor
        value={null}
        colDef={{ cellEditorParams: { editable: true } }}
        onValueChange={() => {}}
      />,
    );
    const input = getByTestId('text-box') as HTMLInputElement;
    expect(input.value).toBe('');
  });

  it('renders undefined value correctly', () => {
    const { getByTestId } = render(
      // @ts-ignore
      <NumberCellEditor
        value={undefined}
        colDef={{ cellEditorParams: { editable: true } }}
        onValueChange={() => {}}
      />,
    );
    const input = getByTestId('text-box') as HTMLInputElement;
    expect(input.value).toBe('');
  });

  it('handles precision', () => {
    const onValueChange = jest.fn();
    const { getByTestId } = render(
      // @ts-ignore
      <NumberCellEditor
        data={{ valueDisplayPrecision: 3 }}
        colDef={{ cellEditorParams: { editable: true } }}
        onValueChange={onValueChange}
      />,
    );
    const input = getByTestId('text-box') as HTMLInputElement;
    fireEvent.change(input, { target: { value: '123.4567' } });
    fireEvent.change(input, { target: { value: '123.45' } });
    expect(onValueChange).toHaveBeenCalledWith('123.45');
  });

  it('calls onValueChange with the new value when the TextBox value changes', () => {
    const onValueChange = jest.fn();
    const props = {
      value: '1',
      colDef: { cellEditorParams: { editable: true }, field: 'assessmentMethod' },
      data: { validationErrors: { assessmentMethod: 'Error message' } },
      onValueChange,
    };
    // @ts-ignore
    const { getByRole } = render(<NumberCellEditor {...props} />);
    const textBox = getByRole('textbox');

    fireEvent.change(textBox, { target: { value: '2' } });

    expect(onValueChange).toHaveBeenCalledWith('2');
  });
});
