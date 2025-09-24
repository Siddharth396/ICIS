import React from 'react';
import { render, fireEvent } from '@testing-library/react';
import SelectCellEditor from '../SelectCellEditor';

describe('SelectCellEditor', () => {
  it('renders correctly and handles value change', () => {
    const onValueChange = jest.fn();
    const stopEditing = jest.fn();
    const options = ['Option 1', 'Option 2', 'Option 3'];
    const { container, getByText } = render(
      // @ts-ignore
      <SelectCellEditor
        value='Option 1'
        eventKey='Enter'
        colDef={{ cellEditorParams: { options } }}
        onValueChange={onValueChange}
        stopEditing={stopEditing}
      />,
    );
    const select = container.querySelector('.select__control') as Element;
    // Simulate focusing the select input
    fireEvent.focus(select);

    // Simulate pressing the ArrowDown key to open the dropdown
    fireEvent.keyDown(select, { key: 'ArrowDown', code: 'ArrowDown' });

    // Simulate clicking on an option
    fireEvent.click(getByText('Option 2'));
    expect(onValueChange).toHaveBeenCalledWith('Option 2');
    expect(stopEditing).toHaveBeenCalled();
  });
});
